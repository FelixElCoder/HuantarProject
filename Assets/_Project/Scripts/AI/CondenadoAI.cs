using UnityEngine;
using UnityEngine.AI;
using Huantar.Core;

namespace Huantar.AI
{
    [RequireComponent(typeof(PerceptionSensor))]
    public class CondenadoAI : CreatureFSM
    {
        [Header("Deambular")]
        [SerializeField] Transform[] patrolPoints;
        [SerializeField] float wanderSpeed = 1.2f;
        [SerializeField] Vector2 waitAtPoint = new Vector2(1.5f, 4f);
        [SerializeField] bool randomOrder = true;

        [Header("Investigar")]
        [SerializeField] float investigateSpeed = 1.5f;
        [SerializeField] float investigateLookTime = 3f;
        [SerializeField] float investigateMaxTime = 12f;

        [Header("Perseguir")]
        [SerializeField] float chaseSpeed = 1.8f;
        [SerializeField] float repathInterval = 0.25f;
        [SerializeField] float loseSightGrace = 1.5f;
        [SerializeField] float captureDistance = 1.2f;

        [Header("Buscar")]
        [SerializeField] float searchSpeed = 1.4f;
        [SerializeField] float searchDuration = 8f;
        [SerializeField] float searchRadius = 6f;

        [Header("Correa (territorio)")]
        [SerializeField] float leashExtra = 25f;

        [Header("Giro")]
        [SerializeField] float turnSpeed = 90f;

        PerceptionSensor perception;

        int patrolIndex = -1;
        float waitTime;
        bool waiting;

        float repathTimer;
        float timeSinceSeen;
        Vector3 chaseTarget;

        bool lookingAround;
        float lookTimer;

        Vector3 territoryCenter;
        float territoryRadius;

        Vector3 spawnPosition;

        protected override CreatureState InitialState => CreatureState.Deambular;

        protected override void Awake()
        {
            base.Awake();
            perception = GetComponent<PerceptionSensor>();
            ComputeTerritory();
            spawnPosition = transform.position;
        }

        // ───────── Entrar a cada estado ─────────
        protected override void OnEnterState(CreatureState state)
        {
            agent.isStopped = false;
            lookingAround = false;

            switch (state)
            {
                case CreatureState.Deambular:
                    agent.speed = wanderSpeed;
                    waiting = false;
                    GoToNextPatrolPoint();
                    break;

                case CreatureState.Investigar:
                    agent.speed = investigateSpeed;
                    agent.SetDestination(perception.LastHeardPosition);
                    break;

                case CreatureState.Perseguir:
                    agent.speed = chaseSpeed;
                    repathTimer = 0f;
                    timeSinceSeen = 0f;
                    chaseTarget = perception.LastKnownPlayerPosition;
                    break;

                case CreatureState.Buscar:
                    agent.speed = searchSpeed;
                    agent.SetDestination(chaseTarget);
                    break;

                case CreatureState.Capturar:
                    agent.isStopped = true;
                    GameEvents.RaisePlayerCaught(transform);
                    break;
            }
        }

        // ───────── Actualizar cada estado ─────────
        protected override void OnUpdateState(CreatureState state)
        {
            switch (state)
            {
                case CreatureState.Deambular: UpdateDeambular(); break;
                case CreatureState.Investigar: UpdateInvestigar(); break;
                case CreatureState.Perseguir: UpdatePerseguir(); break;
                case CreatureState.Buscar: UpdateBuscar(); break;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvents.PlayerRespawned += OnPlayerRespawned;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvents.PlayerRespawned -= OnPlayerRespawned;
        }

        void OnPlayerRespawned()
        {
            agent.Warp(spawnPosition);
            perception.ResetPerception();
            ChangeState(CreatureState.Deambular);
        }

        void UpdateDeambular()
        {
            if (perception.CanSeePlayer) { ChangeState(CreatureState.Perseguir); return; }
            if (perception.HeardPlayer) { ChangeState(CreatureState.Investigar); return; }
            if (Suspicious()) return;

            if (waiting)
            {
                waitTime -= Time.deltaTime;
                if (waitTime <= 0f)
                {
                    waiting = false;
                    GoToNextPatrolPoint();
                }
                return;
            }

            if (HasArrived())
            {
                waiting = true;
                waitTime = Random.Range(waitAtPoint.x, waitAtPoint.y);
            }
        }

        void UpdateInvestigar()
        {
            if (perception.CanSeePlayer) { ChangeState(CreatureState.Perseguir); return; }
            if (stateTimer > investigateMaxTime) { ChangeState(CreatureState.Deambular); return; }

            if (perception.HeardPlayer)
            {
                agent.SetDestination(perception.LastHeardPosition);
                lookingAround = false;
            }

            if (Suspicious()) return;

            if (!lookingAround && HasArrived())
            {
                lookingAround = true;
                lookTimer = investigateLookTime;
            }

            if (lookingAround)
            {
                transform.Rotate(0f, turnSpeed * 0.6f * Time.deltaTime, 0f);
                lookTimer -= Time.deltaTime;
                if (lookTimer <= 0f) ChangeState(CreatureState.Deambular);
            }
        }

        void UpdatePerseguir()
        {
            Transform player = perception.Player;
            if (player == null) return;

            if (FlatDistance(transform.position, player.position) <= captureDistance)
            {
                ChangeState(CreatureState.Capturar);
                return;
            }

            if (OutsideTerritory()) { ChangeState(CreatureState.Deambular); return; }

            if (perception.CanSeePlayer)
            {
                timeSinceSeen = 0f;
                chaseTarget = player.position;
            }
            else if (perception.HeardPlayer)
            {
                timeSinceSeen = 0f;
                chaseTarget = perception.LastHeardPosition;
            }
            else
            {
                timeSinceSeen += Time.deltaTime;
                if (timeSinceSeen > loseSightGrace)
                {
                    ChangeState(CreatureState.Buscar);
                    return;
                }
            }

            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                repathTimer = repathInterval;
                agent.SetDestination(chaseTarget);
            }
        }

        void UpdateBuscar()
        {
            if (perception.CanSeePlayer) { ChangeState(CreatureState.Perseguir); return; }
            if (perception.HeardPlayer) { ChangeState(CreatureState.Investigar); return; }
            if (OutsideTerritory() || stateTimer > searchDuration)
            {
                ChangeState(CreatureState.Deambular);
                return;
            }

            if (HasArrived())
            {
                Vector3 random = chaseTarget + Random.insideUnitSphere * searchRadius;
                if (NavMesh.SamplePosition(random, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
        }

        // ───────── Utilidades ─────────
        bool Suspicious()
        {
            if (perception.Detection01 > 0.01f && perception.Player != null)
            {
                agent.isStopped = true;
                FaceTowards(perception.Player.position);
                return true;
            }
            agent.isStopped = false;
            return false;
        }

        void FaceTowards(Vector3 target)
        {
            Vector3 dir = target - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        void GoToNextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            if (randomOrder && patrolPoints.Length > 1)
            {
                int next;
                do next = Random.Range(0, patrolPoints.Length);
                while (next == patrolIndex);
                patrolIndex = next;
            }
            else
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }

            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        void ComputeTerritory()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                territoryCenter = transform.position;
                territoryRadius = leashExtra;
                return;
            }

            Vector3 sum = Vector3.zero;
            foreach (Transform p in patrolPoints) sum += p.position;
            territoryCenter = sum / patrolPoints.Length;

            float maxDist = 0f;
            foreach (Transform p in patrolPoints)
                maxDist = Mathf.Max(maxDist, FlatDistance(p.position, territoryCenter));

            territoryRadius = maxDist + leashExtra;
        }

        bool OutsideTerritory() => FlatDistance(transform.position, territoryCenter) > territoryRadius;

        static float FlatDistance(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        void OnDrawGizmosSelected()
        {
            if (patrolPoints == null) return;

            Gizmos.color = new Color(0.9f, 0.3f, 0.2f);
            foreach (Transform p in patrolPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, 0.6f);

            if (!Application.isPlaying) ComputeTerritory();
            Gizmos.color = new Color(0.6f, 0.2f, 0.9f);
            const int seg = 48;
            Vector3 prev = territoryCenter + new Vector3(territoryRadius, 0f, 0f);
            for (int i = 1; i <= seg; i++)
            {
                float a = i / (float)seg * Mathf.PI * 2f;
                Vector3 next = territoryCenter + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * territoryRadius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
using UnityEngine;

namespace Huantar.AI
{
    public class CondenadoAI : CreatureFSM
    {
        [Header("Deambular")]
        [SerializeField] Transform[] patrolPoints;
        [SerializeField] float wanderSpeed = 1.2f;
        [SerializeField] Vector2 waitAtPoint = new Vector2(1.5f, 4f);
        [SerializeField] bool randomOrder = true;

        int patrolIndex = -1;
        float waitTime;
        bool waiting;

        protected override CreatureState InitialState => CreatureState.Deambular;

        protected override void OnEnterState(CreatureState state)
        {
            switch (state)
            {
                case CreatureState.Deambular:
                    agent.speed = wanderSpeed;
                    waiting = false;
                    GoToNextPatrolPoint();
                    break;
            }
        }

        protected override void OnUpdateState(CreatureState state)
        {
            switch (state)
            {
                case CreatureState.Deambular:
                    UpdateDeambular();
                    break;
            }
        }

        void UpdateDeambular()
        {
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

        void OnDrawGizmosSelected()
        {
            if (patrolPoints == null) return;
            Gizmos.color = new Color(0.9f, 0.3f, 0.2f);
            foreach (Transform p in patrolPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, 0.6f);
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

namespace Huantar.AI
{
    public enum CreatureState { Deambular, Investigar, Perseguir, Buscar, Capturar }

    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class CreatureFSM : MonoBehaviour
    {
        [Header("Depuración")]
        [SerializeField] CreatureState currentState;

        public CreatureState CurrentState => currentState;

        protected NavMeshAgent agent;
        protected Transform player;
        protected float stateTimer;

        bool initialized;

        protected abstract CreatureState InitialState { get; }

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Start()
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;

            ChangeState(InitialState);
        }

        protected virtual void Update()
        {
            stateTimer += Time.deltaTime;
            OnUpdateState(currentState);
        }

        protected void ChangeState(CreatureState next)
        {
            if (initialized && next == currentState) return;

            if (initialized) OnExitState(currentState);
            currentState = next;
            stateTimer = 0f;
            initialized = true;
            OnEnterState(next);
        }

        protected virtual void OnEnterState(CreatureState state) { }
        protected virtual void OnExitState(CreatureState state) { }
        protected abstract void OnUpdateState(CreatureState state);

        protected bool HasArrived()
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f;
        }
    }
}
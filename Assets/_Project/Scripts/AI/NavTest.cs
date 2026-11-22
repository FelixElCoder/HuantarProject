using UnityEngine;
using UnityEngine.AI;

namespace Huantar.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavTest : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] bool followContinuously = false;

        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) target = player.transform;
            }

            Debug.Log($"[NavTest] Target: {(target != null ? target.name : "NULO")}");
            Debug.Log($"[NavTest] ¿Está sobre la NavMesh?: {agent.isOnNavMesh}");

            if (target != null)
                agent.SetDestination(target.position);
        }

        void Update()
        {
            if (followContinuously && target != null)
                agent.SetDestination(target.position);
        }
    }
}
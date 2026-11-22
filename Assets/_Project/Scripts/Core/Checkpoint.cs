using UnityEngine;

namespace Huantar.Core
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] string id = "CP_01";
        [SerializeField] Transform spawnPoint;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || CheckpointManager.Instance == null) return;
            Transform p = spawnPoint != null ? spawnPoint : transform;
            CheckpointManager.Instance.Register(id, p.position, p.rotation);
        }

        void OnDrawGizmos()
        {
            Transform p = spawnPoint != null ? spawnPoint : transform;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(p.position, 0.4f);
            Gizmos.DrawLine(p.position, p.position + p.forward * 1.5f);
        }
    }
}
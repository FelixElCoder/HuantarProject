using UnityEngine;
using Huantar.Player;

namespace Huantar.Core
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        Vector3 lastPosition;
        Quaternion lastRotation;
        string lastId = "Inicio";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                lastPosition = player.transform.position;
                lastRotation = player.transform.rotation;
            }
        }

        public void Register(string id, Vector3 position, Quaternion rotation)
        {
            if (id == lastId) return;
            lastId = id;
            lastPosition = position;
            lastRotation = rotation;
            Debug.Log($"[Checkpoint] Guardado: {id}");
            GameEvents.RaiseCheckpointReached(id);
        }

        public void Respawn()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            player.GetComponent<FirstPersonController>().Teleport(lastPosition, lastRotation);
            GameEvents.RaisePlayerRespawned();
        }
    }
}
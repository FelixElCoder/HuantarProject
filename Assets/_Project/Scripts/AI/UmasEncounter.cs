using UnityEngine;
using Huantar.Core;
using Huantar.Player;

namespace Huantar.AI
{
    [RequireComponent(typeof(Collider))]
    public class UmasEncounter : MonoBehaviour
    {
        [SerializeField] UmaAI[] umas;
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] AudioSource alertSource;

        FirstPersonController player;
        bool triggered;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnEnable() => GameEvents.PlayerRespawned += ResetEncounter;
        void OnDisable() => GameEvents.PlayerRespawned -= ResetEncounter;

        void Start()
        {
            foreach (UmaAI u in umas) u.Deactivate();
        }

        void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player")) return;
            triggered = true;

            player = other.GetComponent<FirstPersonController>();
            player.Adrenaline = true;

            for (int i = 0; i < umas.Length; i++)
            {
                Transform sp = spawnPoints[i % spawnPoints.Length];
                umas[i].Activate(player.transform, sp.position);
            }

            if (alertSource != null) alertSource.Play();
            Debug.Log("[Umas] ¡Corre!");
        }

        public void EndEncounter()
        {
            foreach (UmaAI u in umas) u.Deactivate();
            if (player != null) player.Adrenaline = false;
        }

        void ResetEncounter()
        {
            EndEncounter();
            triggered = false;
        }
    }
}
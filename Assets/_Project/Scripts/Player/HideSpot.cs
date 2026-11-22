using UnityEngine;

namespace Huantar.Player
{
    [RequireComponent(typeof(Collider))]
    public class HideSpot : MonoBehaviour
    {
        static int playerInsideCount;
        public static bool PlayerInside => playerInsideCount > 0;

        bool playerInsideThis;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => playerInsideCount = 0;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || playerInsideThis) return;
            playerInsideThis = true;
            playerInsideCount++;
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") || !playerInsideThis) return;
            playerInsideThis = false;
            playerInsideCount--;
        }

        void OnDisable()
        {
            if (!playerInsideThis) return;
            playerInsideThis = false;
            playerInsideCount--;
        }
    }
}
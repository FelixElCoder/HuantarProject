using UnityEngine;

namespace Huantar.Core
{
    [RequireComponent(typeof(Collider))]
    public class EndOfBetaTrigger : MonoBehaviour
    {
        bool fired;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (fired || !other.CompareTag("Player")) return;
            fired = true;
            GameEvents.RaiseBetaFinished();
        }
    }
}
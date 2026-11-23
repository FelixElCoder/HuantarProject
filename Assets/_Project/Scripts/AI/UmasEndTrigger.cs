using UnityEngine;

namespace Huantar.AI
{
    [RequireComponent(typeof(Collider))]
    public class UmasEndTrigger : MonoBehaviour
    {
        [SerializeField] UmasEncounter encounter;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && encounter != null)
                encounter.EndEncounter();
        }
    }
}
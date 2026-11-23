using UnityEngine;

namespace Huantar.Narrative
{
    [RequireComponent(typeof(Collider))]
    public class MonologueTrigger : MonoBehaviour
    {
        [SerializeField] DialogueData dialogue;
        [SerializeField] bool playOnce = true;
        [SerializeField] bool playOnStart = false;

        bool played;

        void Reset() => GetComponent<Collider>().isTrigger = true;

        void Start()
        {
            if (playOnStart) Fire();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) Fire();
        }

        void Fire()
        {
            if (played && playOnce) return;
            if (DialogueSystem.Instance == null) return;
            played = true;
            DialogueSystem.Instance.Play(dialogue);
        }
    }
}
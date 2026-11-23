using UnityEngine;
using Huantar.Core;

namespace Huantar.Narrative
{
    public class Examinable : MonoBehaviour, IInteractable
    {
        [SerializeField] string prompt = "Examinar";
        [SerializeField] DialogueData examineText;
        [SerializeField] NotebookEntryData notebookEntry;

        public string Prompt
        {
            get
            {
                bool isNew = notebookEntry != null && FieldNotebook.Instance != null
                             && !FieldNotebook.Instance.IsDiscovered(notebookEntry);
                return isNew ? $"{prompt}  (nuevo)" : prompt;
            }
        }

        public void Interact()
        {
            DialogueSystem dialogue = DialogueSystem.Instance;
            if (dialogue != null && dialogue.IsPlaying) return;

            if (dialogue != null && examineText != null)
                dialogue.Play(examineText, RegisterEntry);
            else
                RegisterEntry();
        }

        void RegisterEntry()
        {
            if (notebookEntry != null && FieldNotebook.Instance != null)
                FieldNotebook.Instance.Register(notebookEntry);
        }
    }
}
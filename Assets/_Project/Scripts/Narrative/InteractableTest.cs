using UnityEngine;
using Huantar.Core;

namespace Huantar.Narrative
{
    public class InteractableTest : MonoBehaviour, IInteractable
    {
        [SerializeField] string prompt = "Examinar";
        public string Prompt => prompt;

        public void Interact() => Debug.Log($"[Interacción] {name}");
    }
}
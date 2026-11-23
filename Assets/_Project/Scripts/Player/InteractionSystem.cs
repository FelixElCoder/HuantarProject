using UnityEngine;
using UnityEngine.InputSystem;
using Huantar.Core;

namespace Huantar.Player
{
    public class InteractionSystem : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField] float range = 2.5f;
        [SerializeField] LayerMask mask = ~0;

        public IInteractable Current { get; private set; }

        FirstPersonController fpc;

        void Awake()
        {
            fpc = GetComponent<FirstPersonController>();
        }

        void Update()
        {
            Current = null;
            if (fpc != null && !fpc.InputEnabled) return;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward,
                                out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore))
            {
                Current = hit.collider.GetComponentInParent<IInteractable>();
            }

            var kb = Keyboard.current;
            if (Current != null && kb != null && kb.eKey.wasPressedThisFrame)
                Current.Interact();
        }
    }
}
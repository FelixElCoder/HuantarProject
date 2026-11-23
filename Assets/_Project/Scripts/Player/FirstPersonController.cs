using UnityEngine;
using UnityEngine.InputSystem;

namespace Huantar.Player{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        //Caminar
        
        [Header("Referencias")] //Da el nombre de la sección
        [SerializeField] Transform cameraPivot;

        [Header("Movimiento")]
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float gravity = -9.81f;

        [Header("Cámara")]
        [SerializeField] float mouseSensitivity = 0.1f;
        [SerializeField] float maxPitch = 80f;

        CharacterController controller;
        float pitch;
        float verticalVelocity;
        
        //Correr y agacharse

        [Header("Correr")]
        [SerializeField] float runSpeed = 4f;
        [SerializeField] float maxStamina = 5f;        // segundos de carrera
        [SerializeField] float staminaRegenRate = 1f;  // segundos recuperados por segundo
        [SerializeField] float regenDelay = 1f;
        [SerializeField, Range(0f, 1f)] float exhaustedThreshold = 0.3f;

        [Header("Agacharse")]
        [SerializeField] float crouchSpeed = 1.2f;
        [SerializeField] float standHeight = 1.8f;
        [SerializeField] float crouchHeight = 1.1f;
        [SerializeField] float standEyeHeight = 1.65f;
        [SerializeField] float crouchEyeHeight = 0.95f;
        [SerializeField] float crouchTransitionSpeed = 8f;

        float stamina;
        float regenTimer;
        bool exhausted;
        float currentEyeHeight;
        Vector2 input;
        LanternController lantern;

        public bool IsRunning { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsExhausted => exhausted;
        public bool IsHidden => IsCrouching && HideSpot.PlayerInside && (lantern == null || !lantern.IsOn);
        public float Stamina01 => stamina / maxStamina;
        public float NoiseLevel { get; private set; }
        //
        readonly System.Collections.Generic.HashSet<object> inputLocks = new();

        public bool InputEnabled => inputLocks.Count == 0;

        public void SetInputLock(object owner, bool locked)
        {
            if (locked) inputLocks.Add(owner);
            else inputLocks.Remove(owner);
        }
        //
        public bool Adrenaline { get; set; }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            stamina = maxStamina;
            currentEyeHeight = standEyeHeight;
            lantern = GetComponent<LanternController>();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!InputEnabled) return;
            var kb = Keyboard.current;
            if (kb == null) return;

            ReadInput(kb);
            HandleLook();
            HandleCrouch(kb);
            HandleStamina(kb);
            HandleMove();
        }

        void ReadInput(Keyboard kb)
        {
            input = Vector2.zero;
            if (kb.wKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed) input.y -= 1f;
            if (kb.dKey.isPressed) input.x += 1f;
            if (kb.aKey.isPressed) input.x -= 1f;
            input = Vector2.ClampMagnitude(input, 1f);
        }

        void HandleLook()
        {
            if (Mouse.current == null) return;

            Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            transform.Rotate(Vector3.up * delta.x);

            pitch -= delta.y;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void HandleCrouch(Keyboard kb)
        {
            bool wantsCrouch = kb.leftCtrlKey.isPressed || kb.cKey.isPressed;

            if (wantsCrouch) IsCrouching = true;
            else if (IsCrouching && CanStandUp()) IsCrouching = false;

            float targetHeight = IsCrouching ? crouchHeight : standHeight;
            controller.height = targetHeight;
            controller.center = new Vector3(0f, targetHeight / 2f, 0f);

            float targetEye = IsCrouching ? crouchEyeHeight : standEyeHeight;
            currentEyeHeight = Mathf.Lerp(currentEyeHeight, targetEye, crouchTransitionSpeed * Time.deltaTime);
            cameraPivot.localPosition = new Vector3(0f, currentEyeHeight, 0f);
        }

        bool CanStandUp()
        {
            float radius = controller.radius;
            Vector3 origin = transform.position + Vector3.up * (controller.height - radius);
            float distance = standHeight - controller.height;
            return !Physics.SphereCast(origin, radius * 0.9f, Vector3.up, out _, distance,
                                       ~0, QueryTriggerInteraction.Ignore);
        }

        void HandleStamina(Keyboard kb)
        {
            bool moving = input.sqrMagnitude > 0.01f;
            bool wantsRun = kb.leftShiftKey.isPressed && moving;
            IsRunning = wantsRun && !exhausted && !IsCrouching;

            if (IsRunning)
            {
                stamina -= Time.deltaTime;
                regenTimer = regenDelay;
                if (stamina <= 0f)
                {
                    stamina = 0f;
                    exhausted = true;
                }
            }
            else
            {
                regenTimer -= Time.deltaTime;
                if (regenTimer <= 0f)
                    stamina = Mathf.Min(maxStamina, stamina + staminaRegenRate * Time.deltaTime);
            }
            
            if (Adrenaline)
            {
                stamina = maxStamina;
                exhausted = false;
            }

            if (exhausted && stamina >= maxStamina * exhaustedThreshold)
                exhausted = false;

            if (!moving) NoiseLevel = 0f;
            else if (IsRunning) NoiseLevel = 1f;
            else if (IsCrouching) NoiseLevel = 0.2f;
            else NoiseLevel = 0.5f;
        }

        void HandleMove()
        {
            float speed = IsCrouching ? crouchSpeed : IsRunning ? runSpeed : walkSpeed;

            Vector3 move = transform.right * input.x + transform.forward * input.y;
            Vector3 velocity = move * speed;

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            velocity.y = verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            controller.enabled = true;

            pitch = 0f;
            cameraPivot.localRotation = Quaternion.identity;
            verticalVelocity = 0f;
            stamina = maxStamina;
            exhausted = false;
        }

        public void FaceTowards(Vector3 worldPoint)
        {
            Vector3 flat = worldPoint - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(flat);

            Vector3 toTarget = worldPoint - cameraPivot.position;
            float vertical = Mathf.Atan2(toTarget.y, new Vector2(toTarget.x, toTarget.z).magnitude) * Mathf.Rad2Deg;
            pitch = Mathf.Clamp(-vertical, -maxPitch, maxPitch);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}


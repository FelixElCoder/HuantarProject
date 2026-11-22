using UnityEngine;
using UnityEngine.InputSystem;
using Huantar.Core;

namespace Huantar.Player
{
    public class LanternController : MonoBehaviour
    {
        [SerializeField] Light lanternLight;
        [SerializeField] Renderer flameRenderer;
        [SerializeField] bool hasLantern = true;
        [SerializeField] bool startOn = true;

        [Header("Parpadeo de aviso")]
        [SerializeField] float warningRange = 15f;
        [SerializeField] float flickerSpeed = 14f;
        [SerializeField, Range(0f, 1f)] float maxFlickerDepth = 0.85f;
        [SerializeField, Range(0f, 0.3f)] float idleFlicker = 0.08f;

        public bool HasLantern => hasLantern;
        public bool IsOn { get; private set; }
        public float Threat01 { get; private set; }

        FirstPersonController fpc;
        float baseIntensity;
        float seed;

        void Awake()
        {
            fpc = GetComponent<FirstPersonController>();
            baseIntensity = lanternLight.intensity;
            seed = Random.value * 100f;
            SetOn(startOn && hasLantern);
        }

        void Update()
        {
            var kb = Keyboard.current;
            bool canUse = hasLantern && (fpc == null || fpc.InputEnabled);
            if (kb != null && canUse && kb.fKey.wasPressedThisFrame)
                SetOn(!IsOn);

            Threat01 = NearestThreat01();
            if (!IsOn) return;

            float idle = Mathf.PerlinNoise(seed, Time.time * 2f) * idleFlicker;
            float warning = Mathf.PerlinNoise(seed + 50f, Time.time * flickerSpeed) * maxFlickerDepth * Threat01;
            lanternLight.intensity = baseIntensity * Mathf.Clamp01(1f - idle - warning);
        }

        float NearestThreat01()
        {
            float nearest = float.MaxValue;
            foreach (Transform t in ThreatRegistry.Threats)
            {
                if (t == null) continue;
                float d = Vector3.Distance(transform.position, t.position);
                if (d < nearest) nearest = d;
            }
            return nearest >= warningRange ? 0f : 1f - nearest / warningRange;
        }

        public void SetOn(bool on)
        {
            IsOn = on;
            lanternLight.enabled = on;
            if (flameRenderer != null) flameRenderer.enabled = on;
        }

        public void GiveLantern()
        {
            hasLantern = true;
            SetOn(true);
        }
    }
}
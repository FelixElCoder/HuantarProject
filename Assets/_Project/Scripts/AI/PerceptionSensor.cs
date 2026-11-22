using UnityEngine;
using Huantar.Player;

namespace Huantar.AI
{
    public class PerceptionSensor : MonoBehaviour
    {
        [Header("Vista")]
        [SerializeField] float eyeHeight = 2.3f;
        [SerializeField, Range(10f, 180f)] float viewAngle = 100f;
        [SerializeField] float visionRange = 12f;
        [SerializeField] float lanternRangeMultiplier = 2f;
        [SerializeField] LayerMask obstacleMask = ~0;

        [Header("Detección progresiva")]
        [SerializeField] float detectionTimeFar = 1.5f;
        [SerializeField] float detectionTimeNear = 0.25f;
        [SerializeField] float detectionDecay = 0.5f;
        [SerializeField] float proximityRadius = 1.5f;

        [Header("Oído")]
        [SerializeField] float hearingRange = 20f;
        [SerializeField] float riverHearingMultiplier = 0.5f;

        [Header("Rendimiento")]
        [SerializeField] float tickInterval = 0.1f;

        public bool CanSeePlayer { get; private set; }
        public bool HeardPlayer { get; private set; }
        public float Detection01 { get; private set; }
        public Vector3 LastKnownPlayerPosition { get; private set; }
        public Vector3 LastHeardPosition { get; private set; }
        public bool LanternOn { get; set; }
        public Transform Player => player;

        float CurrentVisionRange => visionRange * (LanternOn ? lanternRangeMultiplier : 1f);

        Transform player;
        CharacterController playerController;
        FirstPersonController playerFPC;
        float tickTimer;
        bool inSight;
        float lastDistance = float.MaxValue;

        void Start()
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null) return;
            player = p.transform;
            playerController = p.GetComponent<CharacterController>();
            playerFPC = p.GetComponent<FirstPersonController>();
        }

        void Update()
        {
            if (player == null) return;

            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;
                inSight = CheckVision(out lastDistance);
                HeardPlayer = CheckHearing();
            }

            UpdateDetection(Time.deltaTime);
        }

        Vector3 EyePosition => transform.position + Vector3.up * eyeHeight;

        Vector3 PlayerTargetPoint()
        {
            float h = playerController != null ? playerController.height * 0.85f : 1.5f;
            return player.position + Vector3.up * h;
        }

        bool CheckVision(out float distance)
        {
            Vector3 eye = EyePosition;
            Vector3 target = PlayerTargetPoint();
            distance = Vector3.Distance(eye, target);

            if (distance <= proximityRadius) return true;
            if (distance > CurrentVisionRange) return false;

            Vector3 flatDir = target - eye;
            flatDir.y = 0f;
            if (Vector3.Angle(transform.forward, flatDir) > viewAngle * 0.5f) return false;

            if (Physics.Linecast(eye, target, out RaycastHit hit, obstacleMask, QueryTriggerInteraction.Ignore))
                return hit.transform == player || hit.transform.IsChildOf(player);

            return true;
        }

        bool CheckHearing()
        {
            float noise = playerFPC != null ? playerFPC.NoiseLevel : 0.5f;
            if (noise <= 0f) return false;

            float range = hearingRange * noise;
            if (RiverNoiseZone.PlayerInside) range *= riverHearingMultiplier;

            if (Vector3.Distance(transform.position, player.position) > range) return false;

            LastHeardPosition = player.position;
            return true;
        }

        void UpdateDetection(float dt)
        {
            if (inSight)
            {
                if (lastDistance <= proximityRadius)
                {
                    Detection01 = 1f;
                }
                else
                {
                    float t = Mathf.InverseLerp(proximityRadius, CurrentVisionRange, lastDistance);
                    float timeToDetect = Mathf.Lerp(detectionTimeNear, detectionTimeFar, t);
                    Detection01 = Mathf.Min(1f, Detection01 + dt / timeToDetect);
                }
            }
            else
            {
                Detection01 = Mathf.Max(0f, Detection01 - detectionDecay * dt);
            }

            CanSeePlayer = inSight && Detection01 >= 1f;
            if (CanSeePlayer) LastKnownPlayerPosition = player.position;
        }

        void OnDrawGizmos()
        {
            Vector3 eye = transform.position + Vector3.up * eyeHeight;
            float range = Application.isPlaying ? CurrentVisionRange : visionRange;

            Color c = !Application.isPlaying ? new Color(0.4f, 0.9f, 0.4f)
                    : CanSeePlayer ? Color.red
                    : Detection01 > 0f ? new Color(1f, 0.55f, 0f)
                    : HeardPlayer ? Color.yellow
                    : new Color(0.4f, 0.9f, 0.4f);
            Gizmos.color = c;

            float half = viewAngle * 0.5f;
            Vector3 left = Quaternion.Euler(0f, -half, 0f) * transform.forward;
            Vector3 right = Quaternion.Euler(0f, half, 0f) * transform.forward;
            Gizmos.DrawLine(eye, eye + left * range);
            Gizmos.DrawLine(eye, eye + right * range);

            const int segments = 16;
            Vector3 prev = eye + left * range;
            for (int i = 1; i <= segments; i++)
            {
                float a = Mathf.Lerp(-half, half, i / (float)segments);
                Vector3 next = eye + Quaternion.Euler(0f, a, 0f) * transform.forward * range;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, hearingRange);

#if UNITY_EDITOR
            if (Application.isPlaying)
                UnityEditor.Handles.Label(eye + Vector3.up * 0.5f, $"Sospecha: {Detection01 * 100f:0}%");
#endif
        }
    }
}
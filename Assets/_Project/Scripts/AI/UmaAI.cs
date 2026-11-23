using UnityEngine;
using Huantar.Core;

namespace Huantar.AI
{
    public class UmaAI : MonoBehaviour
    {
        [SerializeField] float speed = 4.3f;
        [SerializeField] float turnResponsiveness = 3f;
        [SerializeField] float hoverHeight = 1.8f;
        [SerializeField] float weaveAmplitude = 1.2f;
        [SerializeField] float weaveFrequency = 1.2f;
        [SerializeField] float captureDistance = 0.9f;
        [SerializeField] LayerMask groundMask = ~0;

        Transform player;
        Vector3 velocity;
        float seed;
        bool active;

        void OnEnable() => ThreatRegistry.Register(transform);
        void OnDisable() => ThreatRegistry.Unregister(transform);

        public void Activate(Transform target, Vector3 startPosition)
        {
            transform.position = startPosition;
            player = target;
            gameObject.SetActive(true);
            velocity = (target.position - startPosition).normalized * speed;
            seed = Random.value * 10f;
            active = true;
        }

        public void Deactivate()
        {
            active = false;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (!active || player == null) return;

            Vector3 target = player.position + Vector3.up * 1.5f;
            Vector3 desired = (target - transform.position).normalized * speed;
            velocity = Vector3.Lerp(velocity, desired, turnResponsiveness * Time.deltaTime);

            Vector3 pos = transform.position + velocity * Time.deltaTime;

            Vector3 side = Vector3.Cross(Vector3.up, velocity.normalized);
            float weave = Mathf.Sin((Time.time + seed) * weaveFrequency * Mathf.PI * 2f);
            pos += side * weave * weaveAmplitude * Time.deltaTime;

            if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 40f, groundMask, QueryTriggerInteraction.Ignore))
                pos.y = Mathf.Lerp(pos.y, hit.point.y + hoverHeight, 4f * Time.deltaTime);

            transform.position = pos;
            if (velocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(velocity);

            if (Vector3.Distance(transform.position, target) <= captureDistance)
            {
                active = false;
                GameEvents.RaisePlayerCaught(transform);
            }
        }
    }
}
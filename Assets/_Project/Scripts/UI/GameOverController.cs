using System.Collections;
using UnityEngine;
using Huantar.Core;
using Huantar.Player;

namespace Huantar.UI
{
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] CanvasGroup panel;
        [SerializeField] float lookDuration = 0.8f;
        [SerializeField] float fadeDuration = 0.8f;
        [SerializeField] float holdDuration = 2f;

        FirstPersonController player;
        bool running;

        void OnEnable() => GameEvents.PlayerCaught += OnPlayerCaught;
        void OnDisable() => GameEvents.PlayerCaught -= OnPlayerCaught;

        void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
            SetPanel(0f);
        }

        void OnPlayerCaught(Transform byWhom)
        {
            if (!running) StartCoroutine(Sequence(byWhom));
        }

        IEnumerator Sequence(Transform byWhom)
        {
            running = true;
            player.SetInputLock(this, true);

            float t = 0f;
            while (t < lookDuration)
            {
                if (byWhom != null) player.FaceTowards(byWhom.position + Vector3.up * 2.2f);
                t += Time.deltaTime;
                yield return null;
            }

            yield return Fade(0f, 1f);
            yield return new WaitForSeconds(holdDuration);

            CheckpointManager.Instance.Respawn();

            yield return Fade(1f, 0f);
            player.SetInputLock(this, false);
            running = false;
        }

        IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                SetPanel(Mathf.Lerp(from, to, t / fadeDuration));
                yield return null;
            }
            SetPanel(to);
        }

        void SetPanel(float alpha)
        {
            panel.alpha = alpha;
            panel.blocksRaycasts = alpha > 0.01f;
        }
    }
}
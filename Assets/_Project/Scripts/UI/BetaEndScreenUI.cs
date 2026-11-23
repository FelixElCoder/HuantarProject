using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Huantar.Core;
using Huantar.Narrative;
using Huantar.Player;
using Huantar.Telemetry;

namespace Huantar.UI
{
    public class BetaEndScreenUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup panel;
        [SerializeField] TMP_Text summaryLabel;
        [SerializeField] Button btnQuit;
        [SerializeField] float fadeDuration = 2f;

        public static bool IsShowing { get; private set; }

        float startTime;
        FirstPersonController player;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => IsShowing = false;

        void OnEnable() => GameEvents.BetaFinished += Show;
        void OnDisable() => GameEvents.BetaFinished -= Show;

        void Start()
        {
            startTime = Time.realtimeSinceStartup;
            player = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
            btnQuit.onClick.AddListener(Quit);
            SetPanel(0f, false);
        }

        void Show()
        {
            if (!IsShowing) StartCoroutine(Sequence());
        }

        IEnumerator Sequence()
        {
            IsShowing = true;
            player.SetInputLock(this, true);

            float elapsed = Time.realtimeSinceStartup - startTime;
            int minutes = (int)(elapsed / 60f);
            int seconds = (int)(elapsed % 60f);
            FieldNotebook nb = FieldNotebook.Instance;
            string code = PlaytestLogger.Instance != null ? PlaytestLogger.Instance.SessionCode : "—";
            summaryLabel.text =
                $"Tiempo de juego: {minutes:00}:{seconds:00}\n" +
                $"Entradas del cuaderno: {nb.CountDiscoveredAll} / {nb.CountTotalAll}\n" +
                $"Código de sesión: <b>{code}</b>";

            for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                SetPanel(t / fadeDuration, false);
                yield return null;
            }

            SetPanel(1f, true);
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void SetPanel(float alpha, bool interactive)
        {
            panel.alpha = alpha;
            panel.interactable = interactive;
            panel.blocksRaycasts = interactive;
        }

        void Quit()
        {
            PlayerPrefs.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
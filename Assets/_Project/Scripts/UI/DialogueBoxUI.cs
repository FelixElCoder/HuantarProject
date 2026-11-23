using UnityEngine;
using TMPro;
using Huantar.Narrative;

namespace Huantar.UI
{
    public class DialogueBoxUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup panel;
        [SerializeField] TMP_Text speakerLabel;
        [SerializeField] TMP_Text bodyLabel;
        [SerializeField] GameObject continueIndicator;

        DialogueSystem system;
        bool currentBlocking;

        void Start()
        {
            system = DialogueSystem.Instance;
            system.LineStarted += OnLineStarted;
            system.VisibleCharsChanged += OnVisibleChars;
            system.LineCompleted += OnLineCompleted;
            system.DialogueEnded += Hide;
            Hide();
        }

        void OnDestroy()
        {
            if (system == null) return;
            system.LineStarted -= OnLineStarted;
            system.VisibleCharsChanged -= OnVisibleChars;
            system.LineCompleted -= OnLineCompleted;
            system.DialogueEnded -= Hide;
        }

        void OnLineStarted(DialogueLine line, bool blocking)
        {
            currentBlocking = blocking;
            panel.alpha = 1f;

            bool hasSpeaker = !string.IsNullOrEmpty(line.speaker);
            speakerLabel.gameObject.SetActive(hasSpeaker);
            speakerLabel.text = line.speaker;

            bodyLabel.text = line.text;
            bodyLabel.fontStyle = hasSpeaker ? FontStyles.Normal : FontStyles.Italic;
            bodyLabel.maxVisibleCharacters = 0;

            if (continueIndicator != null) continueIndicator.SetActive(false);
        }

        void OnVisibleChars(int count) => bodyLabel.maxVisibleCharacters = count;

        void OnLineCompleted()
        {
            if (continueIndicator != null) continueIndicator.SetActive(currentBlocking);
        }

        void Hide()
        {
            panel.alpha = 0f;
            if (continueIndicator != null) continueIndicator.SetActive(false);
        }
    }
}
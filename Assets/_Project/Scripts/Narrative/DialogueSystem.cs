using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Huantar.Player;

namespace Huantar.Narrative
{
    public class DialogueSystem : MonoBehaviour
    {
        public static DialogueSystem Instance { get; private set; }

        [SerializeField] float charsPerSecond = 40f;
        [SerializeField] float ambientBaseHold = 1.5f;
        [SerializeField] float ambientHoldPerChar = 0.04f;

        public event Action<DialogueLine, bool> LineStarted;
        public event Action<int> VisibleCharsChanged;
        public event Action LineCompleted;
        public event Action DialogueEnded;

        public bool IsPlaying { get; private set; }

        readonly Queue<(DialogueData data, Action onComplete)> queue = new();
        FirstPersonController player;
        int startFrame;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.GetComponent<FirstPersonController>();
        }

        public void Play(DialogueData data, Action onComplete = null)
        {
            if (data == null || data.lines == null || data.lines.Length == 0) return;
            queue.Enqueue((data, onComplete));
            if (!IsPlaying) StartCoroutine(RunQueue());
        }

        IEnumerator RunQueue()
        {
            IsPlaying = true;

            while (queue.Count > 0)
            {
                var (data, onComplete) = queue.Dequeue();
                if (data.blocking && player != null) player.InputEnabled = false;

                foreach (DialogueLine line in data.lines)
                    yield return PlayLine(line, data.blocking);

                if (data.blocking && player != null) player.InputEnabled = true;
                onComplete?.Invoke();
            }

            IsPlaying = false;
            DialogueEnded?.Invoke();
        }

        IEnumerator PlayLine(DialogueLine line, bool blocking)
        {
            startFrame = Time.frameCount;
            LineStarted?.Invoke(line, blocking);

            int total = line.text.Length;
            float visible = 0f;
            while (visible < total)
            {
                if (AdvancePressed()) break;
                visible += charsPerSecond * Time.deltaTime;
                VisibleCharsChanged?.Invoke(Mathf.Min(total, (int)visible));
                yield return null;
            }

            VisibleCharsChanged?.Invoke(total);
            LineCompleted?.Invoke();
            yield return null;

            if (blocking)
            {
                while (!AdvancePressed()) yield return null;
            }
            else
            {
                float hold = ambientBaseHold + total * ambientHoldPerChar;
                yield return new WaitForSeconds(hold);
            }
        }

        bool AdvancePressed()
        {
            if (Time.frameCount == startFrame) return false;

            var kb = Keyboard.current;
            var mouse = Mouse.current;
            return (kb != null && (kb.eKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame))
                || (mouse != null && mouse.leftButton.wasPressedThisFrame);
        }
    }
}
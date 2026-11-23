using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Huantar.Narrative;

namespace Huantar.UI
{
    public class NotebookToastUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup toast;
        [SerializeField] TMP_Text label;
        [SerializeField] float fade = 0.4f;
        [SerializeField] float hold = 3f;

        readonly Queue<string> pending = new();
        bool showing;

        void Start()
        {
            toast.alpha = 0f;
            FieldNotebook.Instance.EntryAdded += OnEntryAdded;
        }

        void OnDestroy()
        {
            if (FieldNotebook.Instance != null)
                FieldNotebook.Instance.EntryAdded -= OnEntryAdded;
        }

        void OnEntryAdded(NotebookEntryData e)
        {
            pending.Enqueue($"Nueva entrada en el cuaderno:\n<b>{e.title}</b>   [Tab]");
            if (!showing) StartCoroutine(ShowQueue());
        }

        IEnumerator ShowQueue()
        {
            showing = true;
            while (pending.Count > 0)
            {
                label.text = pending.Dequeue();
                yield return Fade(0f, 1f);
                yield return new WaitForSecondsRealtime(hold);
                yield return Fade(1f, 0f);
            }
            showing = false;
        }

        IEnumerator Fade(float from, float to)
        {
            for (float t = 0f; t < fade; t += Time.unscaledDeltaTime)
            {
                toast.alpha = Mathf.Lerp(from, to, t / fade);
                yield return null;
            }
            toast.alpha = to;
        }
    }
}
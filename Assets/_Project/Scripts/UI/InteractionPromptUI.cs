using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Huantar.Player;

namespace Huantar.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] InteractionSystem interaction;
        [SerializeField] Image dot;
        [SerializeField] TMP_Text label;

        [SerializeField] Color idleColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] Color activeColor = Color.white;
        [SerializeField] float idleSize = 6f;
        [SerializeField] float activeSize = 12f;

        void Update()
        {
            bool has = interaction != null && interaction.Current != null;

            dot.color = has ? activeColor : idleColor;
            dot.rectTransform.sizeDelta = Vector2.one * (has ? activeSize : idleSize);
            label.text = has ? $"[E] {interaction.Current.Prompt}" : "";
        }
    }
}
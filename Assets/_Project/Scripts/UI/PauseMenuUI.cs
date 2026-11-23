using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Huantar.Player;

namespace Huantar.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup panel;
        [SerializeField] NotebookUI notebook;

        [Header("Botones")]
        [SerializeField] Button btnContinue;
        [SerializeField] Button btnNotebook;
        [SerializeField] Button btnQuit;

        [Header("Opciones")]
        [SerializeField] Slider sensitivitySlider;
        [SerializeField] Slider volumeSlider;

        const string KeySensitivity = "opt_sensitivity";
        const string KeyVolume = "opt_volume";

        public bool IsOpen { get; private set; }

        FirstPersonController player;

        void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();

            btnContinue.onClick.AddListener(Close);
            btnNotebook.onClick.AddListener(OpenNotebook);
            btnQuit.onClick.AddListener(Quit);

            sensitivitySlider.minValue = 0.02f;
            sensitivitySlider.maxValue = 0.4f;
            sensitivitySlider.value = PlayerPrefs.GetFloat(KeySensitivity, player.MouseSensitivity);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivity);

            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = PlayerPrefs.GetFloat(KeyVolume, 1f);
            volumeSlider.onValueChanged.AddListener(OnVolume);

            OnSensitivity(sensitivitySlider.value);
            OnVolume(volumeSlider.value);

            SetVisible(false);
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null || !kb.escapeKey.wasPressedThisFrame) return;

            if (BetaEndScreenUI.IsShowing) return;

            if (notebook != null && notebook.IsOpen) { notebook.Close(); return; }

            if (IsOpen) Close();
            else Open();
        }

        public void Open()
        {
            IsOpen = true;
            SetVisible(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
            player.SetInputLock(this, true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            IsOpen = false;
            SetVisible(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
            player.SetInputLock(this, false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            EventSystem.current?.SetSelectedGameObject(null);
        }

        void OpenNotebook()
        {
            Close();
            notebook.Open();
        }

        void OnSensitivity(float value)
        {
            player.MouseSensitivity = value;
            PlayerPrefs.SetFloat(KeySensitivity, value);
        }

        void OnVolume(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(KeyVolume, value);
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

        void SetVisible(bool v)
        {
            panel.alpha = v ? 1f : 0f;
            panel.interactable = v;
            panel.blocksRaycasts = v;
        }
    }
}
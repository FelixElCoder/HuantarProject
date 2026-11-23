using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Huantar.Narrative;
using Huantar.Player;

namespace Huantar.UI
{
    public class NotebookUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] CanvasGroup panel;

        [Header("Pestañas")]
        [SerializeField] Button tabChavin;
        [SerializeField] Button tabColonia;
        [SerializeField] Button tabCreencias;

        [Header("Lista")]
        [SerializeField] Transform listContainer;
        [SerializeField] Button entryButtonPrefab;
        [SerializeField] TMP_Text emptyLabel;

        [Header("Detalle")]
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text notesLabel;
        [SerializeField] GameObject factBox;
        [SerializeField] TMP_Text factLabel;
        [SerializeField] TMP_Text sourceLabel;

        public bool IsOpen { get; private set; }

        FirstPersonController player;
        NotebookCategory current = NotebookCategory.Chavin;
        readonly List<Button> spawned = new();

        void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();

            tabChavin.onClick.AddListener(() => ShowCategory(NotebookCategory.Chavin));
            tabColonia.onClick.AddListener(() => ShowCategory(NotebookCategory.Colonia));
            tabCreencias.onClick.AddListener(() => ShowCategory(NotebookCategory.Creencias));

            Close();
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.tabKey.wasPressedThisFrame)
            {
                if (IsOpen) Close();
                else if (player.InputEnabled) Open();
            }
            else if (IsOpen && kb.escapeKey.wasPressedThisFrame)
            {
                Close();
            }
        }

        public void Open()
        {
            IsOpen = true;
            SetVisible(true);
            Time.timeScale = 0f;
            player.SetInputLock(this, true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowCategory(current);
        }

        public void Close()
        {
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
            IsOpen = false;
            SetVisible(false);
            Time.timeScale = 1f;
            if (player != null) player.SetInputLock(this, false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void SetVisible(bool v)
        {
            panel.alpha = v ? 1f : 0f;
            panel.interactable = v;
            panel.blocksRaycasts = v;
        }

        void ShowCategory(NotebookCategory category)
        {
            current = category;
            UpdateTabLabels();

            foreach (Button b in spawned) Destroy(b.gameObject);
            spawned.Clear();

            NotebookEntryData first = null;
            foreach (NotebookEntryData entry in FieldNotebook.Instance.GetDiscovered(category))
            {
                Button b = Instantiate(entryButtonPrefab, listContainer);
                b.GetComponentInChildren<TMP_Text>().text = entry.title;
                NotebookEntryData captured = entry;
                b.onClick.AddListener(() => ShowEntry(captured));
                spawned.Add(b);
                if (first == null) first = entry;
            }

            emptyLabel.gameObject.SetActive(first == null);
            ShowEntry(first);
        }

        void ShowEntry(NotebookEntryData e)
        {
            if (e == null)
            {
                titleLabel.text = "";
                notesLabel.text = "";
                factBox.SetActive(false);
                return;
            }

            titleLabel.text = e.title;
            notesLabel.text = e.fieldNotes;
            factBox.SetActive(e.hasRealFact);
            factLabel.text = e.realFact;
            sourceLabel.text = string.IsNullOrEmpty(e.source) ? "" : $"Fuente: {e.source}";
        }

        void UpdateTabLabels()
        {
            SetTabText(tabChavin, "Chavín", NotebookCategory.Chavin);
            SetTabText(tabColonia, "Colonia", NotebookCategory.Colonia);
            SetTabText(tabCreencias, "Creencias", NotebookCategory.Creencias);
        }

        void SetTabText(Button tab, string name, NotebookCategory cat)
        {
            FieldNotebook nb = FieldNotebook.Instance;
            string text = $"{name}  {nb.CountDiscovered(cat)}/{nb.CountTotal(cat)}";
            if (cat == current) text = $"<b>{text}</b>";
            tab.GetComponentInChildren<TMP_Text>().text = text;
        }
    }
}
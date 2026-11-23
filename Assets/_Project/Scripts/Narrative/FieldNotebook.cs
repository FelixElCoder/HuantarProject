using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Huantar.Narrative
{
    public class FieldNotebook : MonoBehaviour
    {
        public static FieldNotebook Instance { get; private set; }

        [Tooltip("Todas las entradas que existen en el juego (para calcular el progreso).")]
        [SerializeField] NotebookEntryData[] catalog;

        public event Action<NotebookEntryData> EntryAdded;

        readonly List<NotebookEntryData> discovered = new();

        public int CountDiscoveredAll => discovered.Count;
        public int CountTotalAll => catalog == null ? 0 : catalog.Count(e => e != null);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool IsDiscovered(NotebookEntryData entry) => discovered.Contains(entry);

        public bool Register(NotebookEntryData entry)
        {
            if (entry == null || discovered.Contains(entry)) return false;
            discovered.Add(entry);
            EntryAdded?.Invoke(entry);
            Debug.Log($"[Cuaderno] Nueva entrada: {entry.title}");
            return true;
        }

        public IEnumerable<NotebookEntryData> GetDiscovered(NotebookCategory category) =>
            discovered.Where(e => e.category == category);

        public int CountDiscovered(NotebookCategory category) =>
            discovered.Count(e => e.category == category);

        public int CountTotal(NotebookCategory category) =>
            catalog == null ? 0 : catalog.Count(e => e != null && e.category == category);
    }
}
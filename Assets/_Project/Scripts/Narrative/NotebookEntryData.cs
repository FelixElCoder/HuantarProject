using UnityEngine;

namespace Huantar.Narrative
{
    public enum NotebookCategory { Chavin, Colonia, Creencias }

    [CreateAssetMenu(fileName = "NB_Nueva", menuName = "Huántar/Entrada de cuaderno")]
    public class NotebookEntryData : ScriptableObject
    {
        public string title;
        public NotebookCategory category;

        [Header("Notas del investigador (voz de época)")]
        [TextArea(3, 8)] public string fieldNotes;

        [Header("Dato real (verificado)")]
        public bool hasRealFact = true;
        [TextArea(3, 8)] public string realFact;
        public string source;
    }
}
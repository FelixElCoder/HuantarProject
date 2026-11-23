using UnityEngine;

namespace Huantar.Narrative
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2, 5)] public string text;
    }

    [CreateAssetMenu(fileName = "DLG_Nuevo", menuName = "Huántar/Diálogo")]
    public class DialogueData : ScriptableObject
    {
        public bool blocking = false;
        public DialogueLine[] lines;
    }
}
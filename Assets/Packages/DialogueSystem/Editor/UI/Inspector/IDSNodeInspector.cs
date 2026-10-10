using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem.UI.Inspector
{
    public interface IDSNodeInspector
    {
        void Create();
        void LoadFields(DSDialogueNodeData data);
    }
}

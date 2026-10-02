using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem.UI.Inspector
{
    public interface IDSNodeInspector 
    {
        public void Create();
        public void LoadFields(DSDialogueNodeData data);
    }
}

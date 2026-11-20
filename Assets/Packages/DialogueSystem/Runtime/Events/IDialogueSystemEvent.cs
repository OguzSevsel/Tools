using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem
{
    public interface IDialogueSystemEvent
    {
        public DSNodeData Node { get; }
    }
}

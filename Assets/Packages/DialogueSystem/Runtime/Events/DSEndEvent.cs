using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem
{
    public class DSEndEvent : IDialogueSystemEvent
    {
        public DSNodeData Node { get; }

        internal DSEndEvent(DSNodeData node)
        {
            this.Node = node;
        }
    }
}
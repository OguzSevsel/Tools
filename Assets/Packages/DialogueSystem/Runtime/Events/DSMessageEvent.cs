using Tools.DialogueSystem.Data;
using UnityEngine;

namespace Tools.DialogueSystem
{
    public class DSMessageEvent : IDialogueSystemEvent
    {
        public DSNodeData Node { get; }
        private readonly DSDialogueRunner.DSDialogueInternal runner;

        internal DSMessageEvent(DSDialogueRunner.DSDialogueInternal runner, DSNodeData node)
        {
            this.runner = runner;
            this.Node = node;
        }

        public void Advance() => runner.AdvanceToChoices(Node.Guid);
    }
}
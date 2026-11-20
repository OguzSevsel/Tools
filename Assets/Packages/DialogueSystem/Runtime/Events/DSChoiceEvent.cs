using System.Collections.Generic;
using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem
{
    public class DSChoiceEvent : IDialogueSystemEvent
    {
        public List<DSChoice> Choices { get; private set; }
        public DSNodeData Node { get; }
        private readonly DSDialogueRunner.DSDialogueInternal runner;

        internal DSChoiceEvent(DSDialogueRunner.DSDialogueInternal runner, DSNodeData parentNode, List<DSChoice> choices)
        {
            this.runner = runner;
            this.Node = parentNode;
            this.Choices = choices;
        }

        public void Advance(DSChoice selectedChoice) => runner.NextDialogue(selectedChoice.TargetNodeId);
    }
}
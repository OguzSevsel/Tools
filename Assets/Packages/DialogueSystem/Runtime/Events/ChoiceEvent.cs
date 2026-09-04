using System.Collections.Generic;

namespace Tools.DialogueSystem
{
    public class ChoiceEvent : IDialogueEvent
    {
        public string DialogueId { get; private set; }
        public string ActorName { get; private set; }
        public List<DSChoice> Choices { get; private set; }

        private readonly DialogueRunner.DialogueInternal runner;

        internal ChoiceEvent(DialogueRunner.DialogueInternal runner, string dialogueId, string actorName, List<DSChoice> choices)
        {
            this.runner = runner;
            this.DialogueId = dialogueId;
            this.ActorName = actorName;
            this.Choices = choices;
        }

        public void Advance(DSChoice selectedChoice)
        {
            runner.NextDialogue(selectedChoice.TargetNodeId);
        }
    }
}
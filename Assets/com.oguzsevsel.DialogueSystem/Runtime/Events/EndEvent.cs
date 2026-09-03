namespace Tools.DialogueSystem
{
    public class EndEvent : IDialogueEvent
    {
        public string DialogueId { get; private set; }
        public string ActorName { get; private set; }

        internal EndEvent(string dialogueId, string actorName)
        {
            this.DialogueId = dialogueId;
            this.ActorName = actorName;
        }
    }
}
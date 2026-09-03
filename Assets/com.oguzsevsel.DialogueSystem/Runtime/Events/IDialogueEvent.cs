namespace Tools.DialogueSystem
{
	public interface IDialogueEvent
	{
        public string DialogueId { get; }
        public string ActorName { get; }
	}
}

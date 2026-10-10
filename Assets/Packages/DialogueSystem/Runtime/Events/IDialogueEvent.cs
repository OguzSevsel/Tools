namespace Tools.DialogueSystem
{
    public interface IDialogueEvent
    {
        string DialogueId { get; }
        string ActorName { get; }
    }
}

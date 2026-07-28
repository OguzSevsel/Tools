using UnityEngine;

namespace Tools.DialogueSystem
{
    public class MessageEvent : IDialogueEvent
    {
        public AudioClip AudioClip { get; private set; }
        public Sprite ActorSprite { get; private set; }
        public string DialogueId { get; private set; }
        public string ActorName { get; private set; }
        public string DialogueText { get; private set; }

        private readonly DialogueRunner.DialogueInternal runner;

        internal MessageEvent(DialogueRunner.DialogueInternal runner, string dialogueId, string actorName, string dialogueText, Sprite actorSprite, AudioClip audioClip)
        {
            this.runner = runner;
            this.DialogueId = dialogueId;
            this.ActorName = actorName;
            this.DialogueText = dialogueText;
            this.ActorSprite = actorSprite;
            this.AudioClip = audioClip;
        }

        public void Advance()
        {
            runner.AdvanceToChoices(DialogueId);
        }
    }
}
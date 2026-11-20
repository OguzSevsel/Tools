using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem
{
    public class DSDialogueRunner
    {
        internal class DSDialogueInternal
        {
            private event Action<IDialogueSystemEvent> OnDialogueEvent;
            private DSConversation conversation;
            private DSDatabase database;
            private DSNodeData currentNode;

            public DSDialogueInternal(System.Action<IDialogueSystemEvent> callback, DSConversation conversation, DSDatabase database)
            {
                OnDialogueEvent = callback;
                this.database = database;
                this.conversation = conversation;
            }

            public void AdvanceToChoices(string nodeId)
            {
                currentNode = conversation.Get(nodeId);

                var choiceNodes = this.conversation.GetDialogueChoices(currentNode.Guid);
                var dsChoices = new List<DSChoice>();

                foreach (var choice in choiceNodes)
                {
                    DSChoice newChoice = new DSChoice(database.Get<DSDialogueText>(choice.DialogueId).Text, choice.Guid);

                    dsChoices.Add(newChoice);
                }

                OnDialogueEvent?.Invoke(new DSChoiceEvent(this, currentNode, dsChoices));
            }

            public void NextDialogue(string selectedId)
            {
                currentNode = null;
                currentNode = conversation.Get(selectedId);

                if (currentNode == null || currentNode.IsEndNode)
                {
                    OnDialogueEvent?.Invoke(new DSEndEvent(conversation.Get(selectedId)));
                    return;
                }

                OnDialogueEvent?.Invoke(new DSMessageEvent(this, currentNode));
            }
        }

        private DSNodeData currentNode;
        private DSConversation conversation;
        private DSDatabase database;
        private DSDialogueInternal _internal;

        public event Action<IDialogueSystemEvent> OnDialogueEvent;

        public DSDialogueRunner(DSConversation conversation, DSDatabase database)
        {
            this.conversation = conversation;
            this.database = database;
            _internal = new DSDialogueInternal(OnDialogueEvent, conversation, database);
        }

        public void StartDialogue()
        {
            currentNode = conversation.GetStartNode();

            _internal = new DSDialogueInternal(OnDialogueEvent, conversation, database);

            OnDialogueEvent?.Invoke(new DSMessageEvent(_internal, currentNode));
        }
    }
}

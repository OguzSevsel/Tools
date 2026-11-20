using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
using UnityEngine;

namespace Tools.DialogueSystem
{
    public class DSDialogueManager : MonoBehaviour
    {
        public static DSDialogueManager Instance { get; private set; }
        [SerializeField] private DSDatabase database;
        [SerializeField] private DialogueElement panel;

        private DSDialogueRunner runner;
        private DSConversation currentConversation;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartDialogue("Deneme");
        }

        public void StartDialogue(string name)
        {
            currentConversation = null;
            currentConversation = database.Conversations.FirstOrDefault();
            if (currentConversation == null)
                return;
            runner = new DSDialogueRunner(currentConversation, database);
            runner.OnDialogueEvent += DialogueEventHandler;
            runner.StartDialogue();
        }

        public void StartDialogue(DSConversation conversation)
        {
            currentConversation = null;
            currentConversation = database.Get<DSConversation>(conversation.Guid);
            if (currentConversation == null)
                return;
            runner = new DSDialogueRunner(currentConversation, database);
            runner.OnDialogueEvent += DialogueEventHandler;
            runner.StartDialogue();
        }

        private void DialogueEventHandler(IDialogueSystemEvent e)
        {
            switch (e)
            {
                case DSMessageEvent:
                    DSMessageEvent messageEvent = (DSMessageEvent)e;

                    DSActor actor = database.Get<DSActor>(messageEvent.Node.ActorId);
                    DSActor conversant = database.Get<DSActor>(messageEvent.Node.ConversantId);
                    DSAudioClip audioClip = database.Get<DSAudioClip>(messageEvent.Node.AudioClipId);
                    DSDialogueText dialogue = database.Get<DSDialogueText>(messageEvent.Node.DialogueId);

                    panel.ShowDialogText(dialogue.Text, actor.Name, actor.Sprite, (e) => messageEvent.Advance());
                    break;
                case DSChoiceEvent:
                    DSChoiceEvent choiceEvent = (DSChoiceEvent)e;
                    panel.ShowChoicesText(choiceEvent.Choices, (evt) => choiceEvent.Advance(evt));
                    break;
                case DSEndEvent:
                    DSEndEvent endEvent = (DSEndEvent)e;
                    runner.OnDialogueEvent -= DialogueEventHandler;
                    StartDialogue(endEvent.Node.Guid);
                    if (currentConversation == null)
                    {
                        panel.EndDialog();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

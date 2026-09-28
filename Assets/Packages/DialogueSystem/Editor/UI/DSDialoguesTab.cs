using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialoguesTab : VisualElement
    {
        private Button button_CreateNewDialogue;
        private Dictionary<DSDialogueText, DSDialogueElement> dialogues;
        private ScrollView scrollView_Dialogue;
        private VisualElement grid_Dialogue;
        private VisualElement toolbar;

        public DSDialoguesTab()
        {
            dialogues = new Dictionary<DSDialogueText, DSDialogueElement>();
            CreateUIElements();
            RegisterEvents();
            

        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (var dialogue in DSDatabaseManager.Current.DialogueTexts)
            {
                CreateDialogueUIElement(dialogue);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (var dialogue in DSDatabaseManager.Current.DialogueTexts)
            {
                if (dialogues.ContainsKey(dialogue))
                {
                    DSDialogueElement textElement = dialogues[dialogue];
                    if (grid_Dialogue.Contains(textElement))
                    {
                        grid_Dialogue.Remove(textElement);
                        dialogues.Remove(dialogue);
                    }
                }
            }
        }

        private void OnDetached(DetachFromPanelEvent evt)
        {
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
        }

        private void OnCreateNewDialogueButtonClicked()
        {
            CreateDialogue();
        }

        private void OnDialogueDeleted(DSDialogueText text)
        {
            DeleteDialogueUIElement(text);
            DeleteDialogue(text);
        }

        private void DeleteDialogueUIElement(DSDialogueText text)
        {
            if (dialogues.ContainsKey(text))
            {
                DSDialogueElement textElement = dialogues[text];
                this.grid_Dialogue.Remove(textElement);
                dialogues.Remove(text);
            }
        }

        private void DeleteDialogue(DSDialogueText text)
        {
            DSDatabaseManager.Current.Unregister(text);
        }

        private void CreateDialogue()
        {
            DSDialogueText dialogueText = new DSDialogueText("Default Text", "");
            CreateDialogueUIElement(dialogueText);
            DSDatabaseManager.Current.Register(dialogueText);
        }

        private void CreateDialogueUIElement(DSDialogueText dialogueText)
        {
            DSDialogueElement textElement = new DSDialogueElement(dialogueText);
            textElement.OnDialogueDelete += OnDialogueDeleted;
            this.grid_Dialogue.Add(textElement);
            this.dialogues.Add(dialogueText, textElement);
        }

        private void CreateUIElements()
        {
            this.style.backgroundColor = Color.snow;
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;

            scrollView_Dialogue = new ScrollView
            {
                mode = ScrollViewMode.Vertical,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            grid_Dialogue = new VisualElement();
            grid_Dialogue.style.flexDirection = FlexDirection.Row;
            grid_Dialogue.style.flexWrap = Wrap.Wrap;

            scrollView_Dialogue.Add(grid_Dialogue);

            button_CreateNewDialogue = DSElementUtility.CreateButton("Create New Dialogue", OnCreateNewDialogueButtonClicked);
            toolbar = new VisualElement();

            this.toolbar.Insert(0, button_CreateNewDialogue);
            this.contentContainer.Insert(0, toolbar);
            this.contentContainer.Add(scrollView_Dialogue);
        }

        private void RegisterEvents()
        {
            RegisterCallback<DetachFromPanelEvent>(OnDetached);
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
        }
    }
}
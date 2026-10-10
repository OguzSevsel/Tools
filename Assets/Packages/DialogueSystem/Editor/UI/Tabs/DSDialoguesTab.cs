using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialoguesTab : Tab
    {
        private Dictionary<DSDialogueText, VisualElement> dialogues;

        private VisualTreeAsset cardAsset;

        private VisualElement dialogueCardContainer;
        private ScrollView dialogueScrollView;
        private Button createDialogueButton;

        public DSDialoguesTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            dialogues = new Dictionary<DSDialogueText, VisualElement>();
            tabAsset.CloneTree(this);
            label = "Dialogues";
            this.cardAsset = cardAsset;

            createDialogueButton = this.Q<Button>("CreateDialogueButton");
            dialogueCardContainer = this.Q<VisualElement>("DialogueCardContainer");
            dialogueScrollView = this.Q<ScrollView>("DialogueScrollView");
            createDialogueButton.clicked += OnCreateDialogueButtonClicked;
            RegisterEvents();
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (KeyValuePair<DSDialogueText, VisualElement> cardPair in dialogues)
            {
                VisualElement card = cardPair.Value;
                DSDialogueText text = cardPair.Key;

                dialogueCardContainer.Remove(card);
            }

            dialogues.Clear();
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSDialogueText text in database.DialogueTexts)
            {
                VisualElement card = CreateCard(cardAsset, text);

                AddCard(dialogueCardContainer, dialogues, text, card);
            }
        }

        private void OnCreateDialogueButtonClicked()
        {
            DSDialogueText textObject = new("New Dialogue", "Dialogue Description", "Dialogue Text");
            VisualElement cardElement = CreateCard(cardAsset, textObject);
            AddCard(dialogueCardContainer, dialogues, textObject, cardElement);
            DSDatabaseManager.Current.Register(textObject);
        }

        #region Utils

        private void RegisterEvents()
        {
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
            Debug.Log("Editor Window Closed");
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, DSDialogueText textObject)
        {
            VisualElement cardElement = new();
            cardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow();
                }
            });
            cardElement.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow(true);
                }
            });

            cardElement.dataSource = textObject;

            return cardElement;
        }

        private void AddCard(VisualElement container, Dictionary<DSDialogueText, VisualElement> dialogues, DSDialogueText textObject, VisualElement cardElement)
        {
            container.Add(cardElement);
            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                container.Remove(cardElement);
                dialogues.Remove(textObject);
                DSDatabaseManager.Current.Unregister(textObject);
            };

            dialogues.Add(textObject, cardElement);
        }

        #endregion
    }
}
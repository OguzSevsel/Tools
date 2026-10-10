using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSConversationsTab : Tab
    {
        private VisualTreeAsset cardAsset;
        private VisualElement container;
        private ScrollView scrollView;
        private Button createButton;
        private Dictionary<DSConversation, VisualElement> conversations;


        public DSConversationsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            conversations = new Dictionary<DSConversation, VisualElement>();
            tabAsset.CloneTree(this);
            label = "Conversations";

            this.cardAsset = cardAsset;

            container = this.Q<VisualElement>("ConversationsCardContainer");
            scrollView = this.Q<ScrollView>("ConversationsScrollView");
            createButton = this.Q<Button>("ConversationsCreateNewButton");
            createButton.clicked += OnCreateButtonClicked;

            RegisterEvents();
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSConversation conversation in database.Conversations)
            {
                VisualElement card = CreateCard(cardAsset, conversation);
                AddCard(container, conversations, card, conversation);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (KeyValuePair<DSConversation, VisualElement> cardPair in conversations)
            {
                VisualElement card = cardPair.Value;
                _ = cardPair.Key;

                container.Remove(card);
            }

            conversations.Clear();
        }

        private void OnCreateButtonClicked()
        {
            DSConversation newConversation = new("New Conversation", "Conversation Description");
            VisualElement cardElement = CreateCard(cardAsset, newConversation);
            AddCard(container, conversations, cardElement, newConversation);
            _ = DSDatabaseManager.Current.Register(newConversation);
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
        }

        private void AddCard(VisualElement container, Dictionary<DSConversation, VisualElement> conversations, VisualElement cardElement, DSConversation conversation)
        {
            container.Add(cardElement);

            Label nodeCountLabel = cardElement.Q<Label>("NodeCountLabel");
            conversation.OnNewNodeCreated += () =>
            {
                nodeCountLabel.text = $"{conversation.NodeCount} Nodes";
            };

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                container.Remove(cardElement);
                _ = conversations.Remove(conversation);
                DSDatabaseManager.Current.Unregister(conversation);
            };
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, DSConversation conversation)
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

            cardElement.dataSource = conversation;
            return cardElement;
        }

        #endregion
    }
}
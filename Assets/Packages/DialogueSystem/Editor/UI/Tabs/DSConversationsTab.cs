using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
        public static event Action<DSConversation> OnConversationOpened;

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

        private void AddConversations(DSDatabase database)
        {
            
            foreach (DSConversation conversation in database.Conversations)
            {
                VisualElement card = CreateCard(cardAsset, conversation);
                AddCard(container, conversations, card, conversation);
            }
        }

        private void RemoveConversations()
        {
            container.Clear();
            conversations.Clear();
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            AddConversations(database);
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            RemoveConversations();
        }

        private void OnCreateButtonClicked()
        {
            DSConversation newConversation = new("New Conversation", "Conversation Description");
            VisualElement cardElement = CreateCard(cardAsset, newConversation);
            AddCard(container, conversations, cardElement, newConversation);
            DSDatabaseManager.Instance.Current.Register(newConversation);
        }

        #region Utils

        private void RegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseOpened += OnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed += OnDatabaseClosed;
            DSGraphView.OnNodeCreated += HandleOnNodeCreated;
            DSGraphView.OnNodeDeleted += HandleOnNodeDeleted;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void HandleOnNodeCreated(DSConversation conversation)
        {
            var conversationCard = conversations[conversation];
            Label nodeCountLabel = conversationCard.Q<Label>("NodeCountLabel");
            nodeCountLabel.text = $"{conversation.NodeCount} Nodes";
        }

        private void UnRegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed -= OnDatabaseClosed;
            DSGraphView.OnNodeCreated -= HandleOnNodeCreated;
            DSGraphView.OnNodeDeleted -= HandleOnNodeDeleted;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void HandleOnNodeDeleted(DSConversation conversation)
        {
            var conversationCard = conversations[conversation];
            Label nodeCountLabel = conversationCard.Q<Label>("NodeCountLabel");
            nodeCountLabel.text = $"{conversation.NodeCount} Nodes";
        }

        private void AddCard(VisualElement container, Dictionary<DSConversation, VisualElement> conversations, VisualElement cardElement, DSConversation conversation)
        {
            container.Add(cardElement);

            Label nodeCountLabel = cardElement.Q<Label>("NodeCountLabel");

            nodeCountLabel.text = $"{conversation.NodeCount} Nodes";

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                container.Remove(cardElement);
                conversations.Remove(conversation);
                DSDatabaseManager.Instance.Current.Unregister(conversation);
            };

            Button openButton = cardElement.Q<Button>("OpenButton");
            openButton.clicked += () =>
            {
                OnConversationOpened?.Invoke(conversation);
            };

            conversations.Add(conversation, cardElement);
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
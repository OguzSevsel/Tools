using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : Tab
    {
        private ScrollView actorScrollView;
        private VisualElement actorContainer;
        private VisualTreeAsset cardAsset;
        private Button createActorButton;
        public Dictionary<DSActor, VisualElement> Actors { get; set; }

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";
            Actors = new Dictionary<DSActor, VisualElement>();
            this.cardAsset = cardAsset;

            actorScrollView = this.Q<ScrollView>("ActorsScrollView");
            actorContainer = this.Q<VisualElement>("ActorsCardContainer");
            createActorButton = this.Q<Button>("CreateActorButton");
            createActorButton.clicked += HandleOnCreateActorButtonClicked;

            RegisterEvents();
        }

        #region INITIALIZATION

        private void AddCard(VisualElement container, Dictionary<DSActor, VisualElement> actors, DSActor actor, VisualElement cardElement)
        {
            container.Add(cardElement);
            actors.Add(actor, cardElement);

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                container.Remove(cardElement);
                actors.Remove(actor);
                DSDatabaseManager.Instance.Current.Unregister(actor);
            };

            Image actorImage = cardElement.Q<Image>("ActorImage");
            ObjectField actorSpriteField = cardElement.Q<ObjectField>("SpriteField");
            actorSpriteField.RegisterValueChangedCallback<Object>((evt) =>
            {
                actorImage.sprite = evt.newValue as Sprite;
            });
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, DSActor actor)
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
            cardElement.dataSource = actor;

            return cardElement;
        }

        #endregion

        #region EVENTS

        private void UnRegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseClosed -= HandleOnDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened -= HandleOnDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void RegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseClosed += HandleOnDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened += HandleOnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        #endregion

        #region EVENT HANDLERS

        private void HandleOnDatabaseOpened(DSDatabase database)
        {
            foreach (DSActor actor in database.Actors)
            {
                VisualElement card = CreateCard(cardAsset, actor);

                AddCard(actorContainer, Actors, actor, card);
            }
        }

        private void HandleOnDatabaseClosed(DSDatabase database)
        {
            actorContainer.Clear();
            Actors.Clear();
        }

        private void HandleOnCreateActorButtonClicked()
        {
            DSActor actor = new("New Actor", "Actor Description", "Actor Background", default);
            VisualElement cardElement = CreateCard(cardAsset, actor);
            AddCard(actorContainer, Actors, actor, cardElement);
            DSDatabaseManager.Instance.Current.Register(actor);
        }

        #endregion
    }
}

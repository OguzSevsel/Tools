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
            createActorButton.clicked += OnCreateActorButtonClicked;

            RegisterEvents();
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSActor actor in database.Actors)
            {
                VisualElement card = CreateCard(cardAsset, actor);

                AddCard(actorContainer, Actors, actor, card);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (KeyValuePair<DSActor, VisualElement> cardPair in Actors)
            {
                VisualElement card = cardPair.Value;
                DSActor text = cardPair.Key;

                actorContainer.Remove(card);
            }

            Actors.Clear();
        }

        private void OnCreateActorButtonClicked()
        {
            DSActor actor = new("New Actor", "Actor Description", "Actor Background", default);
            VisualElement cardElement = CreateCard(cardAsset, actor);
            AddCard(actorContainer, Actors, actor, cardElement);
            DSDatabaseManager.Current.Register(actor);
        }

        #region Utils

        private void UnRegisterEvents()
        {
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void RegisterEvents()
        {
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void AddCard(VisualElement container, Dictionary<DSActor, VisualElement> actors, DSActor actor, VisualElement cardElement)
        {
            container.Add(cardElement);
            actors.Add(actor, cardElement);

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                container.Remove(cardElement);
                actors.Remove(actor);
                DSDatabaseManager.Current.Unregister(actor);
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
    }
}

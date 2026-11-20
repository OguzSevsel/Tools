using System.Collections.Generic;
using System.ComponentModel;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : Tab, IShortcutContext
    {
        private ScrollView actorScrollView;
        private VisualElement actorContainer;
        private VisualTreeAsset cardAsset;
        private Button createActorButton;
        public Dictionary<DSActor, VisualElement> Actors { get; set; }

        public bool active => EditorWindow.focusedWindow is DSEditorView window && window.IsActorsTabActive;

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

        public void CreateActor()
        {
            DSActor actor = new("New Actor", "Actor Description", "Actor Background", default);
            VisualElement cardElement = CreateCard(cardAsset, actor);
            AddCard(actorContainer, Actors, actor, cardElement);
            DSDatabaseManager.Instance.Current.Register(actor);
        }

        #region INITIALIZATION

        private void AddCard(VisualElement container, Dictionary<DSActor, VisualElement> actors, DSActor actor, VisualElement cardElement)
        {
            container.Add(cardElement);
            actors.Add(actor, cardElement);

            Button deleteButton = cardElement.Q<Button>("DeleteButton");
            deleteButton.clicked += () =>
            {
                bool confirm = EditorUtility.DisplayDialog(
                    $"Delete {actor.Name}",
                    $"Are you sure you want to delete {actor.Name}?",
                    "Delete",
                    "Cancel"
                );

                if (!confirm) return;

                container.Remove(cardElement);
                actors.Remove(actor);
                DSDatabaseManager.Instance.Current.Unregister(actor);
            };
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
            CreateActor();
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : Tab, IShortcutContext
    {
        private ScrollView actorScrollView;
        private VisualElement actorContainer;
        private VisualElement buttonContainer;
        private Button deleteAllButton;
        private Button selectAllButton;

        private VisualTreeAsset cardAsset;
        private Button createActorButton;
        public Dictionary<DSActor, VisualElement> Actors { get; set; }
        public List<DSActor> selectedActors { get; }

        public bool active => EditorWindow.focusedWindow is DSEditorView window && window.IsActorsTabActive;

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";
            Actors = new Dictionary<DSActor, VisualElement>();
            selectedActors = new List<DSActor>();
            this.cardAsset = cardAsset;

            actorScrollView = this.Q<ScrollView>("ActorsScrollView");
            actorContainer = this.Q<VisualElement>("ActorsCardContainer");
            createActorButton = this.Q<Button>("CreateActorButton");
            buttonContainer = this.Q<VisualElement>("ButtonContainer");
            deleteAllButton = this.Q<Button>("DeleteSelectedButton");
            selectAllButton = this.Q<Button>("SelectAllButton");
            
            RegisterEvents();
        }

        public void CreateActor()
        {
            DSActor actor = new("New Actor", "Actor Description", "Actor Background", default);
            VisualElement cardElement = CreateCard(cardAsset, actor);
            AddCard(actorContainer, Actors, actor, cardElement);
            DSDatabaseManager.Instance.Current.Register(actor);
        }

        public void DeleteActor(DSActor actor, VisualElement actorContainer)
        {
            var cardElement = Actors[actor];
            actorContainer.Remove(cardElement);
            Actors.Remove(actor);
            DSDatabaseManager.Instance.Current.Unregister(actor);
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

                DeleteActor(actor, container);
            };
        }

        private VisualElement CreateCard(VisualTreeAsset cardAsset, DSActor actor)
        {
            VisualElement cardElement = new();
            cardAsset.CloneTree(cardElement);

            var checkBox = cardElement.Q<Toggle>("SelectionCheckBox");
            var image = cardElement.Q<Image>("Image");

            cardElement.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow();
                    if (!checkBox.value)
                    {
                        checkBox.SetVisible(true);
                        image.SetVisible(false);
                    }
                }
            });
            cardElement.RegisterCallback<MouseLeaveEvent>((evt) =>
            {
                if (evt.currentTarget is VisualElement element)
                {
                    element.SetDropShadow(true);
                    if (!checkBox.value)
                    {
                        checkBox.SetVisible(false);
                        image.SetVisible(true);
                    }
                }
            });
            cardElement.RegisterCallback<MouseDownEvent>((evt) =>
            {
                checkBox.value = !checkBox.value;

                if (checkBox.value)
                {
                    if (!selectedActors.Contains(actor))
                    {
                        selectedActors.Add(actor);
                        Debug.Log($"Selected Actors Count: {selectedActors.Count}");
                    }
                }
                else
                {
                    if (selectedActors.Contains(actor))
                    {
                        selectedActors.Remove(actor);
                        Debug.Log($"Selected Actors Count: {selectedActors.Count}");
                    }
                }

                if (selectedActors.Count > 0)
                {
                    buttonContainer.SetVisible(true);
                }
                else
                {
                    buttonContainer.SetVisible(false);
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
            createActorButton.clicked -= HandleOnCreateActorButtonClicked;
            selectAllButton.clicked -= HandleSelectAllButtonClicked;
            deleteAllButton.clicked -= HandleDeleteSelectedButtonClicked;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void RegisterEvents()
        {
            DSDatabaseManager.Instance.DatabaseClosed += HandleOnDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened += HandleOnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
            createActorButton.clicked += HandleOnCreateActorButtonClicked;
            selectAllButton.clicked += HandleSelectAllButtonClicked;
            deleteAllButton.clicked += HandleDeleteSelectedButtonClicked;
        }

        private void HandleDeleteSelectedButtonClicked()
        {
            var deletedActors = new List<DSActor>();

            if (selectedActors.Count > 0)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    $"Delete Actors",
                    $"Are you sure you want to delete {selectedActors.Count} Actors?",
                    "Delete",
                    "Cancel"
                );

                if (!confirm) return;

                foreach (var actor in selectedActors)
                {
                    DeleteActor(actor, actorContainer);
                    deletedActors.Add(actor);
                }
            }

            foreach (var actor in deletedActors)
            {
                selectedActors.Remove(actor);
            }
            buttonContainer.SetVisible(false);
        }

        private void HandleSelectAllButtonClicked()
        {
            if (selectedActors.Count >= Actors.Count)
            {
                foreach (var kvp in Actors)
                {
                    var actor = kvp.Key;
                    var card = kvp.Value;

                    var checkBox = card.Q<Toggle>("SelectionCheckBox");
                    var image = card.Q<Image>("Image");

                    checkBox.value = false;

                    image.SetVisible(true);
                    checkBox.SetVisible(false);

                    if (selectedActors.Contains(actor))
                    {
                        selectedActors.Remove(actor);
                    }
                }
                Debug.Log($"Selected Actors Count: {selectedActors.Count}");
                buttonContainer.SetVisible(false);
            }
            else
            {
                foreach (var kvp in Actors)
                {
                    var actor = kvp.Key;
                    var card = kvp.Value;

                    var checkBox = card.Q<Toggle>("SelectionCheckBox");
                    var image = card.Q<Image>("Image");

                    checkBox.value = true;

                    image.SetVisible(false);
                    checkBox.SetVisible(true);

                    if (!selectedActors.Contains(actor))
                    {
                        selectedActors.Add(actor);
                    }
                }
                Debug.Log($"Selected Actors Count: {selectedActors.Count}");
            }
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
            foreach (var kvp in Actors)
            {
                var actor = kvp.Key;
                var card = kvp.Value;

                var checkBox = card.Q<Toggle>("SelectionCheckBox");
                var image = card.Q<Image>("Image");

                checkBox.value = false;

                image.SetVisible(true);
                checkBox.SetVisible(false);
            }

            actorContainer.Clear();
            Actors.Clear();
            selectedActors.Clear();
        }

        private void HandleOnCreateActorButtonClicked()
        {
            CreateActor();
        }

        #endregion
    }
}

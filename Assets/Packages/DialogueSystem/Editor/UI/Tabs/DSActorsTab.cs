using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : DSTab, IShortcutContext
    {
        public bool active => EditorWindow.focusedWindow is DSEditorView window && window.IsActorsTabActive;

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset) : base(tabAsset, cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";

            scrollView = this.Q<ScrollView>("ActorsScrollView");
            cardContainer = this.Q<VisualElement>("ActorsCardContainer");
            createButton = this.Q<Button>("CreateActorButton");
            buttonContainer = this.Q<VisualElement>("ButtonContainer");
            deleteAllButton = this.Q<Button>("DeleteSelectedButton");
            selectAllButton = this.Q<Button>("SelectAllButton");
            
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            createButton.clicked += CreateActor;
            selectAllButton.clicked += HandleSelectAllButtonClicked;
            deleteAllButton.clicked += HandleDeleteSelectedButtonClicked;
            DSDatabaseManager.Instance.DatabaseOpened += HandleOnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            createButton.clicked -= CreateActor;
            selectAllButton.clicked -= HandleSelectAllButtonClicked;
            deleteAllButton.clicked -= HandleDeleteSelectedButtonClicked;
            DSDatabaseManager.Instance.DatabaseOpened -= HandleOnDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        public void CreateActor()
        {
            DSActor actor = new DSActor("New Actor", "Actor Description", "Actor Background", null);
            CreateItem(actor);
        }

        private void HandleOnDatabaseOpened(DSDatabase database)
        {
            foreach (DSActor actor in database.Actors)
            {
                VisualElement card = CreateCard(cardAsset, actor);
                AddCard(cardContainer, Items, actor, card);
            }
        }
    }
}

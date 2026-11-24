using Tools.DialogueSystem.Data;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialoguesTab : DSTab, IShortcutContext
    {
        public bool active => EditorWindow.focusedWindow is DSEditorView window && window.IsDialoguesTabActive;

        public DSDialoguesTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset) : base(tabAsset, cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Dialogues";

            createButton = this.Q<Button>("CreateDialogueButton");
            cardContainer = this.Q<VisualElement>("DialogueCardContainer");
            scrollView = this.Q<ScrollView>("DialogueScrollView");
            buttonContainer = this.Q<VisualElement>("ButtonContainer");
            deleteAllButton = this.Q<Button>("DeleteSelectedButton");
            selectAllButton = this.Q<Button>("SelectAllButton");
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            deleteAllButton.clicked += HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked += HandleSelectAllButtonClicked;
            createButton.clicked += CreateDialogue;
            DSDatabaseManager.Instance.DatabaseOpened += OnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            deleteAllButton.clicked -= HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked -= HandleSelectAllButtonClicked;
            createButton.clicked -= CreateDialogue;
            DSDatabaseManager.Instance.DatabaseOpened -= OnDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        public void CreateDialogue()
        {
            DSDialogueText dialogue = new("New Dialogue", "Dialogue Description", "Dialogue Text");
            CreateItem(dialogue);
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSDialogueText text in database.DialogueTexts)
            {
                VisualElement card = CreateCard(cardAsset, text);
                AddCard(cardContainer, Items, text, card);
            }
        }
    }
}
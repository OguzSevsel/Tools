using Tools.DialogueSystem.Data;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSAudioClipsTab : DSTab, IShortcutContext
    {
        public bool active => EditorWindow.focusedWindow is DSEditorView window && window.IsAudioClipsTabActive;

        public DSAudioClipsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset) : base(tabAsset, cardAsset) 
        {
            tabAsset.CloneTree(this);
            label = "Audio Clips";

            cardContainer = this.Q<VisualElement>("AudioClipsCardContainer");
            scrollView = this.Q<ScrollView>("AudioClipsScrollView");
            createButton = this.Q<Button>("CreateAudioClipButton");
            buttonContainer = this.Q<VisualElement>("ButtonContainer");
            deleteAllButton = this.Q<Button>("DeleteSelectedButton");
            selectAllButton = this.Q<Button>("SelectAllButton");

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            deleteAllButton.clicked += HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked += HandleSelectAllButtonClicked;
            createButton.clicked += CreateAudioClip;
            DSDatabaseManager.Instance.DatabaseOpened += OnDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            deleteAllButton.clicked -= HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked -= HandleSelectAllButtonClicked;
            createButton.clicked -= CreateAudioClip;
            DSDatabaseManager.Instance.DatabaseOpened -= OnDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        public void CreateAudioClip()
        {
            DSAudioClip clip = new DSAudioClip("New Audio Clip", "Audio Clip Description", null);
            CreateItem(clip);
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (DSAudioClip audioClip in database.AudioClips)
            {
                VisualElement card = CreateCard(cardAsset, audioClip);
                AddCard(cardContainer, Items, audioClip, card);
            }
        }
    }
}
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
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

        public void CreateAudioClip()
        {
            DSAudioClip clip = new DSAudioClip("New Audio Clip", "Audio Clip Description", null);
            CreateItem(clip);
        }
        
        private void OnCreateAudioClipsButtonClicked()
        {
            DSAudioClip clip = new DSAudioClip("New Audio Clip", "Audio Clip Description", null);
            CreateItem(clip);
        }

        #region Utils

        private void RegisterEvents()
        {
            deleteAllButton.clicked += HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked += HandleSelectAllButtonClicked;
            createButton.clicked += OnCreateAudioClipsButtonClicked;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            deleteAllButton.clicked -= HandleDeleteSelectedButtonClicked;
            selectAllButton.clicked -= HandleSelectAllButtonClicked;
            createButton.clicked -= OnCreateAudioClipsButtonClicked;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        #endregion
    }
}
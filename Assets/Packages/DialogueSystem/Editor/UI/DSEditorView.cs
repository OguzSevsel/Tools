using System;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSEditorView : EditorWindow
    {
        [SerializeField] private VisualTreeAsset editorWindowAsset;
        [SerializeField] private VisualTreeAsset sideBarAsset;

        [SerializeField] private VisualTreeAsset actorsTabAsset;
        [SerializeField] private VisualTreeAsset actorsCardAsset;

        [SerializeField] private VisualTreeAsset audioClipsTabAsset;
        [SerializeField] private VisualTreeAsset audioClipCardAsset;

        [SerializeField] private VisualTreeAsset databaseTabAsset;

        [SerializeField] private VisualTreeAsset dialoguesTabAsset;
        [SerializeField] private VisualTreeAsset dialogueCardAsset;

        [SerializeField] private VisualTreeAsset conversationsTabAsset;
        [SerializeField] private VisualTreeAsset conversationCardAsset;

        DSGraphTab conversationsTab;
        DSActorsTab actorsTab;
        DSAudioClipsTab audioClipsTab;
        DSDialoguesTab dialoguesTab;
        DSDatabaseTab databaseTab;

        TabView tabView;

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorView>();
            window.maximized = true;
            window.titleContent = new UnityEngine.GUIContent("Database Name");
        }

        private void OnEnable()
        {
            CreateDefaultFolders();
            editorWindowAsset.CloneTree(rootVisualElement);
            tabView = rootVisualElement.Q<TabView>("TabView");
            tabView.activeTabChanged += OnActiveTabChanged;
            AddTabs();
            tabView.activeTab = conversationsTab;
        }

        private void OnActiveTabChanged(Tab tab1, Tab tab2)
        {

        }

        private void AddTabs()
        {
            conversationsTab = new DSGraphTab(conversationsTabAsset, sideBarAsset);
            actorsTab = new DSActorsTab(actorsTabAsset, actorsCardAsset);
            audioClipsTab = new DSAudioClipsTab(audioClipsTabAsset, audioClipCardAsset);
            dialoguesTab = new DSDialoguesTab(dialoguesTabAsset, dialogueCardAsset);
            databaseTab = new DSDatabaseTab(databaseTabAsset);

            tabView.Add(conversationsTab);
            tabView.Add(actorsTab);
            tabView.Add(audioClipsTab);
            tabView.Add(dialoguesTab);
            tabView.Add(databaseTab);
        }

        #region Utils

        private void CreateDefaultFolders()
        {
            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");
        }

        #endregion
    }
}

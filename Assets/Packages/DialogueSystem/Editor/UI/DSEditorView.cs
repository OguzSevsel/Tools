using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSEditorView : EditorWindow
    {
        DSGraphTab graphTab;
        DSActorsTab actorsTab;
        DSAudioClipsTab audioClipsTab;
        DSDialoguesTab dialoguesTab;
        DSDatabaseTab databaseTab;
        Button actorsTabButton;
        Button databaseTabButton;
        Button graphTabButton;
        Button dialoguesTabButton;
        Button audioClipsTabButton;

        Toolbar toolbar;
        VisualElement mainView;


        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorView>();
            window.maximized = true;
            window.titleContent = new UnityEngine.GUIContent("Database Name");
        }

        private void OnEnable()
        {
            CreateTabs();
            CreateMainView();
            CreateToolbar();
            CreateDefaultFolders();
            AddStyles();
            OnConversationsTabButtonClicked();
        }

        #region Tabs

        private void CreateMainView()
        {
            mainView = new VisualElement();
            rootVisualElement.Add(mainView);
        }

        private void CreateTabs()
        {
            graphTab = new DSGraphTab();
            actorsTab = new DSActorsTab();
            dialoguesTab = new DSDialoguesTab();
            audioClipsTab = new DSAudioClipsTab();
            databaseTab = new DSDatabaseTab();
        }

        #endregion

        #region Utils

        private void CreateDefaultFolders()
        {
            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
            rootVisualElement.style.flexGrow = 1;
            rootVisualElement.style.justifyContent = Justify.Center;

            mainView.SetFlex();

            toolbar.SetHeight(40, 40, 40);
            toolbar.SetAlignment(alignContent: Align.Center, Align.Center);

            foreach (var button in toolbar.Children())
            {
                button.SetHeight(30, 30, 30);
                button.SetFlex();
            }
        }

        #endregion

        #region Toolbar

        private void CreateToolbar()
        {
            toolbar = new Toolbar();

            actorsTabButton = DSElementUtility.CreateButton("Actors", OnActorsTabButtonClicked);
            databaseTabButton = DSElementUtility.CreateButton(text: "Database", OnDatabaseTabButtonClicked);
            graphTabButton = DSElementUtility.CreateButton("Conversations", OnConversationsTabButtonClicked);
            audioClipsTabButton = DSElementUtility.CreateButton("Audio Clips", OnAudioClipsTabButtonClicked);
            dialoguesTabButton = DSElementUtility.CreateButton("Dialogues", OnDialoguesTabButtonClicked);

            toolbar.Insert(0, graphTabButton);
            toolbar.Insert(1, databaseTabButton);
            toolbar.Insert(2, dialoguesTabButton);
            toolbar.Insert(3, actorsTabButton);
            toolbar.Insert(4, audioClipsTabButton);

            rootVisualElement.Insert(0, toolbar);
        }

        private void OnDialoguesTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(dialoguesTab);
        }

        private void OnAudioClipsTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(audioClipsTab);
        }

        private void OnConversationsTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(graphTab);
        }

        private void OnDatabaseTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(databaseTab);
        }

        private void OnActorsTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(actorsTab);
        }

        #endregion
    }
}

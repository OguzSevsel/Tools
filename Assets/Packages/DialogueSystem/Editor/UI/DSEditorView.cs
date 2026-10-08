using System;
using Tools.DialogueSystem.Data;
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

        DSGraphTab graphTab;
        DSActorsTab actorsTab;
        DSAudioClipsTab audioClipsTab;
        DSDialoguesTab dialoguesTab;
        DSDatabaseTab databaseTab;
        TabView tabView;
        VisualElement databaseDialogContainer;
        Button createDatabaseButton;
        Button loadDatabaseButton;
        public static event Action OnWindowCloses;

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
            AddTabs();
            tabView.activeTab = graphTab;

            databaseDialogContainer = rootVisualElement.Q<VisualElement>("DatabaseDialogContainer");
            createDatabaseButton = rootVisualElement.Q<Button>("CreateDatabaseButton");
            loadDatabaseButton = rootVisualElement.Q<Button>("LoadDatabaseButton");
            createDatabaseButton.clicked += OnCreateDatabaseButtonClicked;
            loadDatabaseButton.clicked += OnLoadDatabaseButtonClicked;
        }

        private void OnDisable()
        {
            OnWindowCloses?.Invoke();
        }

        private void OnLoadDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog(true);

            if (database != null)
            {
                DSDatabaseManager.Open(database);
                databaseDialogContainer.style.display = DisplayStyle.None;
                tabView.style.display = DisplayStyle.Flex;

                var window = GetWindow<DSEditorView>();
                window.titleContent = new UnityEngine.GUIContent(database.name);
            }
        }

        private void OnCreateDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog();

            if (database != null)
            {
                DSDatabaseManager.Open(database);
                databaseDialogContainer.style.display = DisplayStyle.None;
                tabView.style.display = DisplayStyle.Flex;

                var window = GetWindow<DSEditorView>();
                window.titleContent = new UnityEngine.GUIContent(database.name);
            }
        }

        public DSDatabase OpenDatabaseFileDialog(bool isLoad = false)
        {
            string path = "";

            if (isLoad)
            {
                path = EditorUtility.OpenFilePanel(
                    "Load Database",
                    Application.dataPath + "/DialogueSystem/Databases",
                    "asset"
                );
            }
            else
            {
                path = EditorUtility.SaveFilePanel(
                    "Create Database",
                    Application.dataPath + "/DialogueSystem/Databases",
                    "NewDatabase",
                    "asset"
                );
            }

            if (string.IsNullOrEmpty(path))
                return null;

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogError("Selected file must be inside Assets folder.");
                return null;
            }

            string relativePath =
                "Assets" + path.Substring(Application.dataPath.Length);

            if (isLoad)
            {
                return AssetDatabase.LoadAssetAtPath<DSDatabase>(relativePath);
            }

            DSDatabase database = ScriptableObject.CreateInstance<DSDatabase>();

            string uniquePath =
                AssetDatabase.GenerateUniqueAssetPath(relativePath);

            AssetDatabase.CreateAsset(database, uniquePath);

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return database;
        }

        private void AddTabs()
        {
            graphTab = new DSGraphTab(conversationsTabAsset, conversationCardAsset, sideBarAsset);
            actorsTab = new DSActorsTab(actorsTabAsset, actorsCardAsset);
            audioClipsTab = new DSAudioClipsTab(audioClipsTabAsset, audioClipCardAsset);
            dialoguesTab = new DSDialoguesTab(dialoguesTabAsset, dialogueCardAsset);
            databaseTab = new DSDatabaseTab(databaseTabAsset);

            tabView.Add(graphTab);
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

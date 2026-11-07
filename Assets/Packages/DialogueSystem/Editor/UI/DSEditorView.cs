using System;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.ShortcutManagement;
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

        private DSGraphTab graphTab;
        private DSActorsTab actorsTab;
        private DSAudioClipsTab audioClipsTab;
        private DSDialoguesTab dialoguesTab;
        private DSDatabaseTab databaseTab;
        private TabView tabView;
        private VisualElement databaseDialogContainer;
        private Button createDatabaseButton;
        private Button loadDatabaseButton;
        private Button returnButton;
        public static event Action OnWindowCloses;
        public static event Action OnToggleSideBarShortcut;

        public static DSEditorView Instance;

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            Instance = GetWindow<DSEditorView>();
            Instance.maximized = true;
            Instance.titleContent = new UnityEngine.GUIContent("No Database");
        }

        private void OnEnable()
        {
            var databaseManager = new DSDatabaseManager();
            CreateDefaultFolders();
            GetUIElements();
            CreateTabs();
            AddTabs();
            RegisterEvents();
        }

        private void OnDisable()
        {
            CloseDatabase();
            UnRegisterEvents();
            OnWindowCloses?.Invoke();
        }

        #region INITIALIZATION

        private void CreateDefaultFolders()
        {
            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");
        }

        private void GetUIElements()
        {
            editorWindowAsset.CloneTree(rootVisualElement);
            tabView = rootVisualElement.Q<TabView>("TabView");
            databaseDialogContainer = rootVisualElement.Q<VisualElement>("DatabaseDialogContainer");
            createDatabaseButton = rootVisualElement.Q<Button>("CreateDatabaseButton");
            loadDatabaseButton = rootVisualElement.Q<Button>("LoadDatabaseButton");
            returnButton = rootVisualElement.Q<Button>("ReturnButton");
            returnButton.SetVisible(false);
        }

        private void AddTabs()
        {
            tabView.Add(graphTab);
            tabView.Add(actorsTab);
            tabView.Add(audioClipsTab);
            tabView.Add(dialoguesTab);
            tabView.Add(databaseTab);
            tabView.activeTab = graphTab;
        }

        private void CreateTabs()
        {
            graphTab = new DSGraphTab(conversationsTabAsset, conversationCardAsset, sideBarAsset);
            actorsTab = new DSActorsTab(actorsTabAsset, actorsCardAsset);
            audioClipsTab = new DSAudioClipsTab(audioClipsTabAsset, audioClipCardAsset);
            dialoguesTab = new DSDialoguesTab(dialoguesTabAsset, dialogueCardAsset);
            databaseTab = new DSDatabaseTab(databaseTabAsset);
        }

        #endregion

        #region EVENTS

        private void RegisterEvents()
        {
            createDatabaseButton.clicked += HandleOnCreateDatabaseButtonClicked;
            loadDatabaseButton.clicked += HandleOnLoadDatabaseButtonClicked;
            returnButton.clicked += HandleReturnButtonClicked;
        }

        private void UnRegisterEvents()
        {
            createDatabaseButton.clicked -= HandleOnCreateDatabaseButtonClicked;
            loadDatabaseButton.clicked -= HandleOnLoadDatabaseButtonClicked;
            returnButton.clicked -= HandleReturnButtonClicked;
        }

        #endregion

        #region EVENT HANDLERS

        private void HandleReturnButtonClicked()
        {
            CloseDatabase();
        }

        private void HandleOnLoadDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog(true);

            if (database != null)
            {
                DSDatabaseManager.Instance.Open(database);

                databaseDialogContainer.SetVisible(false);
                tabView.SetVisible(true);
                returnButton.SetVisible(true);

                ChangeWindowTitle(database.name);
            }
        }

        private void HandleOnCreateDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog();

            if (database != null)
            {
                DSDatabaseManager.Instance.Open(database);

                databaseDialogContainer.SetVisible(false);
                tabView.SetVisible(true);
                returnButton.SetVisible(true);

                ChangeWindowTitle(database.name);
            }
        }

        #endregion

        #region SHORTCUTS

        [Shortcut("Dialogue System/Close Database", typeof(DSEditorView), KeyCode.W, ShortcutModifiers.Action)]
        private static void CloseDatabaseShortcut()
        {
            if (Instance == null)
                return;

            if (DSDatabaseManager.Instance.HasDatabase)
            {
                Instance.CloseDatabase();
            }
            else
            {
                Instance.Close();
            }
        }

        [Shortcut("Dialogue System/Toggle Side Bar", typeof(DSEditorView), KeyCode.R, ShortcutModifiers.Action)]
        private static void ToggleSideBarShortcut()
        {
            if (Instance == null)
                return;

            OnToggleSideBarShortcut?.Invoke();
        }

        [Shortcut("Dialogue System/Next Tab", typeof(DSEditorView), KeyCode.E, ShortcutModifiers.Action)]
        private static void NextTabShortcut()
        {
            if (Instance == null)
                return;

            Instance.ChangeTab(1);
        }

        [Shortcut("Dialogue System/Previous Tab", typeof(DSEditorView), KeyCode.Q, ShortcutModifiers.Action)]
        private static void PreviousTabShortcut()
        {
            if (Instance == null)
                return;

            Instance.ChangeTab(-1);
        }

        #endregion

        #region Utils

        private void ChangeWindowTitle(string title)
        {
            Instance = GetWindow<DSEditorView>();
            Instance.titleContent = new UnityEngine.GUIContent(title);
        }

        private void CloseDatabase()
        {
            if (DSDatabaseManager.Instance.HasDatabase)
            {
                ChangeWindowTitle("No Database");
                returnButton.SetVisible(false);
                databaseDialogContainer.SetVisible(true);
                tabView.SetVisible(false);
                tabView.activeTab = graphTab;

                DSDatabaseManager.Instance.Close();
            }
        }

        private void ChangeTab(int direction)
        {
            int count = tabView.Query<Tab>().ToList().Count;

            if (count == 0)
                return;

            tabView.selectedTabIndex =
                (tabView.selectedTabIndex + direction + count) % count;
        }

        public DSDatabase OpenDatabaseFileDialog(bool isLoad = false)
        {
            string path = isLoad
                ? EditorUtility.OpenFilePanel(
                    "Load Database",
                    Application.dataPath + "/DialogueSystem/Databases",
                    "asset"
                )
                : EditorUtility.SaveFilePanel(
                    "Create Database",
                    Application.dataPath + "/DialogueSystem/Databases",
                    "NewDatabase",
                    "asset"
                );
            if (string.IsNullOrEmpty(path))
                return null;

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogError("Selected file must be inside Assets folder.");
                return null;
            }

            string relativePath =
                "Assets" + path[Application.dataPath.Length..];

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

        #endregion
    }
}



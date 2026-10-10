using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSEditorWindowLegacy : EditorWindow
    {
        private VisualElement mainView;
        private DSGraphTab graphTab;
        private DSActorsTab actorsTab;
        private DSDatabaseTab databaseTab;
        private DSConversationsTab conversationsTab;
        private DSAudioClipsTab audioClipsTab;
        private DSDialoguesTab dialoguesTab;

        private Button actorsTabButton;
        private Button databaseTabButton;
        private Button graphTabButton;
        private Button conversationsTabButton;
        private Button dialoguesTabButton;
        private Button audioClipsTabButton;

        private Toolbar toolbar;
        private DSDialogElement databaseDialog;


        //[MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            DSEditorWindowLegacy window = GetWindow<DSEditorWindowLegacy>();
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
        }

        private static void OnDatabaseClosed(DSDatabase database)
        {

        }

        private static void OnDatabaseOpened(DSDatabase database)
        {
            DSEditorWindowLegacy window = GetWindow<DSEditorWindowLegacy>();
            window.titleContent = new GUIContent(database.name);
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexGrow = 1;
            rootVisualElement.style.justifyContent = Justify.Center;
            mainView = DSElementUtility.CreateVisualElement(rootVisualElement);

            //conversationsTab = new DSConversationsTab();
            //actorsTab = new DSActorsTab();
            //databaseTab = new DSDatabaseTab();
            //graphTab = new DSGraphTab();
            //audioClipsTab = new DSAudioClipsTab();
            //dialoguesTab = new DSDialoguesTab();

            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");

            CreateDatabaseDialog();
            AddStyles();
        }

        private void OnDisable()
        {
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.Close();
        }

        private void CreateUIElements()
        {
            toolbar = new Toolbar();

            actorsTabButton = DSElementUtility.CreateButton("Actors", OnActorsTabButtonClicked);
            databaseTabButton = DSElementUtility.CreateButton(text: "Database", OnDatabaseTabButtonClicked);
            graphTabButton = DSElementUtility.CreateButton("Graph", OnGraphTabButtonClicked);
            audioClipsTabButton = DSElementUtility.CreateButton("Audio Clips", OnAudioClipsTabButtonClicked);
            dialoguesTabButton = DSElementUtility.CreateButton("Dialogues", OnDialoguesTabButtonClicked);
            conversationsTabButton = DSElementUtility.CreateButton("Conversations", OnConversationsTabButtonClicked);

            toolbar.Add(databaseTabButton);
            toolbar.Add(conversationsTabButton);
            toolbar.Add(dialoguesTabButton);
            toolbar.Add(actorsTabButton);
            toolbar.Add(audioClipsTabButton);
            toolbar.Add(graphTabButton);

            rootVisualElement.Insert(0, toolbar);
        }

        private void CreateDatabaseDialog()
        {
            VisualElement dialogRoot = new();

            databaseDialog = new DSDialogElement(dialogRoot);

            rootVisualElement.Add(dialogRoot);

            databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseButtonClicked, OnLoadDatabaseButtonClicked);
        }

        private void OnCreateDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog();

            if (database != null)
            {
                DSDatabaseManager.Open(database);
                CreateUIElements();
                return;
            }
            else
            {
                databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseButtonClicked, OnLoadDatabaseButtonClicked);
            }
        }

        private void OnLoadDatabaseButtonClicked()
        {
            DSDatabase database = OpenDatabaseFileDialog(true);

            if (database != null)
            {
                DSDatabaseManager.Open(database);
                CreateUIElements();
                return;
            }
            else
            {
                databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseButtonClicked, OnLoadDatabaseButtonClicked);
            }
        }

        private void OnConversationsTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(conversationsTab);
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

        private void OnActorsTabButtonClicked() => mainView.Clear();//mainView.Add(actorsTab);

        private void OnDatabaseTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(databaseTab);
        }

        private void OnGraphTabButtonClicked()
        {
            mainView.Clear();
            mainView.Add(graphTab);
        }

        private void AddStyles() => rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");

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
    }
}

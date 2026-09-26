using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSEditorWindow : EditorWindow
    {
        VisualElement mainView;
        DSGraphTab graphTab;
        DSActorsTab actorsTab;
        DSDatabaseTab databaseTab;
        
        Button actorsTabButton;
        Button databaseTabButton;
        Button graphTabButton;

        Toolbar toolbar;
        DSDialogElement databaseDialog;


        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexGrow = 1;
            rootVisualElement.style.justifyContent = Justify.Center;
            mainView = DSElementUtility.CreateVisualElement(rootVisualElement);
            mainView.style.backgroundColor = Color.red;

            DSIOUtility.CreateFolderIfNotExists("Assets", "DialogueSystem");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Conversations");
            DSIOUtility.CreateFolderIfNotExists("Assets/DialogueSystem", "Databases");

            CreateDatabaseDialog();
            AddStyles();
        }
        
        private void CreateUIElements()
        {
            toolbar = new Toolbar();

            actorsTabButton = DSElementUtility.CreateButton("Actors", OnActorsTabButtonClicked);
            databaseTabButton = DSElementUtility.CreateButton(text: "Database", OnDatabaseTabButtonClicked);
            graphTabButton = DSElementUtility.CreateButton("Graph", OnGraphTabButtonClicked);

            toolbar.Add(actorsTabButton);
            toolbar.Add(databaseTabButton);
            toolbar.Add(graphTabButton);
            
            rootVisualElement.Insert(0,toolbar);
        }

        private void CreateDatabaseDialog()
        {
            VisualElement dialogRoot = new VisualElement();

            databaseDialog = new DSDialogElement(dialogRoot);

            rootVisualElement.Add(dialogRoot);
            
            databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseTabButtonClicked, OnLoadDatabaseTabButtonClicked);
        }
                                                  
        private void OnCreateDatabaseTabButtonClicked()
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
                databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseTabButtonClicked, OnLoadDatabaseTabButtonClicked);
            }
        }

        private void OnLoadDatabaseTabButtonClicked()
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
                databaseDialog.Show("", "", "Create New Database", "Load Database", OnCreateDatabaseTabButtonClicked, OnLoadDatabaseTabButtonClicked);
            }
        }

        private void OnActorsTabButtonClicked()
        {
            mainView.Clear();
            actorsTab = new DSActorsTab();
            mainView.Add(actorsTab);
        }

        private void OnDatabaseTabButtonClicked()
        {
            mainView.Clear();
            databaseTab = new DSDatabaseTab();
            mainView.Add(databaseTab);
        }

        private void OnGraphTabButtonClicked()
        {
            mainView.Clear();
            graphTab = new DSGraphTab();
            mainView.Add(graphTab);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
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
    }
}

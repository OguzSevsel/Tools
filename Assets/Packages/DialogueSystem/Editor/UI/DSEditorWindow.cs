using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Tools.DialogueSystem.Utilities;

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

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            rootVisualElement.style.flexGrow = 1;
            AddTabs();
            mainView = DSElementUtility.CreateVisualElement(rootVisualElement);
            mainView.style.backgroundColor = Color.red;
            AddStyles();
        }
        
        private void AddTabs()
        {
            Toolbar toolbar = new Toolbar();

            actorsTabButton = DSElementUtility.CreateButton("Actors", OnActorsTabButtonClicked);
            databaseTabButton = DSElementUtility.CreateButton(text: "Database", OnDatabaseTabButtonClicked);
            graphTabButton = DSElementUtility.CreateButton("Graph", OnGraphTabButtonClicked);

            toolbar.Add(actorsTabButton);
            toolbar.Add(databaseTabButton);
            toolbar.Add(graphTabButton);
            
            rootVisualElement.Add(toolbar);
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
    } 
}

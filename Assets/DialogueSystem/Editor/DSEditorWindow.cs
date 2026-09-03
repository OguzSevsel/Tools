using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Tools.DialogueSystem.Utilities;

namespace Tools.DialogueSystem
{
    public class DSEditorWindow : EditorWindow
    {
        DSGraphView graphView;
        Button saveButton;
        Button loadButton;
        Button miniMapButton;

        public TextField FileNameTextField { get; set; }

        private string fileName;

        [MenuItem("Tools/Dialogue Graph")]
        public static void Open()
        {
            var window = GetWindow<DSEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
            SetFileName("New Dialogue");
        }

        public void SetFileName(string newFileName)
        {
            fileName = newFileName;
            FileNameTextField.value = fileName;
        }

        public string GetFileName()
        {
            return fileName;
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            FileNameTextField = DSElementUtility.CreateTextField(fileName, "File Name: ", onValueChanged: (evt) =>
            {
                fileName = evt.newValue;
            });

            saveButton = DSElementUtility.CreateButton("Save");
            saveButton.clicked += SaveButtonClickHandler;

            loadButton = DSElementUtility.CreateButton("Load");
            loadButton.clicked += LoadButtonClickHandler;

            miniMapButton = DSElementUtility.CreateButton("Mini Map");
            miniMapButton.clicked += MiniMapButtonClickHandler;

            toolbar.Add(FileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(miniMapButton);

            rootVisualElement.Add(toolbar);
        }

        private void MiniMapButtonClickHandler()
        {
            graphView.ToggleMiniMap();
        }

        private void LoadButtonClickHandler()
        {
            graphView.Load();
        }

        private void SaveButtonClickHandler()
        {
            graphView.Save();
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("DialogueSystem/DSVariables.uss");
        }

        private void AddGraphView()
        {
            graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);   
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
            loadButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
            loadButton.SetEnabled(false);
        }
    } 
}

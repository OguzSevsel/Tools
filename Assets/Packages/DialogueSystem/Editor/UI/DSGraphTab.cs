using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphTab : VisualElement
    {
        DSGraphView graphView;
        Button saveButton;
        Button loadButton;
        Button miniMapButton;

        public TextField FileNameTextField { get; set; }
        private string fileName;

        public DSGraphTab()
        {
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
            this.style.backgroundColor = Color.red;
            Create();
        }

        public void Create()
        {
            AddGraphView();
            AddToolBar();
            SetFileName("New Dialogue");
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            FileNameTextField = DSElementUtility.CreateTextField(fileName, "File Name: ", onValueChanged: (evt) =>
            {
                fileName = evt.newValue;
            });

            saveButton = DSElementUtility.CreateButton("Save", SaveButtonClickHandler);
            loadButton = DSElementUtility.CreateButton("Load", LoadButtonClickHandler);
            miniMapButton = DSElementUtility.CreateButton("Mini Map", MiniMapButtonClickHandler);

            toolbar.Add(FileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(miniMapButton);
            graphView.Add(toolbar);
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

        private void AddGraphView()
        {
            graphView = new DSGraphView(this);
            this.Add(graphView);
            graphView.StretchToParentSize();
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

        public void SetFileName(string newFileName)
        {
            fileName = newFileName;
            FileNameTextField.value = fileName;
        }

        public string GetFileName()
        {
            return fileName;
        }
    }
}

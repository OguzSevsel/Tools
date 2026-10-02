using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphTab : Tab
    {
        DSGraphView graphView;
        public DSSideBar sideBar;
        Button miniMapButton;

        public DSGraphTab(VisualTreeAsset tabAsset, VisualTreeAsset sideBarAsset)
        {
            tabAsset.CloneTree(this);
            Create();
        }

        public void Create()
        {
            AddStyles();
            //AddToolBar();
            AddGraphView();
            //sideBar = new DSSideBar();
            //this.mainView.Add(sideBar);
        }

        private void AddGraphView()
        {
            graphView = new DSGraphView(this);
            this.contentContainer.Add(graphView);
            graphView.SetFlex(flexGrow: 1);
        }

        private void AddStyles()
        {
            this.contentContainer.SetFlex(flexGrow: 1, flexDirection: FlexDirection.Column);
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();
            miniMapButton = DSElementUtility.CreateButton("Mini Map", MiniMapButtonClickHandler);
            toolbar.Add(miniMapButton);
            toolbar.SetHeight(30, 30, 30);
            this.contentContainer.Insert(0, toolbar);
        }

        private void MiniMapButtonClickHandler()
        {
            graphView.ToggleMiniMap();
        }
    }
}

using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphView : GraphView
    {
        //Nodes
        public List<DSNode> Nodes;

        //UI Elements
        private DSGraphTab tab;
        private VisualElement sideBar;
        private VisualElement inspectorContainer;
        private IDSNodeInspector inspector;

        //Utils
        private DSNode selectedNode;

        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
            Nodes = new List<DSNode>();

            AddSideBar();
            AddGridBackground();
            AddManipulators();
            AddStyles();
            RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        }

        #region Initialization

        private void AddManipulators()
        {
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Node"));
            SetupZoom(0.5f, 3f);
        }

        private IManipulator CreateNodeContextualMenu(DialogueType type, string actionTitle)
        {
            ContextualMenuManipulator manipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent =>
                {
                    if (this.Nodes.Count == 0)
                    {
                        AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: true, "Dialogue ID", "Actor Name", null, null, "Dialogue Text"));
                    }
                    else
                    {
                        AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: false, "Dialogue ID", "Actor Name", null, null, "Dialogue Text"));
                    }
                })
            );

            return manipulator;
        }

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            DSDialogueText text = new DSDialogueText("dialogueName", dialogueText);
            DSActor actor = new DSActor(actorName, "", actorSprite);
            DSAudioClip clip = new DSAudioClip("audioClip", audioClip);

            node.Initialize(position, type, text, actor, clip, isStartNode);
            this.Nodes.Add(node);

            node.OnNodeSelect += OnNodeSelected;

            node.OnEdgeDeleted += (edge) =>
            {
                if (this.Contains(edge))
                {
                    this.RemoveElement(edge);
                }
            };

            return node;
        }

        private void AddSideBar()
        {
            sideBar = new VisualElement();
            inspectorContainer = new VisualElement();
            VisualElement resizeElement = new VisualElement();

            resizeElement.SetBackgroundColor(Color.green);
            resizeElement.SetWidth(10, 10, 10);
            resizeElement.SetFlex();

            inspectorContainer.SetFlex();
            inspectorContainer.SetBackgroundColor(Color.blue);

            sideBar.SetAlignment(alignSelf: Align.FlexEnd, alignItems: Align.Stretch);
            sideBar.SetFlex(flexGrow: 1, flexShrink: 1, flexDirection: FlexDirection.Row);
            sideBar.SetWidth(300, 600, 200);
            sideBar.SetBackgroundColor(Color.red);

            sideBar.Add(resizeElement);
            sideBar.Add(inspectorContainer);
            resizeElement.AddManipulator(new ResizeManipulator(sideBar));
            this.contentContainer.Add(sideBar);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
        }

        #endregion

        #region Events

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            if (evt.target != this)
                return;

            CloseInspector();
        }

        private void OnNodeSelected(DSNode selectedNode)
        {
            this.selectedNode = selectedNode;

            if (selectedNode is DSNode node && inspector == null)
            {
                inspector = new DSNodeInspector(node.Data);
                this.inspectorContainer.Add(inspector as DSNodeInspector);
                return;
            }

            this.inspectorContainer.Add(inspector as DSNodeInspector);
        }

        #endregion

        #region Utils

        private void CloseInspector()
        {
            if (inspector is DSNodeInspector nodeInspector)
            {
                nodeInspector.RemoveFromHierarchy();
            }
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = tab.ChangeCoordinatesTo(tab, mousePosition);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        #endregion
        
        #region Overrides

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion
    }
}

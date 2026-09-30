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
        public List<DSNode> Nodes;
        public VisualElement sideBar;

        private DSNode selectedNode;
        private DSGraphTab tab;
        private IDSNodeInspector inspector;

        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
            Nodes = new List<DSNode>();
            sideBar = new VisualElement();
            VisualElement element = new VisualElement();

            element.style.width = 10;
            element.style.flexGrow = 1;
            element.style.backgroundColor = Color.green;
            element.AddManipulator(new ResizeManipulator(sideBar));
            element.style.alignSelf = Align.FlexStart;

            sideBar.style.flexDirection = FlexDirection.Column;
            sideBar.Add(element);
            sideBar.SetAlignment(Align.FlexEnd);
            sideBar.style.width = 300;
            sideBar.style.flexGrow = 1;
            sideBar.style.backgroundColor = Color.red;

            this.contentContainer.Add(sideBar);

            AddGridBackground();
            AddManipulators();
            AddStyles();
            RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            if (evt.target != this)
                return;

            CloseInspector();
        }

        private void CloseInspector()
        {
            if (inspector is DSNodeInspector nodeInspector)
            {
                nodeInspector.RemoveFromHierarchy();
            }
        }

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

        private void OnNodeSelected(DSNode selectedNode)
        {
            this.selectedNode = selectedNode;

            if (selectedNode is DSNode node && inspector == null)
            {
                inspector = new DSNodeInspector(node.Data);
                this.contentContainer.Add(inspector as DSNodeInspector);
                return;
            }

            this.contentContainer.Add(inspector as DSNodeInspector);
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
    }
}

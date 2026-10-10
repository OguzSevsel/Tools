using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphView : GraphView
    {
        public DSConversation CurrentConversation { get; private set; }

        //UI Elements
        private DSGraphTab tab;
        private IDSNodeInspector inspector;
        private MiniMap miniMap;

        //Utils
        private DSNode selectedNode;

        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
            CurrentConversation = new DSConversation("Oguzun convosu", "Default Conversation");
            DSDatabaseManager.Open(ScriptableObject.CreateInstance<DSDatabase>());

            AddGridBackground();
            AddMiniMap();
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
            ContextualMenuManipulator manipulator = new(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent =>
                {
                    if (CurrentConversation.GetNodes().Count == 0)
                    {
                        AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: true, "Dialogue ID", "Actor Name", "Conversant Name", null, null, null, "Dialogue Text"));
                    }
                    else
                    {
                        AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: false, "Dialogue ID", "Actor Name", "Conversant Name", null, null, null, "Dialogue Text"));
                    }
                })
            );

            return manipulator;
        }

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, string dialogueId, string actorName, string conversantName, AudioClip audioClip, Sprite actorSprite, Sprite conversantSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            DSDialogueText text = new("New Dialogue Text", "Dialogue Description", dialogueText);
            DSActor actor = new(actorName, "Actor Description", "Actor Background", actorSprite);
            DSActor conversant = new(conversantName, "Conversant Description", "Conversant Background", conversantSprite);
            DSAudioClip clip = new("New Audio Clip", "Audio Description", audioClip);

            node.Initialize(position, type, text, actor, conversant, clip, isStartNode);
            CurrentConversation.AddToNodes(node);

            node.OnNodeSelect += OnNodeSelected;

            node.OnEdgeDeleted += (edge) =>
            {
                if (Contains(edge))
                {
                    RemoveElement(edge);
                }
            };

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            _ = this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
        }

        private void AddMiniMap()
        {
            miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(5, 30, 200, 200));
            Add(miniMap);
            ToggleMiniMap();
        }

        public void ToggleMiniMap() => miniMap.visible = !miniMap.visible;

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
                return;
            }

            inspector.LoadFields(selectedNode.Data);
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
            List<Port> compatiblePorts = new();

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

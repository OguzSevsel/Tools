using System;
using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphView : GraphView
    {
        public List<DSNode> Nodes;
        private DSGraphTab tab;


        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
            Nodes = new List<DSNode>();
            AddGridBackground();
            AddManipulators();
            AddStyles();
        }

        private void AddManipulators()
        {
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Node"));

            //this.AddManipulator(new MousePan());
            this.AddManipulator(new ContentDragger());
            SetupZoom(0.5f, 3f);

            //this.AddManipulator(new ContentDragger());
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

            node.OnEdgeDeleted += (edge) =>
            {
                if (this.Contains(edge))
                {
                    this.RemoveElement(edge);
                }
            };

            return node;
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

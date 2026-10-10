using System;
using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    #region Copy Data

    [Serializable]
    public class DialogueCopyBuffer
    {
        public List<NodeCopyData> nodes = new();
    }

    [Serializable]
    public class NodeCopyData
    {
        public string DialogueId;
        public AudioClip AudioClip;
        public string ActorName;
        public string ConversantName;
        public Sprite ActorSprite;
        public Sprite ConversantSprite;
        public string DialogueText;
        public DialogueType DialogueType;
        public Vector2 Position;
        public List<DSPortData> Ports = new();
        public List<string> PortNames = new();
    }

    #endregion

    public class DSGraphViewLegacy : GraphView
    {
        public List<DSNodeLegacy> Nodes;
        private DSGraphTab tab;
        public int NodeErrorCount;
        private DSGraphSO loadedGraph;
        private MiniMap miniMap;
        private DialogueCopyBuffer copyBuffer;

        public Vector2 LastMousePosition { get; private set; }

        #region Initialization

        public DSGraphViewLegacy(DSGraphTab tab)
        {
            NodeErrorCount = 0;
            Nodes = new List<DSNodeLegacy>();
            this.tab = tab;

            AddManipulators();
            AddGridBackground();
            AddStyles();
            AddMiniMap();

            OnElementsDeleted();

            serializeGraphElements += Serialize;
            unserializeAndPaste += PasteSerialized;
            canPasteSerializedData += CanPaste;

            RegisterCallback<MouseMoveEvent>(evt =>
            {
                LastMousePosition = GetLocalMousePosition(evt.localMousePosition);
            });
        }

        private void AddStyles()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddManipulators()
        {
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Single Choice Node"));
            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Multi, "Add Multi Choice Node"));

            //this.AddManipulator(new MousePan());
            this.AddManipulator(new ContentDragger());
            SetupZoom(0.5f, 3f);

            //this.AddManipulator(new ContentDragger());
        }

        #endregion

        #region Copy/Paste/Duplicate/Delete

        private string Serialize(IEnumerable<GraphElement> elements)
        {
            Copy();
            return JsonUtility.ToJson(copyBuffer);
        }

        private void PasteSerialized(string op, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            if (!data.Contains("\"nodes\"")) // cheap but effective filter
                return;

            DialogueCopyBuffer buffer;

            try
            {
                buffer = JsonUtility.FromJson<DialogueCopyBuffer>(data);
            }
            catch
            {
                return; // silently fail like Unity does
            }

            if (buffer == null || buffer.nodes == null || buffer.nodes.Count == 0)
                return;

            copyBuffer = buffer;
            PasteAtMouse();
        }

        private bool CanPaste(string data) => !string.IsNullOrEmpty(data);

        private void PasteAtMouse()
        {
            if (copyBuffer == null || copyBuffer.nodes.Count == 0)
                return;

            Vector2 mousePos = LastMousePosition;
            Vector2 center = GetCopiedNodesCenter(copyBuffer);

            ClearSelection();

            foreach (NodeCopyData data in copyBuffer.nodes)
            {
                DSDialogueNodeLegacy node = CreateNode(data.DialogueType, mousePos, false, data.DialogueId, data.ActorName, data.ConversantName, data.AudioClip, data.ActorSprite, data.ConversantSprite, data.DialogueText, isPasting: true);

                Vector2 offsetFromCenter = data.Position - center;
                Vector2 newPos = mousePos + offsetFromCenter;

                node.SetPosition(new Rect(newPos, Vector2.zero));

                for (int i = 0; i < data.Ports.Count; i++)
                {
                    DSPortData portData = data.Ports[i];
                    string portName = data.PortNames[i];

                    Port choicePort = node.CreateChoicePort(portData.PortName, portData);
                    node.outputContainer.Add(choicePort);
                    node.RefreshExpandedState();
                }

                AddElement(node);
                AddToSelection(node);
            }
        }

        private Vector2 GetCopiedNodesCenter(DialogueCopyBuffer buffer)
        {
            Vector2 sum = Vector2.zero;

            foreach (NodeCopyData n in buffer.nodes)
                sum += n.Position;

            return sum / buffer.nodes.Count;
        }

        private void Copy()
        {
            List<DSDialogueNodeLegacy> selectedNodes = selection
                .OfType<DSDialogueNodeLegacy>()
                .ToList();

            copyBuffer = new DialogueCopyBuffer();

            foreach (DSDialogueNodeLegacy node in selectedNodes)
            {
                NodeCopyData data = new()
                {
                    DialogueType = node.DialogueType,
                    DialogueId = node.Id,
                    ActorName = node.Data.Actor.Title,
                    ActorSprite = node.Data.Actor.Sprite,
                    AudioClip = node.Data.AudioClip.Clip,
                    DialogueText = node.Data.Dialogue.Text,
                    Position = node.GetPosition().position,
                };

                foreach (KeyValuePair<Port, string> choice in node.Choices)
                {
                    data.Ports.Add((DSPortData)choice.Key.userData);
                    data.PortNames.Add(choice.Value);
                }

                copyBuffer.nodes.Add(data);
            }
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {

                List<DSDialogueNodeLegacy> deletedNodes = new();
                List<UnityEditor.Experimental.GraphView.Edge> deletedEdges = new();

                foreach (GraphElement element in selection)
                {
                    if (element is DSDialogueNodeLegacy node)
                    {
                        deletedNodes.Add(node);

                        foreach (KeyValuePair<Port, string> port in node.Choices)
                        {
                            foreach (Edge edge in port.Key.connections)
                            {
                                deletedEdges.Add(edge);
                            }
                        }

                        if (node.InputPort != null)
                        {
                            foreach (Edge inputEdge in node.InputPort.connections)
                            {
                                deletedEdges.Add(inputEdge);
                            }
                        }

                        node.Choices.Clear();
                    }
                }

                foreach (DSDialogueNodeLegacy node in deletedNodes)
                {
                    this.Nodes.Remove(node);
                    RemoveElement(node);
                }

                foreach (Edge edge in deletedEdges)
                {
                    RemoveElement(edge);
                }
            };
        }

        #endregion

        #region Save and Load

        public void Load()
        {
            if (this.Nodes.Count > 0)
            {
                int choice = EditorUtility.DisplayDialogComplex("Save", "Do you want to save current graph?", "Save", "Cancel", "No");
                if (choice == 0)
                {
                    Save();
                    LoadGraph();
                }
                else if (choice == 1)
                {
                    return;
                }
                else if (choice == 2)
                {
                    LoadGraph();
                }
            }
            else
            {
                LoadGraph();
            }
        }

        private void LoadGraph()
        {
            ClearGraph();
            DSGraphSO graph = DSIOUtility.PromptAndLoad();
            this.loadedGraph = graph;

            DSIOUtility.Load(graph, this);
        }

        public void Save()
        {
            if (this.Nodes.Count > 0)
            {
                string parentPath = "Assets";
                string folderName = "Conversations";
                DSIOUtility.CreateFolderIfNotExists(parentPath, folderName);

                if (loadedGraph != null)
                {
                    ClearGraph();
                }
                else
                {
                    ClearGraph();
                }
            }
        }

        public void ClearGraph()
        {
            foreach (DSNodeLegacy node in Nodes)
            {
                node.OnNodeIdChanged -= NodeIdChangedHandler;
            }

            this.NodeErrorCount = 0;
            CheckErrors();
            this.DeleteElements(this.edges);
            this.DeleteElements(this.nodes.ToList());
            Nodes.Clear();
            this.loadedGraph = null;
        }

        #endregion

        #region Mini Map

        private void AddMiniMap()
        {
            miniMap = new MiniMap();

            miniMap.SetPosition(new Rect(5, 30, 200, 140));
            Add(miniMap);
        }

        public void ToggleMiniMap() => miniMap.visible = !miniMap.visible;

        #endregion

        #region Creation

        public DSDialogueNodeLegacy CreateNode(DialogueType type, Vector2 position, bool isStartNode, string dialogueId, string actorName, string conversantName, AudioClip audioClip, Sprite actorSprite, Sprite conversantSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.DS{type}Node");

            DSDialogueNodeLegacy node = (DSDialogueNodeLegacy)Activator.CreateInstance(nodeType);

            DSDialogueText text = new("Dialogue Name", "Dialogue Description", dialogueText);
            DSActor actor = new(actorName, "Actor Description", "Actor Background", actorSprite);
            DSActor conversant = new(conversantName, "Conversant Description", "Conversant Background", conversantSprite);
            DSAudioClip clip = new("Audio Clip Name", "Audio Clip Description", audioClip);

            node.Initialize(position, type, text, actor, conversant, clip, isStartNode, isPasting, isLoading);
            node.Draw();
            this.Nodes.Add(node);

            node.OnNodeIdChanged += NodeIdChangedHandler;
            SetNodeError(node.Id, node, this.Nodes);

            node.OnEdgeDeleted += (edge) =>
            {
                if (this.Contains(edge))
                {
                    this.RemoveElement(edge);
                }
            };

            return node;
        }

        private IManipulator CreateNodeContextualMenu(DialogueType type, string actionTitle)
        {
            ContextualMenuManipulator manipulator = new(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent =>
                {
                    if (this.Nodes.Count == 0)
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

        #endregion

        #region Utils

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

        #region Error Handling

        private void NodeIdChangedHandler(DSNodeLegacy node, ChangeEvent<string> evt)
        {
            if (Nodes.Contains(node))
            {
                SetNodeError(evt.newValue, node, this.Nodes);
            }
        }

        private void SetNodeError(string title, DSNodeLegacy node, List<DSNodeLegacy> nodes)
        {
            if (CheckNodeNames(nodes, node, title))
            {
                node.SetErrorStyle(Color.red);
            }
            else
            {
                node.ResetStyle();
            }
        }

        private bool CheckNodeNames(List<DSNodeLegacy> nodes, DSNodeLegacy node, string title)
        {
            bool isError = false;
            int errorCount = 0;

            foreach (DSNodeLegacy item in nodes)
            {
                if (item.Id == title && node != item)
                {
                    errorCount++;
                    isError = true;
                }
            }

            NodeErrorCount = errorCount;
            CheckErrors();
            return isError;
        }

        private void CheckErrors()
        {
            if (NodeErrorCount <= 0)
            {
                return;
            }
        }

        #endregion

        #region Ports

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

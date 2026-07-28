using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    using System.Linq;
    using UnityEditor;

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
        public Sprite ActorSprite;
        public string DialogueText;
        public DialogueType DialogueType;
        public Vector2 Position;
        public List<DSPortData> Ports = new List<DSPortData>();
        public List<string> PortNames = new List<string>(); 
    }

    public class DSGraphView : GraphView
    {
        public List<DSNode> Nodes;
        private DSEditorWindow window;
        public int NodeErrorCount;
        private DSGraphSO loadedGraph;
        private MiniMap miniMap;
        private DialogueCopyBuffer copyBuffer;

        public Vector2 LastMousePosition { get; private set; }

        public DSGraphView(DSEditorWindow editorWindow)
        {
            NodeErrorCount = 0;
            Nodes = new List<DSNode>();
            this.window = editorWindow;

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

        private bool CanPaste(string data)
        {
            return !string.IsNullOrEmpty(data);
        }

        void PasteAtMouse()
        {
            if (copyBuffer == null || copyBuffer.nodes.Count == 0)
                return;

            Vector2 mousePos = LastMousePosition;
            Vector2 center = GetCopiedNodesCenter(copyBuffer);

            ClearSelection();

            foreach (var data in copyBuffer.nodes)
            {
                var node = CreateNode(data.DialogueType, mousePos, false, data.DialogueId, data.ActorName, data.AudioClip, data.ActorSprite, data.DialogueText, isPasting: true);

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

        Vector2 GetCopiedNodesCenter(DialogueCopyBuffer buffer)
        {
            Vector2 sum = Vector2.zero;

            foreach (var n in buffer.nodes)
                sum += n.Position;

            return sum / buffer.nodes.Count;
        }

        void Copy()
        {
            var selectedNodes = selection
                .OfType<DSNode>()
                .ToList();

            copyBuffer = new DialogueCopyBuffer();

            foreach (var node in selectedNodes)
            {
                NodeCopyData data = new NodeCopyData
                {
                    DialogueType = node.DialogueType,
                    DialogueId = node.DialogueId,
                    ActorName = node.ActorName,
                    ActorSprite = node.ActorSprite,
                    AudioClip = node.AudioClip,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position,
                };

                foreach (var choice in node.Choices)
                {
                    data.Ports.Add((DSPortData)choice.Key.userData);
                    data.PortNames.Add(choice.Value);
                }

                copyBuffer.nodes.Add(data);
            }
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) => {

                List<DSNode> deletedNodes = new List<DSNode>();
                List<UnityEditor.Experimental.GraphView.Edge> deletedEdges = new List<UnityEditor.Experimental.GraphView.Edge>();

                foreach (GraphElement element in selection)
                {
                    if (element is DSNode node)
                    {
                        deletedNodes.Add(node);

                        foreach (var port in node.Choices)
                        {
                            foreach (var edge in port.Key.connections)
                            {
                                deletedEdges.Add(edge);
                            }
                        }

                        if (node.InputPort != null)
                            foreach (var inputEdge in node.InputPort.connections)
                            {
                                deletedEdges.Add(inputEdge);
                            }

                        node.Choices.Clear();
                    }
                }

                foreach (var node in deletedNodes)
                {
                    this.Nodes.Remove(node);
                    RemoveElement(node);
                }

                foreach (var edge in deletedEdges)
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
            if (graph != null)
                window.SetFileName(graph.name);
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
                    DSIOUtility.SaveLoadedGraph(this, loadedGraph, window.GetFileName());
                    ClearGraph();
                }
                else
                {
                    DSIOUtility.Save(this, parentPath + "/" + folderName, window.GetFileName());
                    ClearGraph();
                }
            }
        }

        public void ClearGraph()
        {
            foreach (var node in Nodes)
            {
                node.OnDialogueIdChanged -= NodeIdChangedHandler;
            }

            this.NodeErrorCount = 0;
            CheckErrors();
            this.DeleteElements(this.edges);
            this.DeleteElements(this.nodes.ToList());
            Nodes.Clear();
            this.loadedGraph = null;
            window.EnableSaving();
        }

        #endregion

        #region Mini Map

        private void AddMiniMap()
        {
            miniMap = new MiniMap();

            miniMap.SetPosition(new Rect(5, 30, 200, 140));
            Add(miniMap);
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }

        #endregion

        #region Events

        #endregion

        #region Creation

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.DS{type}Node");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(position, isStartNode, dialogueId, actorName, audioClip, actorSprite, dialogueText, isPasting, isLoading);
            node.Draw();
            this.Nodes.Add(node);

            node.OnDialogueIdChanged += NodeIdChangedHandler;
            SetNodeError(node.DialogueId, node, this.Nodes);

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

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddManipulators()
        {
            SetupZoom(0.5f, 3f);

            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Single Choice Node"));
            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Multi, "Add Multi Choice Node"));

            this.AddManipulator(new ContentDragger());
        }

        #endregion

        #region Utils

        private void AddStyles()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, mousePosition - window.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        #endregion

        #region Error Handling

        private void NodeIdChangedHandler(DSNode node, ChangeEvent<string> evt)
        {
            if (Nodes.Contains(node))
            {
                SetNodeError(evt.newValue, node, this.Nodes);
            }
        }

        private void SetNodeError(string title, DSNode node, List<DSNode> nodes)
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

        private bool CheckNodeNames(List<DSNode> nodes, DSNode node, string title)
        {
            bool isError = false;
            int errorCount = 0;

            foreach (var item in nodes)
            {
                if (item.DialogueId == title && node != item)
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
                window.EnableSaving();
                return;
            }
            window.DisableSaving();
        }

        #endregion

        #region Ports

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

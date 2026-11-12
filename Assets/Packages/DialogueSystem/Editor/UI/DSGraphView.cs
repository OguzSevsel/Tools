using System;
using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphView : GraphView
    {
        #region Cut/Copy Variables

        private DSNodeData copiedNodeData;
        private const string ClipboardPrefix = "DSGraphViewClipboard:";
        private Vector2 _mouseLocation = new Vector2(0, 0);

        [Serializable]
        public class DSNodeConnections
        {
            public string InputId;
            public string OutputId;
        }

        [Serializable]
        public class DSNodeClipboard
        {
            public List<DSClipboardNode> Nodes = new();
            public List<DSNodeConnections> Connections = new();

            public DSNodeClipboard()
            {

            }
        }

        [Serializable]
        public class DSClipboardNode
        {
            public string Data;
            public Vector2 Position;
            public Vector2 Size;
        }

        #endregion

        private VisualTreeAsset nodeInspectorAsset;

        //Utils
        public DSConversation CurrentConversation { get; private set; }
        private DSNode selectedNode;

        //UI Elements
        private DSGraphTab tab;
        private IDSNodeInspector inspector;
        private MiniMap miniMap;
        public Button OpenSideBarButton;
        private Button miniMapButton;

        //Info
        private bool _showActor = false;
        private bool _showConversant = false;
        private bool _showDialogueText = false;
        private bool _showAudioClip = false;

        //Events
        public event Action<VisualElement, DSNode> OnInspectorOpened;
        public event Action OnInspectorClosed;
        public event Action OnSideBarOpened;
        public static event Action<DSConversation> OnNodeCreated;
        public static event Action<DSConversation> OnNodeDeleted;

        public DSGraphView(DSGraphTab tab, VisualTreeAsset nodeInspectorAsset)
        {
            this.tab = tab;
            this.nodeInspectorAsset = nodeInspectorAsset;
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
            this.AddStyleSheets("DialogueSystem/DialogueSystem.uss");
            AddGridBackground();
            AddMiniMap();
            AddManipulators();
            RegisterEvents();

            OpenSideBarButton = DSElementUtility.CreateButton("", HandleOpenSideBarButtonClicked);
            miniMapButton = DSElementUtility.CreateButton("Minimap", HandleOpenMiniMapButtonClicked);

            OpenSideBarButton.AddClasses("SideBarButton");
            this.contentContainer.Add(OpenSideBarButton);
            miniMapButton.AddClasses("MiniMapButton");
            this.contentContainer.Add(miniMapButton);
        }

        #region -----INITIALIZATION-----

        private void RegisterEvents()
        {
            RegisterCallback<MouseDownEvent>(HandleOnMouseDown, TrickleDown.TrickleDown);
            RegisterCallback<MouseMoveEvent>(HandleMouseMove, TrickleDown.TrickleDown);
            RegisterCallback<KeyDownEvent>(HandleOnKeyDown);
            DSDatabase.OnDataRemoved += HandleOnDataRemoved;
            
            canPasteSerializedData += HandleCanPaste;
            serializeGraphElements += HandleCutCopyOperation;
            unserializeAndPaste += HandlePasteOperation;

            deleteSelection += HandleDeleteSelection;
            DSDatabaseManager.Instance.DatabaseClosed += HandleDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened += HandleDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void HandleOnDataRemoved(DSData data)
        {
            if (CurrentConversation == null)
                return;

            foreach (var nodeData in CurrentConversation.Nodes)
            {
                if (data is DSActor)
                {
                    if (nodeData.ActorId == data.Guid)
                        nodeData.SetActorId("");

                    if (nodeData.ConversantId == data.Guid)
                        nodeData.SetConversantId("");
                }

                if (data is DSAudioClip && nodeData.AudioClipId == data.Guid)
                    nodeData.SetAudioClipId("");

                if (data is DSDialogueText && nodeData.DialogueId == data.Guid)
                    nodeData.SetDialogueId("");
            }

            if (selectedNode == null)
                return;

            inspector?.LoadFields(selectedNode.Data);
        }

        private void UnRegisterEvents()
        {
            UnregisterCallback<MouseDownEvent>(HandleOnMouseDown, TrickleDown.TrickleDown);
            UnregisterCallback<MouseMoveEvent>(HandleMouseMove, TrickleDown.TrickleDown);
            UnregisterCallback<KeyDownEvent>(HandleOnKeyDown);
            DSDatabase.OnDataRemoved -= HandleOnDataRemoved;

            canPasteSerializedData -= HandleCanPaste;
            serializeGraphElements -= HandleCutCopyOperation;
            unserializeAndPaste -= HandlePasteOperation;

            deleteSelection -= HandleDeleteSelection;
            DSDatabaseManager.Instance.DatabaseClosed -= HandleDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened -= HandleDatabaseOpened;
            
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void AddManipulators()
        {
            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Node"));
            this.AddManipulator(CreateShowContextMenu());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());
            SetupZoom(0.5f, 3f);
        }

        private IManipulator CreateNodeContextualMenu(DialogueType type, string actionTitle)
        {
            ContextualMenuManipulator manipulator = new(
                menuEvent => AddNodeAction(menuEvent, "Add Node"));

            return manipulator;
        }

        private IManipulator CreateShowContextMenu()
        {
            return new ContextualMenuManipulator(menuEvent =>
            {
                AddInfoAction(menuEvent,"Show/Actors",() => _showActor,() => _showActor = !_showActor);
                AddInfoAction(menuEvent,"Show/Conversant",() => _showConversant,() => _showConversant = !_showConversant);
                AddInfoAction(menuEvent,"Show/Audio Clip", () => _showAudioClip,() => _showAudioClip = !_showAudioClip);
                AddInfoAction(menuEvent,"Show/Dialogue Text",() => _showDialogueText,() => _showDialogueText = !_showDialogueText);
            });
        }

        private void AddNodeAction(ContextualMenuPopulateEvent menuEvent, string actionName)
        {
            menuEvent.menu.AppendAction(actionName, actionEvent =>
            {
                if (CurrentConversation != null)
                {
                    if (CurrentConversation.Nodes.Count == 0)
                    {
                        AddElement(CreateNode(DialogueType.Single, contentViewContainer.WorldToLocal(actionEvent.eventInfo.localMousePosition), isStartNode: true));
                        RefreshNodes();
                    }
                    else
                    {
                        AddElement(CreateNode(DialogueType.Single, contentViewContainer.WorldToLocal(actionEvent.eventInfo.localMousePosition), isStartNode: false));
                        RefreshNodes();
                    }
                }
            });
        }

        private void AddInfoAction(ContextualMenuPopulateEvent menuEvent,string actionName,Func<bool> getValue,Action toggleValue)
        {
            menuEvent.menu.AppendAction(
                actionName,
                action =>
                {
                    toggleValue();
                    RefreshNodes();
                },
                action => getValue()
                    ? DropdownMenuAction.Status.Checked
                    : DropdownMenuAction.Status.Normal
            );
        }

        #endregion

        #region -----SAVE AND LOAD-----

        public void Load(DSConversation conversation)
        {
            if (conversation == null || conversation == default) return;

            ClearGraph();

            foreach (var node in conversation.Nodes)
            {
                AddElement(LoadNode(node));
            }

            CurrentConversation = conversation;
            DSIOUtility.LoadConnections(conversation, this);
            RefreshNodes();
        }

        public void Save()
        {
            if (CurrentConversation != null)
            {
                DSIOUtility.Save(this, CurrentConversation);
            }

            ClearGraph();
            this.CurrentConversation = null;
        }

        #endregion

        #region -----CREATION-----

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, bool isPasting = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");
            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            DSNodeData data = new DSNodeData(position, type, "New Node", isStartNode);
            node.Initialize(data);
            CurrentConversation.AddToNodes(node.Data);

            OnNodeCreated?.Invoke(CurrentConversation);
            node.OnNodeSelected += HandleOnNodeSelected;

            return node;
        }

        public DSNode LoadNode(DSNodeData data)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(data);

            node.OnNodeSelected += HandleOnNodeSelected;

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        private void AddMiniMap()
        {
            miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(5, 30, 200, 200));
            Add(miniMap);
            ToggleMiniMap();
        }

        #endregion

        #region -----EVENT HANDLERS-----

        private void HandleDatabaseOpened(DSDatabase database)
        {
            var conversation = database.Conversations.FirstOrDefault();
            if (conversation != null)
            {
                Load(conversation);
                tab.sideBar.ChangeTitleLabel(conversation.Name);
            }

            _showDialogueText = true;
            RefreshNodes();
        }

        private void HandleDatabaseClosed(DSDatabase database)
        {
            Save();

            if (this.inspector != null)
            {
                this.inspector.GetVisualElement().RemoveFromHierarchy();
                this.inspector = null;
            }
        }

        private void HandleMouseMove(MouseMoveEvent evt)
        {
            _mouseLocation = evt.mousePosition;
        }

        private void HandleOnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            if (evt.target != this)
                return;
            
            foreach (var node in this.nodes.OfType<DSNode>().ToList())
            {
                node.Unselect(this);
            }

            CloseInspector();
        }

        private void HandleOnKeyDown(KeyDownEvent evt)
        {
            if (evt.ctrlKey && evt.shiftKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.C:
                        CopyNodeData();
                        evt.StopPropagation();
                        break;

                    case KeyCode.V:
                        PasteNodeData();
                        evt.StopPropagation();
                        break;
                }
            }
        }

        private void HandleOnNodeSelected(DSNode selectedNode)
        {
            this.selectedNode = selectedNode;

            if (selectedNode is DSNode node && inspector == null)
            {
                inspector = new DSNodeInspector(node.Data, this, nodeInspectorAsset);
            }

            OnInspectorOpened?.Invoke(this.inspector.GetVisualElement(), this.selectedNode);
            inspector.LoadFields(selectedNode.Data);
            RefreshNodes();
        }

        private void HandleOpenMiniMapButtonClicked()
        {
            ToggleMiniMap();
        }

        private void HandleOpenSideBarButtonClicked()
        {
            OnSideBarOpened?.Invoke();
            OpenSideBarButton.SetVisible(false);
        }

        #endregion

        #region -----UTILS-----

        public void SelectAll()
        {
            foreach (DSNode node in this.nodes.ToList().OfType<DSNode>())
            {
                AddToSelection(node);
            }

            foreach (Edge edge in this.edges)
            {
                AddToSelection(edge);
            }
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }

        public void RefreshNodes()
        {
            foreach (DSNode node in nodes.ToList().OfType<DSNode>())
            {
                node.SetInfo(
                    _showActor,
                    _showConversant,
                    _showAudioClip,
                    _showDialogueText
                );
            }
        }

        private void CloseInspector()
        {
            OnInspectorClosed?.Invoke();

            if (inspector is DSNodeInspector nodeInspector)
            {
                nodeInspector.RemoveFromHierarchy();
            }

            selectedNode = null;
        }

        private void ClearGraph()
        {
            CloseInspector();

            foreach (DSNode node in nodes.OfType<DSNode>().ToList())
            {
                node.OnNodeSelected -= HandleOnNodeSelected;
            }

            DeleteElements(edges);
            DeleteElements(nodes);
        }

        #endregion

        #region -----CUT/COPY/DUPLICATE/PASTE/DELETE-----

        private void HandleDeleteSelection(string operationName, AskUser askUser)
        {
            if (CurrentConversation == null) return;

            List<DSNode> selectedNodes = selection.OfType<DSNode>().ToList();

            foreach (DSNode node in selectedNodes)
            {
                CurrentConversation.Nodes.Remove(node.Data);
                node.OnNodeSelected -= HandleOnNodeSelected;
            }

            this.DeleteSelection();
            OnNodeDeleted?.Invoke(CurrentConversation);
        }

        private void CopyNodeData()
        {
            DSNode selectedNode = selection
                .OfType<DSNode>()
                .FirstOrDefault();

            if (selectedNode == null)
                return;

            copiedNodeData = JsonUtility.FromJson<DSNodeData>(
                JsonUtility.ToJson(selectedNode.Data)
            );
        }

        private void PasteNodeData()
        {
            if (copiedNodeData == null)
                return;

            DSNode selectedNode = selection
                .OfType<DSNode>()
                .FirstOrDefault();

            if (selectedNode == null)
                return;

            selectedNode.Data.SetAllIds(copiedNodeData.ActorId, copiedNodeData.ConversantId, copiedNodeData.AudioClipId, copiedNodeData.DialogueId);

            if (inspector != null)
            {
                inspector.LoadFields(selectedNode.Data);
            }

            RefreshNodes();
        }

        private bool HandleCanPaste(string data)
        {
            if (CurrentConversation == null ||
        string.IsNullOrEmpty(data) ||
        !data.StartsWith(ClipboardPrefix))
                return false;

            string json = data.Substring(ClipboardPrefix.Length);

            DSNodeClipboard clipboard =
                JsonUtility.FromJson<DSNodeClipboard>(json);

            return clipboard?.Nodes != null && clipboard.Nodes.Count > 0;
        }

        private Vector2 CalculateOffSet(List<DSClipboardNode> nodes)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (var node in nodes)
            {
                Vector2 position = node.Position;
                Vector2 size = node.Size;

                min.x = Mathf.Min(min.x, position.x);
                min.y = Mathf.Min(min.y, position.y);

                max.x = Mathf.Max(max.x, position.x + size.x);
                max.y = Mathf.Max(max.y, position.y + size.y);
            }

            Vector2 center = (min + max) * 0.5f;
            Vector2 target = contentViewContainer.WorldToLocal(_mouseLocation);
            Vector2 offset = target - center;
            return offset;
        }

        private void HandlePasteOperation(string operationName, string data)
        {
            if (string.IsNullOrEmpty(data) ||
        !data.StartsWith(ClipboardPrefix))
                return;

            data = data.Substring(ClipboardPrefix.Length);

            DSNodeClipboard clipboard =
                JsonUtility.FromJson<DSNodeClipboard>(data);

            if (clipboard?.Nodes == null || clipboard.Nodes.Count == 0)
                return;

            ClearSelection();
            Vector2 offset = CalculateOffSet(clipboard.Nodes);

            Dictionary<string, DSNode> pastedNodes = new();

            foreach (DSClipboardNode clipboardNode in clipboard.Nodes)
            {
                DSNodeData nodeData =
                    JsonUtility.FromJson<DSNodeData>(clipboardNode.Data);

                if (nodeData == null)
                    continue;

                string originalID = nodeData.Guid;

                nodeData.IsStartNode = false;

                Vector2 position = clipboardNode.Position + offset;

                DSNode node = CreateNode(nodeData.DialogueType, position, nodeData.IsStartNode, true);

                node.Data.SetAllIds(nodeData.ActorId, nodeData.ConversantId, nodeData.AudioClipId, nodeData.DialogueId);

                node.Data.SetName(nodeData.Name);

                pastedNodes.Add(originalID, node);
                AddElement(node);
                AddToSelection(node);
            }

            foreach (var connection in clipboard.Connections)
            {
                if (!pastedNodes.TryGetValue(connection.OutputId, out DSNode outputNode))
                    continue;

                if (!pastedNodes.TryGetValue(connection.InputId, out DSNode inputNode))
                    continue;

                Edge edge = outputNode.OutputPort.ConnectTo(inputNode.InputPort);

                AddElement(edge);
            }

            RefreshNodes();
        }

        private string HandleCutCopyOperation(IEnumerable<GraphElement> elements)
        {
            DSNodeClipboard clipboard = new();

            List<DSNode> selectedNodes = elements
                .OfType<DSNode>()
                .ToList();

            foreach (DSNode node in selectedNodes)
            {
                var clipboardNode = new DSClipboardNode
                {
                    Data = JsonUtility.ToJson(node.Data),
                    Position = node.GetPosition().position,
                    Size = node.GetPosition().size
                };

                clipboard.Nodes.Add(clipboardNode);
            }

            var copiedNodeIDs = selectedNodes.Select(node => node.Data.Guid).ToHashSet();

            var connections = edges.Where(edge =>
                    edge.output?.node is DSNode outputNode &&
                    edge.input?.node is DSNode inputNode &&
                    copiedNodeIDs.Contains(outputNode.Data.Guid) &&
                    copiedNodeIDs.Contains(inputNode.Data.Guid))
                    .Select(edge => new DSNodeConnections
                    {
                        OutputId = ((DSNode)edge.output.node).Data.Guid,
                        InputId = ((DSNode)edge.input.node).Data.Guid
                    }).ToList();

            clipboard.Connections = connections;

            return ClipboardPrefix + JsonUtility.ToJson(clipboard);
        }

        #endregion

        #region -----OVERRIDES-----

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

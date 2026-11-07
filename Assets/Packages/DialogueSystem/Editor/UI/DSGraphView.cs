using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
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
        public class DSNodeClipboard
        {
            public List<DSClipboardNode> Nodes = new();

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

        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
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

        private void HandleOpenMiniMapButtonClicked()
        {
            ToggleMiniMap();
        }

        private void HandleOpenSideBarButtonClicked()
        {
            OnSideBarOpened?.Invoke();
            OpenSideBarButton.SetVisible(false);
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
                    if (nodeData.Actor == data)
                        nodeData.Actor = null;

                    if (nodeData.Conversant == data)
                        nodeData.Conversant = null;
                }

                if (data is DSAudioClip && nodeData.AudioClip == data)
                    nodeData.AudioClip = null;

                if (data is DSDialogueText && nodeData.Dialogue == data)
                    nodeData.Dialogue = null;
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

            this.DeleteElements(this.edges);
            this.DeleteElements(this.nodes);

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
            this.DeleteElements(this.edges);
            this.DeleteElements(this.nodes);
            this.CurrentConversation = null;
        }

        #endregion

        #region -----CREATION-----

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, bool isPasting = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            DSNodeData data = new DSNodeData(position, type)
            {
                IsStartNode = isStartNode
            };

            node.Initialize(data);
            CurrentConversation.AddToNodes(node.Data);

            OnNodeCreated?.Invoke(CurrentConversation);

            node.OnNodeSelected += HandleOnNodeSelected;
            node.OnEdgeDeleted += HandleOnEdgeDeleted;

            return node;
        }

        public DSNode LoadNode(DSNodeData data)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(data);

            node.OnNodeSelected += HandleOnNodeSelected;
            node.OnEdgeDeleted += (edge) =>
            {
                if (this.Contains(edge))
                {
                    this.RemoveElement(edge);
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
                inspector = new DSNodeInspector(node.Data, this);
            }

            OnInspectorOpened?.Invoke(this.inspector.GetContainer(), this.selectedNode);
            inspector.LoadFields(selectedNode.Data);
        }

        private void HandleOnEdgeDeleted(Edge edge)
        {
            if (this.Contains(edge))
            {
                this.RemoveElement(edge);
            }
        }

        #endregion

        #region -----UTILS-----

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
                nodeInspector.Remove();
            }
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
                node.OnEdgeDeleted -= HandleOnEdgeDeleted;
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

            selectedNode.Data.Actor = copiedNodeData.Actor;
            selectedNode.Data.Conversant = copiedNodeData.Conversant;
            selectedNode.Data.Dialogue = copiedNodeData.Dialogue;
            selectedNode.Data.AudioClip = copiedNodeData.AudioClip;

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

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (var node in clipboard.Nodes)
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

            foreach (DSClipboardNode clipboardNode in clipboard.Nodes)
            {
                DSNodeData nodeData =
                    JsonUtility.FromJson<DSNodeData>(clipboardNode.Data);

                if (nodeData == null)
                    continue;

                nodeData.IsStartNode = false;

                Vector2 position = clipboardNode.Position + offset;

                DSNode node = CreateNode(nodeData.DialogueType, position, nodeData.IsStartNode,true);

                node.Data.SetAllFields(nodeData.Actor, nodeData.Conversant, nodeData.AudioClip, nodeData.Dialogue);

                AddElement(node);
                this.AddToSelection(node);
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
                clipboard.Nodes.Add(new DSClipboardNode
                {
                    Data = JsonUtility.ToJson(node.Data),
                    Position = node.GetPosition().position,
                    Size = node.GetPosition().size
                });
            }

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

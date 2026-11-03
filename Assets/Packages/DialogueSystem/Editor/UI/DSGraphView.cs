using System;
using System.Collections.Generic;
using System.Linq;
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
        public DSConversation CurrentConversation { get; private set; }

        //UI Elements
        private DSGraphTab tab;
        private IDSNodeInspector inspector;
        private MiniMap miniMap;

        public event Action<VisualElement, DSNode> OnInspectorOpened;
        public event Action OnInspectorClosed;

        //Utils
        private DSNode selectedNode;

        private bool _showActors = false;
        private bool _showConversant = false;
        private bool _showDialogueText = false;
        private bool _showAudioClip = false;

        public DSGraphView(DSGraphTab tab)
        {
            this.tab = tab;
            AddGridBackground();
            AddMiniMap();
            AddManipulators();
            AddStyles();
            RegisterEvents();
        }

        private void UnRegisterEvents()
        {
            UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            DSDatabaseManager.Instance.DatabaseClosed -= HandleDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened -= HandleDatabaseOpened;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        private void RegisterEvents()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            DSDatabaseManager.Instance.DatabaseClosed += HandleDatabaseClosed;
            DSDatabaseManager.Instance.DatabaseOpened += HandleDatabaseOpened;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void HandleDatabaseOpened(DSDatabase database)
        {
            var conversation = database.Conversations.FirstOrDefault();
            Load(conversation);
            _showDialogueText = true;
            tab.sideBar.ChangeTitleLabel(conversation.Name);
            RefreshNodes();
        }

        private void HandleDatabaseClosed(DSDatabase database)
        {
            Save();
        }

        #region Initialization

        private void AddManipulators()
        {
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(CreateNodeContextualMenu(DialogueType.Single, "Add Node"));
            this.AddManipulator(CreateShowContextMenu());
            SetupZoom(0.5f, 3f);
        }

        private IManipulator CreateShowContextMenu()
        {
            return new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction(
                    "Show/Actors",
                    action =>
                    {
                        _showActors = !_showActors;
                        RefreshNodes();
                    },
                    action => _showActors
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal
                );

                menuEvent.menu.AppendAction(
                    "Show/Conversant",
                    action =>
                    {
                        _showConversant = !_showConversant;
                        RefreshNodes();
                    },
                    action => _showConversant
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal
                );

                menuEvent.menu.AppendAction(
                    "Show/Audio Clip",
                    action =>
                    {
                        _showAudioClip = !_showAudioClip;
                        RefreshNodes();
                    },
                    action => _showAudioClip
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal
                );

                menuEvent.menu.AppendAction(
                    "Show/Dialogue Text",
                    action =>
                    {
                        _showDialogueText = !_showDialogueText;
                        RefreshNodes();
                    },
                    action => _showDialogueText
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal
                );
            });
        }

        private void RefreshNodes()
        {
            foreach (DSNode node in nodes.ToList().OfType<DSNode>())
            {
                node.SetInfo(
                    _showActors,
                    _showConversant,
                    _showAudioClip,
                    _showDialogueText
                );
            }
        }

        private IManipulator CreateNodeContextualMenu(DialogueType type, string actionTitle)
        {
            ContextualMenuManipulator manipulator = new(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent =>
                {
                    if (CurrentConversation != null)
                    {
                        DSActor actor = new DSActor("", "", "", null);
                        DSActor conversant = new DSActor("", "", "", null);
                        DSDialogueText dialogueText = new DSDialogueText("", "", "");
                        DSAudioClip audioClip = new DSAudioClip("", "", null);

                        if (CurrentConversation.Nodes.Count == 0)
                        {
                            AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: true, actor, conversant, dialogueText, audioClip));
                        }
                        else
                        {
                            AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), isStartNode: false, actor, conversant, dialogueText, audioClip));
                        }
                    }
                })
            );

            return manipulator;
        }

        public DSNode CreateNode(DialogueType type, Vector2 position, bool isStartNode, DSActor actor, DSActor conversant, DSDialogueText dialogueText, DSAudioClip audioClip, bool isPasting = false)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            DSNodeData data = new DSNodeData(position, type, dialogueText, actor, conversant, audioClip)
            {
                IsStartNode = isStartNode
            };

            node.Initialize(data);
            CurrentConversation.AddToNodes(node.Data);

            node.OnNodeSelect += HandleNodeSelected;
            node.OnEdgeDeleted += HandleEdgeDeleted;

            return node;
        }

        

        public DSNode LoadNode(DSNodeData data)
        {
            Type nodeType = Type.GetType($"Tools.DialogueSystem.UI.Elements.DSNode");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(data);

            node.OnNodeSelect += HandleNodeSelected;
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

        private void AddStyles()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
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
        
        #region Save and Load

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
        }

        public void Save()
        {
            DSIOUtility.Save(this, CurrentConversation);
            ClearGraph();
            this.CurrentConversation = null;
        }

        public void ClearGraph()
        {
            this.DeleteElements(this.edges);
            this.DeleteElements(this.nodes);
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

        private void HandleNodeSelected(DSNode selectedNode)
        {
            this.selectedNode = selectedNode;

            if (selectedNode is DSNode node && inspector == null)
            {
                inspector = new DSNodeInspector(node.Data);
            }

            OnInspectorOpened?.Invoke(this.inspector.GetContainer(), this.selectedNode);
            inspector.LoadFields(selectedNode.Data);
        }

        private void HandleEdgeDeleted(Edge edge)
        {
            if (this.Contains(edge))
            {
                this.RemoveElement(edge);
            }
        }

        #endregion

        #region Utils

        private void CloseInspector()
        {
            OnInspectorClosed?.Invoke();
            if (inspector is DSNodeInspector nodeInspector)
            {
                nodeInspector.Remove();
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

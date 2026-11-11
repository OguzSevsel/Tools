using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNode : Node
    {
        public Port InputPort { get; set; }
        public Port OutputPort { get; set; }
        public DSNodeData Data = null;
        public event Action<DSNode> OnNodeSelected;
        public event Action<Edge> OnEdgeDeleted;

        private VisualElement portContainer;
        private VisualElement infoContainer;
        private VisualElement container;
        private Label actorLabel;
        private Label conversantLabel;
        private Label audioClipLabel;
        private Label dialogueTextLabel;

        class LabelBinding
        {
            public Label Label;
            public DataBinding Binding;
            public BindingId Id;

            public LabelBinding(Label label, object dataSource, BindingId id, string path, BindingMode mode)
            {
                this.Label = label;
                this.Id = id;

                this.Binding = new DataBinding()
                {
                    dataSource = dataSource,
                    dataSourcePath = new PropertyPath(path),
                    bindingMode = mode
                };
            }

            public void SetBinding()
            {
                this.Label.SetBinding(Id, Binding);
            }
        }

        public virtual void Initialize(DSNodeData data)
        {
            this.Data = data;
            SetPosition(new Rect(data.Position, Vector2.zero));
            Draw();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            if (Data != null)
                Data.SetPosition(newPos.position);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public virtual void Draw()
        {
            container = new VisualElement();
            CreatePortContainer();
            CreateInfoContainer();
            CreateTitleLabel();
            CreateInfoLabels();
            //ContainerBGColors();
            AddTo(mainContainer, portContainer);
            AddTo(portContainer, container);
            AddTo(container, titleContainer, infoContainer);
            container.SetFlex(flexGrow: 1,flexDirection: FlexDirection.Column);
            CreatePorts();
            RefreshPorts();
        }

        private void CreatePorts()
        {
            if (!Data.IsStartNode)
                InputPort = CreatePort(portContainer, Direction.Input, 0);
            
            OutputPort = CreatePort(portContainer, Direction.Output, portContainer.childCount);
        }

        private void CreateTitleLabel()
        {
            Label label = DSElementUtility.CreateLabel("Node");
            label.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            label.SetTextSettings(fontStyle: FontStyle.Bold, alignment: TextAnchor.MiddleCenter);
            label.SetFlex(flexGrow: 1);
            titleContainer.Clear();
            titleContainer.SetBackgroundColor(DSColors.WindowDarkGrey2);
            titleContainer.SetBorder(DSColors.GridLightGrey4, 2);
            titleContainer.SetBorderRadius(5);
            titleContainer.Insert(0, label);
            titleContainer.SetFlex(flexGrow: 1);
        }

        private void CreateInfoLabels()
        {
            var infoLabels = CreateLabels(actorLabel, conversantLabel, audioClipLabel, dialogueTextLabel);
            (actorLabel, conversantLabel, audioClipLabel, dialogueTextLabel) = (infoLabels[0], infoLabels[1], infoLabels[2], infoLabels[3]);

            AddTo(infoContainer, actorLabel, conversantLabel, audioClipLabel, dialogueTextLabel);
            SetInfo(false, false, false, false);
        }

        private void CreateInfoContainer()
        {
            infoContainer = new VisualElement();
            infoContainer.SetFlex(flexDirection: FlexDirection.Column, flexGrow: 1);
            infoContainer.SetAlignment(alignContent: Align.Center, alignItems: Align.Center);
        }

        private void CreatePortContainer()
        {
            portContainer = new VisualElement();
            portContainer.style.flexDirection = FlexDirection.Row;
            portContainer.style.alignItems = Align.Center;
            portContainer.SetBackgroundColor(DSColors.WindowDarkGrey2);
        }

        private void ContainerBGColors()
        {
            mainContainer.SetBackgroundColor(Color.red);
            portContainer.SetBackgroundColor(Color.blue);
            titleContainer.SetBackgroundColor(Color.green);
        }

        #region Creation

        private Label[] CreateLabels(params Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = DSElementUtility.CreateLabel("");
                labels[i].style.whiteSpace = WhiteSpace.Normal;
                labels[i].SetWidth(maxWidth: 250);
            }

            return labels;
        }

        private void AddTo(VisualElement container, params VisualElement[] elements)
        {
            foreach (var element in elements)
            {
                container.Add(element);
            }
        }

        private Port CreatePort(VisualElement container, Direction direction, int index)
        {
            Port port = this.CreatePort("", Orientation.Horizontal, direction, Port.Capacity.Multi);
            DSPortData data = new DSPortData(this.Data.Guid, "port");
            port.userData = data;
            container.Insert(index, port);
            Label label = port.Q<Label>("type");
            label.SetMargins(0);
            port.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            return port;
        }

        internal void SetInfo(bool showActor, bool showConversant, bool showAudioClip, bool showDialogueText)
        {
            if (this.Data == null)
            {
                return;
            }

            Dictionary<LabelBinding, bool> bindings = new Dictionary<LabelBinding, bool>();

            if (this.Data.Actor != null)
            {
                bindings.Add(new LabelBinding(actorLabel, this.Data, "text", "Actor.Name", BindingMode.ToTarget), showActor);
            }

            if (this.Data.Conversant != null)
            {
                bindings.Add(new LabelBinding(conversantLabel, this.Data, "text", "Conversant.Name", BindingMode.ToTarget), showConversant);
            }

            if (this.Data.AudioClip != null)
            {
                bindings.Add(new LabelBinding(audioClipLabel, this.Data, "text", "AudioClip.Name", BindingMode.ToTarget), showAudioClip);
            }
            
            if (this.Data.Dialogue != null)
            {
                bindings.Add(new LabelBinding(dialogueTextLabel, this.Data, "text", nameof(DSNodeData.DialogueText), BindingMode.ToTarget), showDialogueText);
            }

            SetInfoVisible(new Dictionary<Label, bool> 
            { 
                { actorLabel, showActor },
                { conversantLabel, showConversant },
                { dialogueTextLabel, showDialogueText },
                { audioClipLabel, showAudioClip } 
            });

            SetInfoBindings(bindings);

            void SetInfoVisible(Dictionary<Label, bool> infoLabels)
            {
                foreach (var kvp in infoLabels)
                {
                    Label label = kvp.Key;
                    bool isVisible = kvp.Value;
                    label.SetVisible(isVisible);
                }
            }
            
            void SetInfoBindings(Dictionary<LabelBinding, bool> bindings)
            {
                foreach (var kvp in bindings)
                {
                    LabelBinding binding = kvp.Key;
                    binding.Label.ClearBinding(binding.Id);
                    bool isVisible = kvp.Value;

                    if (isVisible)
                    {
                        binding.SetBinding();
                    }
                }
            }
        }

        //public TextField CreateTextField(string title = null, string label = null, bool isMultiLine = false, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    TextField textField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateTextField(title, label, isMultiLine, onValueChanged: onValueChanged);

        //    if (isMultiLine)
        //    {
        //        textField.AddClasses("ds-node__text-field",
        //      "ds-node__quote-text-field");
        //    }
        //    else
        //    {
        //        textField.AddClasses("ds-node__text-field",
        //        "ds-node__filename-text-field",
        //        "ds-node__text-field__hidden");
        //    }

        //    customDataContainer?.Add(textField);

        //    return textField;
        //}

        //public Label CreateLabel(string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    Label label = Tools.DialogueSystem.Utilities.DSElementUtility.CreateLabel(text, onValueChanged);

        //    customDataContainer?.Add(label);

        //    label.AddClasses("ds-node__text-field",
        //      "ds-node__quote-text-field");

        //    return label;
        //}

        //public DropdownField CreateDropdown(List<string> choices, string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    DropdownField dropdown = Tools.DialogueSystem.Utilities.DSElementUtility.CreateDropdown(text, choices, onValueChanged);

        //    customDataContainer?.Add(dropdown);

        //    return dropdown;
        //}

        //public ObjectField CreateObjectField(string title, string label, Type type, VisualElement customDataContainer, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        //{
        //    ObjectField objectField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateObjectField(title, type, label, onValueChanged);
        //    customDataContainer.Add(objectField);
        //    return objectField;
        //}

        #endregion
    }
}

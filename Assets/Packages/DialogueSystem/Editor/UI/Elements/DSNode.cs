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

        private VisualElement portContainer;
        private VisualElement infoContainer;
        private VisualElement container;
        
        private Label titleLabel;
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
            titleLabel = DSElementUtility.CreateLabel("");
            titleLabel.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            titleLabel.SetTextWeight(FontStyle.Bold);
            titleLabel.SetTextAlign(TextAnchor.MiddleCenter);
            titleLabel.SetOverFlow(TextOverflow.Ellipsis, Overflow.Hidden, WhiteSpace.NoWrap);
            titleLabel.SetFlex(flexGrow: 1);
            
            titleContainer.Clear();
            titleContainer.SetBackgroundColor(DSColors.WindowDarkGrey2);
            titleContainer.SetBorder(DSColors.GridLightGrey4, 2);
            titleContainer.SetBorderRadius(5);
            titleContainer.SetWidth(maxWidth: 200);
            titleContainer.Insert(0, titleLabel);
            titleContainer.SetFlex(flexGrow: 1);

            if (!this.Data.IsStartNode)
            {
                var binding = new LabelBinding(titleLabel, this.Data, "text", nameof(this.Data.Name), BindingMode.ToTarget);
                binding.Label.ClearBinding(binding.Id);
                binding.SetBinding();
                return;
            }

            titleContainer.SetBackgroundColor(Color.green);
            titleLabel.text = "Start Node";
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

        #region Creation

        private Label[] CreateLabels(params Label[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = DSElementUtility.CreateLabel("");
                labels[i].SetWidth(maxWidth: 250);
                labels[i].SetHeight(maxHeight: 150);
                labels[i].SetOverFlow(TextOverflow.Ellipsis, Overflow.Hidden, WhiteSpace.Normal);
                labels[i].SetTextAlign(TextAnchor.MiddleCenter);
                labels[i].SetTextWeight(FontStyle.BoldAndItalic);
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

        private T GetFromDatabase<T>(string id) where T : DSData
        {
            return DSDatabaseManager.Instance.Current.Get<T>(id);
        }

        internal void SetInfo(bool showActor, bool showConversant, bool showAudioClip, bool showDialogueText)
        {
            if (this.Data == null)
            {
                return;
            }

            Dictionary<LabelBinding, bool> bindings = new Dictionary<LabelBinding, bool>();

            var actor = GetFromDatabase<DSActor>(this.Data.ActorId);
            var conversant = GetFromDatabase<DSActor>(this.Data.ConversantId);
            var audioClip = GetFromDatabase<DSAudioClip>(this.Data.AudioClipId);
            var dialogue = GetFromDatabase<DSDialogueText>(this.Data.DialogueId);

            if (actor != null)
            {
                bindings.Add(new LabelBinding(actorLabel, actor, "text", nameof(actor.Name), BindingMode.ToTarget), showActor);
            }

            if(conversant != null)
            {
                bindings.Add(new LabelBinding(conversantLabel, conversant, "text", nameof(conversant.Name), BindingMode.ToTarget), showConversant);
            }

            if (audioClip != null)
            {
                bindings.Add(new LabelBinding(audioClipLabel, audioClip, "text", nameof(audioClip.Name), BindingMode.ToTarget), showAudioClip);
            }
            
            if (dialogue != null)
            {
                bindings.Add(new LabelBinding(dialogueTextLabel, dialogue, "text", nameof(dialogue.Text), BindingMode.ToTarget), showDialogueText);
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

        #endregion
    }
}

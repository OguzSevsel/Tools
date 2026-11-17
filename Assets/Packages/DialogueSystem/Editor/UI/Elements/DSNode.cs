using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using Unity.Properties;
using UnityEditor;
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

        #region Info UI Elements

        private VisualElement actorInfoContainer;
        private VisualElement conversantInfoContainer;
        private VisualElement audioClipInfoContainer;
        private VisualElement dialogueInfoContainer;
        
        private Image actorInfoImage;
        private Image conversantInfoImage;
        private Image audioClipInfoImage;
        private Image dialogueInfoImage;

        private Label actorInfoLabel;
        private Label conversantInfoLabel;
        private Label audioClipInfoLabel;
        private Label dialogueTextInfoLabel;

        #endregion

        class ElementBinding
        {
            public VisualElement Element;
            public DataBinding Binding;
            public BindingId Id;

            public ElementBinding(VisualElement element, object dataSource, BindingId id, string path, BindingMode mode)
            {
                this.Element = element;
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
                this.Element.SetBinding(Id, Binding);
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
            CreateTitle();
            StyleTitleContainer();

            if (!this.Data.IsStartNode)
            {
                var binding = new ElementBinding(titleLabel, this.Data, "text", nameof(this.Data.Name), BindingMode.ToTarget);
                binding.Element.ClearBinding(binding.Id);
                binding.SetBinding();
                return;
            }

            titleContainer.SetBackgroundColor(DSColors.Green);
            titleLabel.text = "Start Node";
        }

        #region -----STYLING-----

        private void SetImage(Image image, string path)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            image.image = texture;
        }

        private void SetInfoContainerFlex(int flexGrow, FlexDirection flexDirection, params VisualElement[] elements)
        {
            foreach (var element in elements)
            {
                element.SetFlex(flexGrow: flexGrow, flexDirection: flexDirection);
            }
        }

        private void SetInfoImageSize(float size, float padding, params VisualElement[] elements)
        {
            foreach (var element in elements)
            {
                element.SetWidth(maxWidth: 25);
                element.SetHeight(maxHeight: 25);
                element.SetPaddings(paddingRight: 5);
            }
        }

        private void StyleTitleContainer()
        {
            titleContainer.Clear();
            titleContainer.SetBackgroundColor(DSColors.Transparent);
            titleContainer.SetBorder(DSColors.GridLightGrey4, 2);
            titleContainer.SetBorderRadius(5);
            titleContainer.SetWidth(maxWidth: 200);
            titleContainer.Insert(0, titleLabel);
            titleContainer.SetFlex(flexGrow: 1);
        }

        public void SetEndNode(bool isEndNode)
        {
            this.Data.IsEndNode = isEndNode;

            if (isEndNode)
            {
                titleLabel.ClearBindings();
                titleLabel.SetText("End Node");
                titleContainer.SetBackgroundColor(DSColors.Red);
                return;
            }

            var binding = new ElementBinding(titleLabel, this.Data, "text", nameof(this.Data.Name), BindingMode.ToTarget);
            binding.Element.ClearBinding(binding.Id);
            binding.SetBinding();
            titleContainer.SetBackgroundColor(DSColors.Transparent);
        }

        #endregion

        #region -----CREATION-----

        private Image[] CreateImages(params Image[] images)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i] = new Image();
            }

            return images;
        }

        private VisualElement[] CreateVisualElements(params VisualElement[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new VisualElement();
            }

            return elements;
        }

        private void CreateInfoContainer()
        {
            infoContainer = new VisualElement();
            infoContainer.SetFlex(flexDirection: FlexDirection.Column, flexGrow: 1);
            infoContainer.SetAlignment(alignContent: Align.Center, alignItems: Align.FlexStart);
        }

        private void CreatePortContainer()
        {
            portContainer = new VisualElement();
            portContainer.style.flexDirection = FlexDirection.Row;
            portContainer.style.alignItems = Align.Center;
            portContainer.SetBackgroundColor(DSColors.WindowDarkGrey2);
        }

        private void CreateTitle()
        {
            titleLabel = DSElementUtility.CreateLabel("");
            titleLabel.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            titleLabel.SetTextWeight(FontStyle.Bold);
            titleLabel.SetTextAlign(TextAnchor.MiddleCenter);
            titleLabel.SetOverFlow(TextOverflow.Ellipsis, Overflow.Hidden, WhiteSpace.NoWrap);
            titleLabel.SetFlex(flexGrow: 1);
        }

        private void CreateInfoLabels()
        {
            var infoLabels = CreateLabels(actorInfoLabel, conversantInfoLabel, audioClipInfoLabel, dialogueTextInfoLabel);
            (actorInfoLabel, conversantInfoLabel, audioClipInfoLabel, dialogueTextInfoLabel) = (infoLabels[0], infoLabels[1], infoLabels[2], infoLabels[3]);

            var infoContainers = CreateVisualElements(actorInfoContainer, conversantInfoContainer, audioClipInfoContainer, dialogueInfoContainer);
            (actorInfoContainer, conversantInfoContainer, audioClipInfoContainer, dialogueInfoContainer) = (infoContainers[0], infoContainers[1], infoContainers[2], infoContainers[3]);

            var infoImages = CreateImages(actorInfoImage, conversantInfoImage, audioClipInfoImage, dialogueInfoImage);
            (actorInfoImage, conversantInfoImage, audioClipInfoImage, dialogueInfoImage) = (infoImages[0], infoImages[1], infoImages[2], infoImages[3]);

            SetImage(actorInfoImage, "Assets/Packages/DialogueSystem/Editor/Icons/actor.png");
            SetImage(conversantInfoImage, "Assets/Packages/DialogueSystem/Editor/Icons/actor.png");
            SetImage(audioClipInfoImage, "Assets/Packages/DialogueSystem/Editor/Icons/audio.png");
            SetImage(dialogueInfoImage, "Assets/Packages/DialogueSystem/Editor/Icons/dialogue.png");

            SetInfoImageSize(25, 5, audioClipInfoImage, dialogueInfoImage, actorInfoImage, conversantInfoImage);

            var actorLabel = new Label("Actor: ");
            var conversantLabel = new Label("Conversant: ");
            var audioClipLabel = new Label("Audio Clip: ");

            SetInfoContainerFlex(1, FlexDirection.Row, actorInfoContainer, conversantInfoContainer, audioClipInfoContainer, dialogueInfoContainer);

            AddTo(actorInfoContainer, actorInfoImage, actorLabel, actorInfoLabel);
            AddTo(conversantInfoContainer, conversantInfoImage, conversantLabel, conversantInfoLabel);
            AddTo(audioClipInfoContainer, audioClipInfoImage, audioClipLabel, audioClipInfoLabel);
            AddTo(dialogueInfoContainer, dialogueInfoImage, dialogueTextInfoLabel);
            AddTo(infoContainer, actorInfoContainer, conversantInfoContainer, audioClipInfoContainer, dialogueInfoContainer);

            SetInfo(false, false, false, false);
        }

        private void SetColors(Color color, params VisualElement[] elements)
        {
            foreach (var element in elements)
            {
                element.SetColor(color);
            }
        }

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

            Dictionary<ElementBinding, bool> bindings = new Dictionary<ElementBinding, bool>();

            var actor = GetFromDatabase<DSActor>(this.Data.ActorId);
            var conversant = GetFromDatabase<DSActor>(this.Data.ConversantId);
            var audioClip = GetFromDatabase<DSAudioClip>(this.Data.AudioClipId);
            var dialogue = GetFromDatabase<DSDialogueText>(this.Data.DialogueId);

            if (actor != null)
            {
                bindings.Add(new ElementBinding(actorInfoLabel, actor, "text", nameof(actor.Name), BindingMode.ToTarget), showActor);
                bindings.Add(new ElementBinding(actorInfoImage, actor, "sprite", nameof(actor.Sprite), BindingMode.ToTarget),showActor);
            }

            if(conversant != null)
            {
                bindings.Add(new ElementBinding(conversantInfoLabel, conversant, "text", nameof(conversant.Name), BindingMode.ToTarget), showConversant);
                bindings.Add(new ElementBinding(conversantInfoImage, conversant, "sprite", nameof(conversant.Sprite), BindingMode.ToTarget), showConversant);
            }

            if (audioClip != null)
            {
                bindings.Add(new ElementBinding(audioClipInfoLabel, audioClip, "text", nameof(audioClip.Name), BindingMode.ToTarget), showAudioClip);
            }
            
            if (dialogue != null)
            {
                bindings.Add(new ElementBinding(dialogueTextInfoLabel, dialogue, "text", nameof(dialogue.Text), BindingMode.ToTarget), showDialogueText);
            }

            SetInfoVisible(new Dictionary<VisualElement, bool> 
            { 
                { actorInfoContainer, showActor },
                { conversantInfoContainer, showConversant },
                { dialogueInfoContainer, showDialogueText },
                { audioClipInfoContainer, showAudioClip } 
            });

            SetInfoBindings(bindings);

            void SetInfoVisible(Dictionary<VisualElement, bool> infoLabels)
            {
                foreach (var kvp in infoLabels)
                {
                    VisualElement element = kvp.Key;
                    bool isVisible = kvp.Value;
                    element.SetVisible(isVisible);
                }
            }
            
            void SetInfoBindings(Dictionary<ElementBinding, bool> bindings)
            {
                foreach (var kvp in bindings)
                {
                    ElementBinding binding = kvp.Key;
                    binding.Element.ClearBinding(binding.Id);
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

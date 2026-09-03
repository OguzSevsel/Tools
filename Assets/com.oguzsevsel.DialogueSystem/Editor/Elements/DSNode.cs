using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;
using Tools.DialogueSystem.Utilities;

namespace Tools.DialogueSystem.Elements
{
    public class DSNode : Node
    {
        private TextField dialogueIdTextField;
        private TextField actorNameField;
        private ObjectField spriteField;
        private ObjectField audioClipField;
        private TextField dialogueTextField;

        public string DialogueId { get; set; } = "Dialogue ID";
        public Dictionary<Port, string> Choices { get; set; }
        public string ActorName { get; set; } = "Actor Name";
        public string DialogueText { get; set; } = "Dialogue Text";
        public DialogueType DialogueType { get; set; }
        public AudioClip AudioClip { get; set; } = null;
        public Sprite ActorSprite { get; set; } = null;
        public Port InputPort { get; set; }

        public bool isStartNode = false;
        public bool isPasting = false;
        public bool isLoading = false;
        private Color defaultBackgroundColor;
        public event Action<DSNode, ChangeEvent<string>> OnDialogueIdChanged;
        public event Action<Edge> OnEdgeDeleted;


        public virtual void Initialize(Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Choices = new Dictionary<Port, string>();
            this.isStartNode = isStartNode;
            this.isLoading = isLoading;
            this.isPasting = isPasting;

            LoadFields(dialogueId, dialogueText, actorName, actorSprite, audioClip);
            SetPosition(new Rect(position, Vector2.zero));

            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {
            CreateDialogueIdField();

            if (!this.isStartNode)
            {
                InputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
                inputContainer.Add(InputPort);
            }

            if (!isLoading && !isPasting)
            {
                Port port = CreateChoicePort("Next Dialogue", new DSPortData("", this.DialogueId, "Next Dialogue"));
                this.outputContainer.Add(port);
                RefreshExpandedState();
            }

            CreateCustomDataContainer();
            LoadFields(DialogueId, DialogueText, ActorName, ActorSprite, AudioClip);
            RefreshExpandedState();
        }
        
        private void OnIdChanged(ChangeEvent<string> evt)
        {
            this.DialogueId = evt.newValue;

            var keys = new List<Port>(Choices.Keys);
            foreach (var key in keys)
            {
                Choices[key] = evt.newValue;
            }

            OnDialogueIdChanged?.Invoke(this, evt);
        }

        #region Creations

        private void CreateDialogueIdField()
        {
            dialogueIdTextField = DSElementUtility.CreateTextField(DialogueId, onValueChanged: OnIdChanged);

            dialogueIdTextField.AddClasses("ds-node__text-field",
                "ds-node__filename-text-field",
                "ds-node__text-field__hidden");

            titleContainer.Insert(0, dialogueIdTextField);
        }

        private void CreateActorNameField(VisualElement customDataContainer)
        {
            actorNameField = DSElementUtility.CreateTextField(ActorName, onValueChanged: (evt) => ActorName = evt.newValue);

            actorNameField.AddClasses("ds-node__text-field",
                "ds-node__filename-text-field",
                "ds-node__text-field__hidden");

            customDataContainer.Add(actorNameField);
        }

        private void CreateActorSpriteField(VisualElement customDataContainer)
        {
            spriteField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateObjectField("Actor Sprite", typeof(Sprite), "Sprite", (evt) =>
            {
                ActorSprite = evt.newValue as Sprite;
            });

            customDataContainer.Add(spriteField);
        }

        private void CreateAudioClipField(VisualElement customDataContainer)
        {
            audioClipField = DSElementUtility.CreateObjectField("Dialogue Audio", typeof(AudioClip), "Audio Clip", (evt) =>
            {
                AudioClip = evt.newValue as AudioClip;
            });
            customDataContainer.Add(audioClipField);
        }

        private void CreateDialogueTextField(Foldout textFoldout)
        {
            dialogueTextField = DSElementUtility.CreateTextArea(DialogueText, onValueChanged: DialogueTextChangedHandler);
            dialogueTextField.RegisterCallback<KeyDownEvent>(DialogueTextFieldKeyDownHandler, TrickleDown.TrickleDown);

            dialogueTextField.AddClasses("ds-node__text-field",
               "ds-node__quote-text-field");

            textFoldout.Add(dialogueTextField);
        }

        private void InsertText(TextField field, string text)
        {
            int cursor = field.cursorIndex;
            int select = field.selectIndex;

            int start = Mathf.Min(cursor, select);
            int end = Mathf.Max(cursor, select);

            string value = field.value ?? "";

            field.value =
                value.Substring(0, start) +
                text +
                value.Substring(end);

            //field.cursorIndex = start + text.Length;
            //field.selectIndex = field.cursorIndex;
        }

        private void DialogueTextChangedHandler(ChangeEvent<string> evt)
        {
            DialogueText = evt.newValue;
        }

        public List<string> GetTextsBetweenCharacters(string text, char startCharacter, char endCharacter)
        {
            List<int> indexOfStarts = new List<int>();
            List<int> indexOfEnds = new List<int>();
            List<string> values = new List<string>();

            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];

                if (character == startCharacter)
                {
                    indexOfStarts.Add(i + 1);
                }

                if (character == endCharacter)
                {
                    indexOfEnds.Add(i - 1);
                }
            }

            if (indexOfStarts.Count > 0 && indexOfEnds.Count > 0)
            {
                for (int i = 0; i < indexOfStarts.Count; i++)
                {
                    int startIndex = indexOfStarts[i];
                    int endIndex = indexOfEnds[i];

                    string valueText = text.Substring(startIndex, endIndex - startIndex + 1);
                    values.Add(valueText);
                }
            }

            return values;
        }

        private void DialogueTextFieldKeyDownHandler(KeyDownEvent evt)
        {
            if (dialogueTextField.value.Contains("{"))
            {
                if (evt.keyCode == KeyCode.Backspace)
                {
                    List<int> indexesOfLeft = new List<int>();
                    List<int> indexesOfRight = new List<int>();

                    for (int i = 0; i < dialogueTextField.value.Length; i++)
                    {
                        char character = dialogueTextField.value[i];

                        if (character == '{')
                        {
                            indexesOfLeft.Add(i);
                        }

                        if (character == '}')
                        {
                            indexesOfRight.Add(i);
                        }
                    }

                    for (int i = 0; i < indexesOfLeft.Count; i++)
                    {
                        int startIndex = indexesOfLeft[i];
                        int endIndex = indexesOfRight[i];

                        if (dialogueTextField.cursorIndex - 1 <= endIndex && dialogueTextField.cursorIndex + 1 >= startIndex)
                        {
                            if (startIndex > -1 && endIndex > -1)
                            {
                                dialogueTextField.value = dialogueTextField.value.Remove(startIndex, endIndex - startIndex + 1);
                                DialogueText = dialogueTextField.value;
                                dialogueTextField.cursorIndex = startIndex;
                                dialogueTextField.selectIndex = dialogueTextField.cursorIndex;
                                evt.StopImmediatePropagation();
                            }
                        }
                    }
                }
            }

            if (evt.character == '{')
            {
                InsertText(dialogueTextField, "}");
            }
        }

        private void CreateCustomDataContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            CreateActorNameField(customDataContainer);
            CreateActorSpriteField(customDataContainer);
            CreateAudioClipField(customDataContainer);

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text", true);

            CreateDialogueTextField(textFoldout);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }

        public Port CreateChoicePort(string choice, DSPortData portData)
        {
            Port choicePort = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            choicePort.userData = portData;
            this.Choices.Add(choicePort, portData.PortName);
            Button deletePortButton = null;

            if (DialogueType != DialogueType.Single)
            {
                deletePortButton = DSElementUtility.CreateButton("X", () =>
                {
                    if (this.Choices.Count > 1)
                    {
                        List<Edge> edges = choicePort.connections.ToList();

                        Choices.Remove(choicePort);
                        outputContainer.Remove(choicePort);
                        
                        foreach (Edge edge in edges)
                        {
                            edge.input?.Disconnect(edge);
                            edge.output?.Disconnect(edge);
                            
                            OnEdgeDeleted?.Invoke(edge);
                        }

                        RefreshExpandedState();
                    }
                });
            }

            TextField choiceTextField = DSElementUtility.CreateTextField(choice, onValueChanged: (evt) =>
            {
                DSPortData data = (DSPortData)choicePort.userData;
                data.PortName = evt.newValue;
            });

            choiceTextField.AddClasses("ds-node__text-field",
                "ds-node__choice-text-field",
                "ds-node__text-field__hidden");

            choicePort.Add(choiceTextField);

            if (deletePortButton != null)
            {
                choicePort.Add(deletePortButton);
            }

            return choicePort;
        }

        #endregion

        #region Utils

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public void LoadFields(string id, string dialogueText, string actorName, Sprite actorSprite, AudioClip audioClip)
        {
            if (dialogueIdTextField != null)
            {
                this.dialogueIdTextField.value = id;
                this.dialogueTextField.value = dialogueText;
                this.actorNameField.value = actorName;
                this.spriteField.value = actorSprite;
                this.audioClipField.value = audioClip;
            }

            this.DialogueId = id;
            this.ActorName = actorName;
            this.DialogueText = dialogueText;
            this.ActorSprite = actorSprite;
            this.AudioClip = audioClip;
        }

        #endregion
    } 
}

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Tools.DialogueSystem.Utilities;

namespace Tools.DialogueSystem.Elements
{
    public class DSDialogueNode : DSNode
    {
        //Fields
        private TextField dialogueIdTextField;
        private TextField actorNameField;
        private ObjectField spriteField;
        private ObjectField audioClipField;
        private TextField dialogueTextField;

        //Values
        public string ActorName { get; set; } = "Actor Name";
        public string DialogueText { get; set; } = "Dialogue Text";
        public AudioClip AudioClip { get; set; } = null;
        public Sprite ActorSprite { get; set; } = null;

        //Events

        #region Initialize and Draw

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
            dialogueIdTextField = CreateTextField(Id, null, false, null, OnIdChanged);
            titleContainer.Insert(0, dialogueIdTextField);
            dialogueIdTextField.RegisterCallback<KeyDownEvent>(DialogueTextFieldKeyDownHandler, TrickleDown.TrickleDown);

            if (!this.isStartNode)
            {
                InputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
                inputContainer.Add(InputPort);
            }
            
            if (!isLoading && !isPasting)
            {
                Port port = CreateChoicePort("Next Dialogue", new DSPortData("", this.Id, "Next Dialogue"));
                this.outputContainer.Add(port);
                RefreshExpandedState();
            }

            CreateCustomDataContainer();
            LoadFields(Id, DialogueText, ActorName, ActorSprite, AudioClip);
            RefreshExpandedState();
        }

        private void CreateCustomDataContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            actorNameField = CreateTextField(ActorName, null, false, customDataContainer, onValueChanged: (evt) => ActorName = evt.newValue);
            spriteField = CreateObjectField("Actor Sprite", "Sprite", typeof(Sprite), customDataContainer, (evt) => ActorSprite = evt.newValue as Sprite);
            audioClipField = CreateObjectField("Dialogue Audio", "Audio Clip", typeof(AudioClip), customDataContainer, (evt) => AudioClip = evt.newValue as AudioClip);

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text", false);
            dialogueTextField = CreateTextField(DialogueText, null, true, textFoldout, DialogueTextChangedHandler);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }

        #endregion

        #region Events

        private void DialogueTextChangedHandler(ChangeEvent<string> evt)
        {
            DialogueText = evt.newValue;
        }

        #endregion

        #region Utils

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

            this.Id = id;
            this.ActorName = actorName;
            this.DialogueText = dialogueText;
            this.ActorSprite = actorSprite;
            this.AudioClip = audioClip;
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

        #endregion
    } 
}

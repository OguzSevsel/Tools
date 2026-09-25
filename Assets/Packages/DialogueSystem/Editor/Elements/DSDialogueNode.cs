using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Tools.DialogueSystem.Utilities;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem.UI
{
    public class DSDialogueNode : DSNode
    {
        //Fields
        private TextField dialogueIdTextField;
        private TextField actorNameField;
        private Button createNewActorButton;
        private ObjectField spriteField;
        private ObjectField audioClipField;
        private TextField dialogueTextField;
        public DSDialogueNodeData Data = null;

        //Events

        #region Initialize and Draw

        public virtual void Initialize(Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            Choices = new Dictionary<Port, string>();
            this.isStartNode = isStartNode;
            this.isLoading = isLoading;
            this.isPasting = isPasting;

            DSDialogueNodeData data = new DSDialogueNodeData(dialogueId, dialogueText, position, this.DialogueType, audioClip, new DSActor(actorName, "actorBackground", actorSprite), new List<DSChoice>());
            this.Id = dialogueId;
            this.Data = data;

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
            LoadFields(Data.Guid, Data.DialogueText, Data.GetPosition(), Data.DialogueType, Data.Actor.Name, Data.Actor.background, Data.Actor.sprite, Data.AudioClip);
            RefreshExpandedState();
        }

        private void CreateCustomDataContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            VisualElement actorContainer = new VisualElement();
            actorContainer.AddToClassList("ds-node__custom-data-container");

            actorNameField = CreateTextField(Data.Actor.Name, null, false, customDataContainer, onValueChanged: (evt) => Data.Actor.Name = evt.newValue);
            spriteField = CreateObjectField("Actor Sprite", "Sprite", typeof(Sprite), customDataContainer, (evt) => Data.Actor.sprite = evt.newValue as Sprite);

            audioClipField = CreateObjectField("Dialogue Audio", "Audio Clip", typeof(AudioClip), customDataContainer, (evt) => Data.AudioClip = evt.newValue as AudioClip);

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text", false);
            dialogueTextField = CreateTextField(Data.DialogueText, null, true, textFoldout, DialogueTextChangedHandler);
            dialogueTextField.RegisterSelectionChangedCallback(OnSelectionChanged);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }

        #endregion

        #region Events

        public void OnSelectionChanged(string selectedText)
        {
        }

        private void DialogueTextChangedHandler(ChangeEvent<string> evt)
        {
            Data.DialogueText = evt.newValue;
        }

        #endregion

        #region Utils

        public void LoadFields(string id, string dialogueText, Vector2 position, DialogueType type, string actorName, string actorBackground, Sprite actorSprite, AudioClip audioClip)
        {
            if (dialogueIdTextField != null && Data != null)
            {
                this.dialogueIdTextField.value = Data.Guid;
                this.dialogueTextField.value = Data.DialogueText;
                this.actorNameField.value = Data.Actor.Name;
                this.spriteField.value = Data.Actor.sprite;
                this.audioClipField.value = Data.AudioClip;
            }
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
                                Data.DialogueText = dialogueTextField.value;
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

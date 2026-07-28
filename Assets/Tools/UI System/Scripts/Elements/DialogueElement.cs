using PrimeTween;
using System;
using System.Collections.Generic;
using TMPro;
using Tools.AutoTagSystem;
using Tools.UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.DialogueSystem
{
    public class DialogueElement : UIElement, IMouseInteractable
    {
        [field: SerializeField] public Image BGImage { get; private set; }

        [SerializeField] private bool skipDialogueOnClick = true;
        [SerializeField] private TextElement dialogueText;
        [SerializeField] private TextElement speakerNameText;
        [SerializeField] private UnityEngine.UI.Image speakerImage;
        [SerializeField] private GameObject choicesContainer;
        [SerializeField] private DialogOptionButton choiceButtonPrefab;

        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
        public event Action<PointerEventData> OnMouseClick;
        public event Action<PointerEventData> OnMouseUp;
        public event Action<PointerEventData> OnMouseDown;

        private Tween dialogueTween;

        public override void Awake()
        {
            base.Awake();
        }

        public Tween Color(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(BGImage,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        #region Dialogue System

        public void ShowDialogText(string dialogueText, string actorName, Sprite actorSprite, System.Action<TextMeshProUGUI> onComplete = null)
        {
            foreach (Transform child in choicesContainer.transform)
            {
                Destroy(child.gameObject);
            }

            speakerNameText.SetText(actorName);
            speakerImage.sprite = actorSprite;
            dialogueTween = this.dialogueText.TypeText(dialogueText, onComplete: onComplete);

            if (AutoTag.Instance != null)
            {
                this.dialogueText.Text.text = AutoTag.Instance.SetAutoTags(this.dialogueText.Text.text);
            }
        }

        public void ShowChoicesText(List<DSChoice> choices, System.Action<DSChoice> onComplete = null)
        {
            foreach (var choice in choices)
            {
                DialogOptionButton choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                TextMeshProUGUI choiceButtonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

                choiceButtonText.text = choice.Title;

                choiceButton.OnMouseClick += (eventData) =>
                {
                    onComplete?.Invoke(choice);
                };
            }
        }

        public void EndDialog()
        {
            SetActive(false);
        }

        public void TextColorSpeakerName()
        {
            speakerNameText.TextColor();
        }

        public void TextColorDialogue()
        {
            speakerNameText.TextColor();
        }

        #endregion

        #region Events

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            if (skipDialogueOnClick) dialogueTween.Complete();
            OnMouseClick?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseUp?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseDown?.Invoke(eventData);
        }

        #endregion
    }
}


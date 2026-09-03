using PrimeTween;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tools.UISystem.Interfaces;

namespace Tools.UISystem.Elements
{
    public class ButtonElement : UIElement, IMouseInteractable
    {
        public Image Image { get; private set; }
        public TextMeshProUGUI Text { get; private set; }

        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
        public event Action<PointerEventData> OnMouseClick;
        public event Action<PointerEventData> OnMouseUp;
        public event Action<PointerEventData> OnMouseDown;
        public event Action<string> OnValueChanged;

        public override void Awake()
        {
            base.Awake();

            Image = GetComponentInChildren<Image>();
            Text = GetComponentInChildren<TextMeshProUGUI>();
        }

        #region Animations

        public void TypeText(int charCount = 0,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<TextMeshProUGUI> onComplete = null)
        {
            if (Text.text.Length == 0) this.Text.text = string.Empty;

            this.Text.text = Text.text;
            this.Text.maxVisibleCharacters = 0;

            if (charCount == 0) charCount = Text.text.Length;

            Text.ForceMeshUpdate();

            Tween.TextMaxVisibleCharacters(Text,
                charCount,
                duration ?? AnimationSettings.Duration,
                ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unscaledTime)
                .OnComplete(target: Text, onComplete);
        }

        public Tween TextColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(Text,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public Tween Color(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(Image,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
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

        #region Utils

        public void SetText(string value)
        {
            Text.SetText(value);

            if (!EventSettings.OnValueChanged) return;
            
            OnValueChanged?.Invoke(value);
        }

        public void AppendText(string value)
        {
            string text = Text.text;
            string newText = text + value;
            Text.SetText(newText);
            if (!EventSettings.OnValueChanged) return;

            OnValueChanged?.Invoke(value);
        }

        public void ClearText()
        {
            Text.SetText(string.Empty);

            if(!EventSettings.OnValueChanged) return;

            OnValueChanged?.Invoke(Text.text);
        }

        #endregion
    }
}

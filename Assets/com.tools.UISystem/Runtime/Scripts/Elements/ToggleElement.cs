using PrimeTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tools.UISystem.Interfaces;

namespace Tools.UISystem.Elements
{
    public class ToggleElement : UIElement
    {
        [field: SerializeField] public Image CheckMarkImage {  get; private set; }
        public Toggle ToggleComponent { get; private set; }
        public TextMeshProUGUI Text { get; private set; }
        public bool Value { get; private set; }

        public event Action<bool> OnValueChanged;

        #region Initialization

        public override void Awake()
        {
            base.Awake();
            ToggleComponent = GetComponent<Toggle>();
            Text = GetComponentInChildren<TextMeshProUGUI>();
            this.Value = ToggleComponent.isOn;
            ToggleComponent.onValueChanged.AddListener(ValueChangedHandler);
        }

        #endregion

        #region Animations

        public void TypeText(int charCount = 0,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<TMP_Text> onComplete = null)
        {
            if (Text.text.Length == 0)
            {
                return;
            }

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
            return Tween.Color(CheckMarkImage,
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

        #region Utils

        public void SetValue(bool value)
        {
            this.Value = value;
            this.ToggleComponent.isOn = value;
        }

        public void SetText(string value)
        {
            Text.text = value;
        }

        public void AppendText(string value)
        {
            string text = Text.text;
            string newText = text + value;
            Text.text = newText;
        }

        public void Clear()
        {
            Text.text = string.Empty;
        }

        #endregion

        #region Events

        private void ValueChangedHandler(bool value)
        {
            this.Value = value;

            if (!EventSettings.OnValueChanged) return;

            OnValueChanged?.Invoke(value);
        }

        #endregion
    }
}
using PrimeTween;
using System;
using TMPro;

namespace Tools.UISystem
{
    public class TextElement : UIElement
    {
        public TextMeshProUGUI Text { get; private set; }

        public event Action<string> OnValueChanged;

        public override void Awake()
        {
            base.Awake();
            Text = GetComponentInChildren<TextMeshProUGUI>();
        }

        #region Animations

        public Tween TextColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<TextElement> onComplete = null)
        {
            return Tween.Color(Text,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween TypeText(string text,
            int charCount = 0,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<TextMeshProUGUI> onComplete = null)
        {
            if (text.Length == 0) this.Text.text = string.Empty;

            this.Text.text = text;
            this.Text.maxVisibleCharacters = 0;

            if (charCount == 0) charCount = Text.text.Length;

            Text.ForceMeshUpdate();
            return Tween.TextMaxVisibleCharacters(Text,
                charCount,
                duration ?? AnimationSettings.Duration,
                ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                cycles: cycles,
                useUnscaledTime: unscaledTime)
                .OnComplete(target: Text, onComplete);
        }

        #endregion

        #region Utils

        public void SetText(string value)
        {
            Text.text = value;
            if (!EventSettings.OnValueChanged) return;
            OnValueChanged?.Invoke(value);
        }

        public void AppendText(string value)
        {
            string text = Text.text;
            string newText = text + value;
            Text.text = newText;
            if (!EventSettings.OnValueChanged) return;
            OnValueChanged?.Invoke(value);
        }

        public void Clear()
        {
            Text.text = string.Empty;
            if (!EventSettings.OnValueChanged) return;
            OnValueChanged?.Invoke(Text.text);
        }

        #endregion
    }
}


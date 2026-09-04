using PrimeTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.TweenSystem.Elements
{
    public class ProgressBarElement : UIElement
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Image BGImage { get; private set; }
        [SerializeField] private Image fill;
        [SerializeField] private Gradient gradient;
        [SerializeField] private bool hasText;
        private float currentValue;
        private Color currentColor;
        public event Action<float> OnValueChanged;

        public float ProgressValue
        {
            get => currentValue;
            set
            {
                if (this.currentValue != value)
                {
                    this.currentValue = value;

                    if (EventSettings.OnValueChanged)
                    {
                        OnValueChanged?.Invoke(this.currentValue);
                    }

                    UpdateUI();
                }
            }
        }

        public Color ProgressColor
        {
            get => currentColor;
            set
            {
                if (this.currentColor != value)
                {
                    this.currentColor = value;
                    UpdateUI();
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
        }

        public virtual void OnValidate()
        {
            if (Text != null)
            {
                Text.gameObject.SetActive(hasText);
            }
        }

        private void UpdateUI()
        {
            SetProgressBarColor(ProgressColor);
            SetProgressBarValue(ProgressValue);
        }

        private void SetProgressBarColor(Color color)
        {
            fill.color = gradient.Evaluate(ProgressValue);
        }

        private void SetProgressBarValue(float value)
        {
            if (value >= 0f && value <= 1f)
            {
                fill.fillAmount = value;
            }
            else
            {
                Debug.LogWarning("Progress Value should be between 0 and 1");
            }
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

        public Tween FillColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(fill,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public Tween BGColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(BGImage,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public void SetText(string value)
        {
            if (hasText)
            {
                Text.text = value;
            }
        }

        public void AppendText(string value)
        {
            if (hasText)
            {
                string text = Text.text;
                string newText = text + value;
                Text.text = newText; 
            }
        }

        public void Clear()
        {
            if (hasText)
            {
                Text.text = string.Empty; 
            }
        }
    }
}


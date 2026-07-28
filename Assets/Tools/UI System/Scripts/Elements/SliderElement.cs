using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.UISystem
{
    [RequireComponent(typeof(Slider))]
    public class SliderElement : UIElement
    {
        [field: SerializeField] public Image BGImage { get; private set; }
        [field: SerializeField] public Image FillImage { get; private set; }
        [field: SerializeField] public Image HandleImage { get; private set; }
        public Slider Slider { get; private set; }
        public float Value { get; private set; }
        public event Action<float> OnValueChanged;

        public override void Awake()
        {
            base.Awake();
            Slider = GetComponent<Slider>();
            Slider.onValueChanged.AddListener(OnValueChangedHandler);

            if (EventSettings != null)
            {
                Selectable = Slider.GetComponent<Selectable>();
            }
        }

        public void SetValue(float value)
        {
            this.Value = value;
            Slider.value = this.Value;
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

        public Tween HandleColor(
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

        private void OnValueChangedHandler(float value)
        {
            this.Value = value;

            if (!EventSettings.OnValueChanged) return;

            this.OnValueChanged?.Invoke(this.Value);
        }
    }
}


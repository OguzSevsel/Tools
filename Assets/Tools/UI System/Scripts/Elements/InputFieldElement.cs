using PrimeTween;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.UISystem
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(Image))]
    public class InputFieldElement : UIElement
    {
        public Image Image { get; private set; }
        public TMP_InputField InputField { get; private set; }
        public string Text { get; private set; }

        public event Action<string> OnTextSelected;
        public event Action<string> OnTextDeselected;
        public event Action<string> OnTextValueChanged;
        public event Action<string> OnTextEndEdit;

        public override void Awake()
        {
            base.Awake();

            InputField = GetComponent<TMP_InputField>();
            Image = GetComponent<Image>();
            Text = InputField.text;

            InputField.onValueChanged.AddListener(OnValueChanged);
            InputField.onEndEdit.AddListener(OnEndEdit);
            InputField.onDeselect.AddListener(OnDeselect);
            InputField.onSelect.AddListener(OnSelect);

            if (EventSettings != null)
            {
                Selectable = InputField.GetComponent<Selectable>();
            }
        }

        public void TypeText(int? charCount = null, float? duration = null, Ease? ease = null, float startDelay = 0f, float endDelay = 0f, bool unscaledTime = true, System.Action<TMP_Text> onComplete = null)
        {
            if (Text.Count() == 0)
            {
                return;
            }

            if (charCount == null) charCount = Text.Count();
            else charCount = charCount.Value;

            InputField.textComponent.ForceMeshUpdate();
            Tween.TextMaxVisibleCharacters(InputField.textComponent,
                charCount.Value,
                duration ?? AnimationSettings.Duration,
                ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unscaledTime)
                .OnComplete(target: InputField.textComponent, onComplete);
        }

        public Tween TextColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(InputField.textComponent,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
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
            return Tween.Color(Image,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        #region Events

        public void OnSelect(string value)
        {
            if (!EventSettings.Selectable) return;

            OnTextSelected?.Invoke(value);
        }

        public void OnDeselect(string value)
        {
            if (!EventSettings.Selectable) return;

            OnTextDeselected?.Invoke(value);
        }

        public void OnValueChanged(string value)
        {
            Text = value;

            if (!EventSettings.OnValueChanged) return;

            OnTextValueChanged?.Invoke(value);
        }

        public void OnEndEdit(string value)
        {
            if (!EventSettings.Interactable) return;

            OnTextEndEdit?.Invoke(value);
        }

        #endregion
    }
}


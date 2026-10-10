using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Tools.TweenSystem.Elements
{
    [RequireComponent(typeof(Image))]
    public class TooltipElement : UIElement
    {
        public Image Image { get; private set; }
        [field: SerializeField] public TextMeshProUGUI HeaderField { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ContentField { get; private set; }
        [field: SerializeField] public LayoutElement LayoutElement { get; private set; }

        public int CharacterWrapLimit;

        #region Initialization

        public override void Awake()
        {
            base.Awake();

            Image = GetComponent<Image>();
        }

        #endregion

        #region Animations

        public Tween ContentColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(ContentField,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public Tween HeaderColor(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(HeaderField,
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

        #endregion

        #region Tooltip System

        private void Update()
        {
            Vector2 position = Mouse.current.position.ReadValue();

            Vector2 normalizedPosition = new(position.x / Screen.width, position.y / Screen.height);
            Vector2 pivot = CalculatePivot(normalizedPosition);
            RectTransform.pivot = pivot;
            transform.position = position;
        }

        private Vector2 CalculatePivot(Vector2 normalizedPosition)
        {
            Vector2 pivotTopLeft = new(-0.05f, 1.05f);
            Vector2 pivotTopRight = new(1.05f, 1.05f);
            Vector2 pivotBottomLeft = new(-0.05f, -0.05f);
            Vector2 pivotBottomRight = new(1.05f, -0.05f);

            return normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f
                ? pivotTopLeft
                : normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f
                    ? pivotTopRight
                    : normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f ? pivotBottomLeft : pivotBottomRight;
        }

        public void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
            {
                HeaderField.gameObject.SetActive(false);
            }
            else
            {
                HeaderField.gameObject.SetActive(true);
                HeaderField.text = header;
            }

            ContentField.text = content;

            int headerLength = HeaderField.text.Length;
            int contentLength = ContentField.text.Length;

            LayoutElement.enabled = headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit;
        }

        #endregion
    }
}

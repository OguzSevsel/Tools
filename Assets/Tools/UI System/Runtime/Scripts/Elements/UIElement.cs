using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Interfaces;
using Utilities;
using Settings;

namespace Elements
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIElement : Element, ISelectable
    {
        [field: SerializeField] public EventSettingsSO EventSettings { get; protected set; }
        public Vector4 Padding { get; protected set; }
        public RectTransform RectTransform { get; protected set; }
        public Selectable Selectable { get; protected set; }
        public CanvasGroup CanvasGroup { get; protected set; }

        public event Action<BaseEventData> Selected;
        public event Action<BaseEventData> Deselected;
        public event Action<BaseEventData> Submitted;

        #region Unity Methods

        public override void Awake()
        {
            base.Awake();

            if (EventSettings != null)
            {
                if (EventSettings.Selectable && gameObject.TryGetComponent<Selectable>(out Selectable selectable))
                {
                    if (selectable == null)
                    {
                        Selectable = gameObject.AddComponent<Selectable>();
                    }
                    else
                    {
                        Selectable = selectable;
                    }
                }
            }

            CanvasGroup = GetComponent<CanvasGroup>();
            RectTransform = GetComponent<RectTransform>();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            ChangeNavigationMode(Navigation.Mode.None);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            ChangeNavigationMode(Navigation.Mode.Automatic);
        }

        #endregion

        #region Animations

        public Tween GrowVertical(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float pivotY = 0.5f,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            RectTransform.localScale = new Vector3(1f, 0f, RectTransform.localScale.z);

            if (RectTransform.pivot.x == 0)
            {
                RectTransform.pivot = new Vector2(0.5f, pivotY);
            }

            RectTransform.pivot = new Vector2(RectTransform.pivot.x, pivotY);

            return Tween.ScaleY(transform,
                endValue: endValue ?? 1f,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ShrinkVertical(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float pivotY = 0.5f,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            RectTransform.localScale = new Vector3(1f, 1f, RectTransform.localScale.z);

            if (RectTransform.pivot.x == 0)
            {
                RectTransform.pivot = new Vector2(0.5f, pivotY);
            }

            RectTransform.pivot = new Vector2(RectTransform.pivot.x, pivotY);

            return Tween.ScaleY(transform,
                endValue: endValue ?? 0f,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        private Tween GrowHorizontal(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float pivotX = 0.5f,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            RectTransform.localScale = new Vector3(0f, 1f, RectTransform.localScale.z);

            if (RectTransform.pivot.y == 0)
            {
                RectTransform.pivot = new Vector2(pivotX, 0.5f);
            }

            RectTransform.pivot = new Vector2(pivotX, RectTransform.pivot.y);

            return Tween.ScaleX(transform,
                endValue: endValue ?? 1f,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ShrinkHorizontal(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float pivotX = 0.5f,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            RectTransform.localScale = new Vector3(1f, 1f, RectTransform.localScale.z);

            if (RectTransform.pivot.y == 0)
            {
                RectTransform.pivot = new Vector2(pivotX, 0.5f);
            }

            RectTransform.pivot = new Vector2(pivotX, RectTransform.pivot.y);

            return Tween.ScaleX(transform,
                endValue: endValue ?? 0f,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween FadeIn(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<UIElement> onComplete = null
            )
        {
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            return Tween.Alpha(CanvasGroup,
                endValue ?? AnimationSettings.FadeInValue,
                duration ?? AnimationSettings.Duration,
                ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween FadeOut(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<UIElement> onComplete = null
            )
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            return Tween.Alpha(CanvasGroup,
                endValue ?? AnimationSettings.FadeOutValue,
                duration ?? AnimationSettings.Duration,
                ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween RollIn(float? startValue = null,
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            SlideDirection? slideDirection = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool useUnscaledTime = true,
            System.Action<UIElement> onComplete = null)
        {
            slideDirection = slideDirection ?? AnimationSettings.RollInDirection;

            switch (slideDirection)
            {
                case SlideDirection.Left: //To Left

                    return GrowHorizontal(endValue, duration, ease, 1f, cycles, 
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Right: //To Right

                    return GrowHorizontal(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Bottom: //To Bottom

                    return GrowVertical(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Top: //To Top

                    return GrowVertical(endValue, duration, ease, 1f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                default: //To Right

                    return GrowHorizontal(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);
            }
        }

        public Tween RollOut(float? startValue = null,
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            SlideDirection? slideDirection = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool useUnscaledTime = true,
            System.Action<UIElement> onComplete = null)
        {
            slideDirection = slideDirection ?? AnimationSettings.RollOutDirection;  

            switch (slideDirection)
            {
                case SlideDirection.Left: //To Left

                    return ShrinkHorizontal(endValue, duration, ease, 1f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Right: //To Right

                    return ShrinkHorizontal(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Bottom: //To Bottom

                    return ShrinkVertical(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                case SlideDirection.Top: //To Top

                    return ShrinkVertical(endValue, duration, ease, 1f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);

                default: //To Right

                    return ShrinkHorizontal(endValue, duration, ease, 0f, cycles,
                        startDelay, endDelay, useUnscaledTime)
                        .OnComplete(this, onComplete);
            }
        }

        #endregion

        #region Selectable Methods

        protected void ChangeNavigationMode(Navigation.Mode mode)
        {
            if (Selectable != null)
            {
                Navigation nav = Selectable.navigation;
                nav.mode = mode;
                Selectable.navigation = nav;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!EventSettings.Selectable) return;
            Selected?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!EventSettings.Selectable) return;
            Deselected?.Invoke(eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!EventSettings.Selectable) return;
            Submitted?.Invoke(eventData);
        }

        #endregion

        #region Show/Hide Methods

        public override void SetActive(bool active = true)
        {
            if (active)
            {
                if (!this.gameObject.activeInHierarchy)
                {
                    ChangeNavigationMode(Navigation.Mode.Automatic);
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                if (this.gameObject.activeInHierarchy)
                {
                    ChangeNavigationMode(Navigation.Mode.None);
                    this.gameObject.SetActive(false);
                }
            }
        }
        
        public bool IsVisible()
        {
            if (CanvasGroup != null && CanvasGroup.alpha == 1)
            {
                return true;
            }
            return false;
        }

        public void Show()
        {
            ChangeNavigationMode(Navigation.Mode.Automatic);
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            ChangeNavigationMode(Navigation.Mode.None);
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0f;
        }

        #endregion
    }
}


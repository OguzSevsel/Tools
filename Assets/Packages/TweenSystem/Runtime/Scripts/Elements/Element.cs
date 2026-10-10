using PrimeTween;
using UnityEngine;
using Tools.TweenSystem.Utilities;
using Tools.TweenSystem.Settings;

namespace Tools.TweenSystem.Elements
{
    public abstract class Element : MonoBehaviour
    {
        [field: SerializeField] public AnimationSettingsSO AnimationSettings { get; protected set; }
        [field: SerializeField] public string ElementId { get; protected set; } = string.Empty;
        public AnimSequence Sequence { get; private set; }
        public Vector3 DefaultLocalPosition { get; protected set; }

        public virtual void Awake()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            PrimeTweenConfig.warnTweenOnDisabledTarget = false;

            if (ElementId != string.Empty)
            {
                Subscribe(ElementId, this);
            }

            DefaultLocalPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y, this.gameObject.transform.localPosition.z);
        }

        public virtual void OnDestroy()
        {
            Unsubscribe();
        }

        public void Subscribe<T>(string id, T register) where T : Element
        {
            ElementId = id;
            Registry.Subscribe(ElementId, register);
        }

        public void Unsubscribe()
        {
            if (ElementId != string.Empty) Registry.Unsubscribe(ElementId);
        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnEnable()
        {
            SaveLocalPos();
        }

        public AnimSequence Animate(bool unScaledTime = true)
        {
            Sequence = new AnimSequence(unScaledTime: unScaledTime);
            return Sequence;
        }

        public void ResetLocalPos()
        {
            this.gameObject.transform.localPosition = Vector3.zero;
            DefaultLocalPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y, this.gameObject.transform.localPosition.z);
        }

        public void SaveLocalPos()
        {
            DefaultLocalPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y, this.gameObject.transform.localPosition.z);
        }

        public virtual void SetActive(bool active = true)
        {
            if (active)
            {
                if (!this.gameObject.activeInHierarchy)
                {
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                if (this.gameObject.activeInHierarchy)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        public virtual bool IsActive()
        {
            if (gameObject.activeInHierarchy)
            {
                return true;
            }
            return false;
        }

        #region Animations

        private Vector3 GetDirectionOffSet(SlideDirection direction, Vector3 startPos, float distance)
        {
            Vector3 endPos = new Vector3(startPos.x, startPos.y, startPos.z);

            switch (direction)
            {
                case SlideDirection.Left: endPos.x -= distance; break;
                case SlideDirection.Right: endPos.x += distance; break;
                case SlideDirection.Top: endPos.y += distance; break;
                case SlideDirection.Bottom: endPos.y -= distance; break;
            }

            return endPos;
        }

        public Tween Move(
            Vector3 endValue,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.Position(transform,
                endValue: endValue,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween PunchScale(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.PunchScale(transform,
                strength: endValue ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween PunchRotation(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float? frequency = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.PunchLocalRotation(transform,
                strength: endValue ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                frequency: frequency ?? AnimationSettings.ShakeFrequency,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween PunchPosition(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float? frequency = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.PunchLocalPosition(transform,
                strength: endValue ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                frequency: frequency ?? AnimationSettings.ShakeFrequency,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween Rotation(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.LocalRotation(transform,
                endValue: endValue ?? AnimationSettings.RotationVector,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween Grow(
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.Scale(transform,
                endValue: new Vector3(1, 1, 1),
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween Shrink(
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.Scale(transform,
                endValue: new Vector3(0, 0, 0),
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween GrowVertical(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
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
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
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

        public Tween GrowHorizontal(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
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
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
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

        public Tween ScaleUp(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.Scale(transform,
                endValue: endValue ?? AnimationSettings.ScaleUpVector,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ScaleDown(
            Vector3? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.Scale(transform,
                    endValue: endValue ?? AnimationSettings.ScaleDownVector,
                    duration: duration ?? AnimationSettings.Duration,
                    ease: ease ?? AnimationSettings.Ease,
                    cycles: cycles,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween SlideIn(
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            if (this.gameObject.activeInHierarchy)
            {
                this.gameObject.transform.localPosition = GetDirectionOffSet(
                    direction ?? AnimationSettings.SlideInDirection,
                    DefaultLocalPosition,
                    slideDistance ?? AnimationSettings.SlideDistance);
            }

            return Tween.LocalPosition(transform,
                    endValue: DefaultLocalPosition,
                    duration: duration ?? AnimationSettings.Duration,
                    ease: ease ?? AnimationSettings.Ease,
                    cycles: cycles,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween SlideOut(
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<Element> onComplete = null)
        {
            Vector3 endPos = DefaultLocalPosition;

            if (this.gameObject.activeInHierarchy)
            {
                endPos = GetDirectionOffSet(
                direction ?? AnimationSettings.SlideOutDirection,
                DefaultLocalPosition,
                slideDistance ?? AnimationSettings.SlideDistance);
            }

            return Tween.LocalPosition(transform,
                    endValue: endPos,
                    duration: duration ?? AnimationSettings.Duration,
                    ease: ease ?? AnimationSettings.Ease,
                    cycles: cycles,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    useUnscaledTime: unScaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ShakePosition(
            Vector3? strength = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float? frequency = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.ShakeLocalPosition(transform,
                strength: strength ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                frequency: frequency ?? AnimationSettings.ShakeFrequency,
                useUnscaledTime: unscaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ShakeRotation(
            Vector3? strength = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float? frequency = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.ShakeLocalRotation(transform,
                strength: strength ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                frequency: frequency ?? AnimationSettings.ShakeFrequency,
                useUnscaledTime: unscaledTime)
                .OnComplete(this, onComplete);
        }

        public Tween ShakeScale(
            Vector3? strength = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float? frequency = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unscaledTime = true,
            System.Action<Element> onComplete = null)
        {
            return Tween.ShakeScale(transform,
                strength: strength ?? AnimationSettings.ShakeStrength,
                duration: duration ?? AnimationSettings.Duration,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                easeBetweenShakes: ease ?? AnimationSettings.Ease,
                frequency: frequency ?? AnimationSettings.ShakeFrequency,
                useUnscaledTime: unscaledTime)
                .OnComplete(this, onComplete);
        }

        #endregion
    } 
}

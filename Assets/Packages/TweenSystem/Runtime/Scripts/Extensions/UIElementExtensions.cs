using PrimeTween;
using Tools.TweenSystem.Elements;
using Tools.TweenSystem.Utilities;

namespace Tools.TweenSystem.Extensions
{
	public static class UIElementExtensions
	{
        public static AnimSequence FadeSlideIn(
            this UIElement el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<UIElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideIn(direction: direction, 
                slideDistance: slideDistance, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime))
                .Also(el.FadeIn(endValue: fadeEndValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));
            
            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence FadeSlideOut(
            this UIElement el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<UIElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideOut(direction: direction, 
                slideDistance: slideDistance, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime))
                .Also(el.FadeOut(endValue: fadeEndValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence RollFadeIn(
            this UIElement el,
            float? rollStartValue = null,
            float? rollEndValue = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            SlideDirection? slideDirection = null,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<UIElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.RollIn(rollStartValue, 
                rollEndValue, 
                duration, 
                ease, 
                slideDirection, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                useUnscaledTime: unScaledTime))
                .Also(el.FadeIn(endValue: fadeEndValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence RollFadeOut(
            this UIElement el,
            float? rollStartValue = null,
            float? rollEndValue = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            SlideDirection? slideDirection = null,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<UIElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.RollOut(startValue: rollStartValue,
                endValue: rollEndValue,
                duration: duration,
                ease: ease,
                slideDirection: slideDirection,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime))
                .Also(el.FadeOut(endValue: fadeEndValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));
            
            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence Breathe(
            this UIElement el,
            float? fadeInValue = null,
            float? fadeOutValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<UIElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.FadeOut(endValue: fadeOutValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime))
                .Next(el.FadeIn(endValue: fadeInValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }
    } 
}

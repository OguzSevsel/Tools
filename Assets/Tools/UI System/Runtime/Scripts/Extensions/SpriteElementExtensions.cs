using Elements;
using PrimeTween;
using Utilities;

namespace Extensions
{
    public static class SpriteElementExtensions
    {
        public static AnimSequence Breathe(
            this SpriteElement el,
            float? fadeInValue = null,
            float? fadeOutValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<SpriteElement> onComplete = null)
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

        public static AnimSequence FadeSlideIn(
            this SpriteElement el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<SpriteElement> onComplete = null)
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
            this SpriteElement el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<SpriteElement> onComplete = null)
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

        public static AnimSequence FlashColor(
            this SpriteElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SpriteElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.Color(endValue: endValue,
                duration: duration,
                ease: ease,
                startDelay: startDelay,
                endDelay: endDelay,
                unScaledTime: unScaledTime))
                .Next(el.Color(endValue: el.AnimationSettings.DefaultColor,
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



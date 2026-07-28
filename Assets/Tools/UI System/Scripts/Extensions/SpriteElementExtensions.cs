using PrimeTween;

namespace Tools.UISystem
{
    public static class SpriteElementExtensions
    {
        public static Sequence Breathe(
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

        public static Sequence FadeSlideIn(
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

        public static Sequence FadeSlideOut(
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

        public static Sequence FlashColor(
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



using PrimeTween;
using Tools.TweenSystem.Elements;
using Tools.TweenSystem.Utilities;

namespace Tools.TweenSystem.Extensions
{
    public static class TextElementExtensions
    {
        public static AnimSequence FlashTextColor(
            this TextElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<TextElement> onComplete = null)
        {
            AnimSequence seq = el.Animate()
                .Begin(el.TextColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.TextColor(
                    endValue: el.AnimationSettings.DefaultColor,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime));

            _ = seq.SetLoops(cycles);

            if (onComplete != null)
                _ = seq.OnComplete(el, onComplete);

            return seq;
        }
    }
}

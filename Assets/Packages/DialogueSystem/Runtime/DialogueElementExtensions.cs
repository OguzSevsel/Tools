using PrimeTween;
using Tools.DialogueSystem.Elements;
using Tools.TweenSystem.Utilities;

namespace Tools.DialogueSystem.Extensions
{
    public static class DialogueElementExtensions
    {
        public static AnimSequence FlashColor(
            this DialogueElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<DialogueElement> onComplete = null)
        {
            AnimSequence seq = el.Animate()
                .Begin(el.Color(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.Color(
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



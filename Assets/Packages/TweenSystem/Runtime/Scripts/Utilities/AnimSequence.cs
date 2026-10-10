using PrimeTween;

namespace Tools.TweenSystem.Utilities
{
    public class AnimSequence
    {
        private readonly PrimeTween.Sequence _seq;
        private bool _hasSlot;
        public bool IsAlive => _seq.isAlive;

        public AnimSequence(bool unScaledTime = true)
        {
            _seq = PrimeTween.Sequence.Create(useUnscaledTime: unScaledTime);
            _hasSlot = false;
        }

        public AnimSequence Begin(Tween tween)
        {
            _seq.Chain(tween);
            _hasSlot = true;
            return this;
        }

        public AnimSequence Also(Tween tween)
        {
            if (!_hasSlot)
                return Begin(tween);
            _seq.Group(tween);
            return this;
        }

        public AnimSequence Next(Tween tween)
        {
            _hasSlot = false;
            return Begin(tween);
        }

        public AnimSequence SetDelay(float time)
        {
            _seq.ChainDelay(time);
            _hasSlot = false;
            return this;
        }

        public AnimSequence SetLoops(int loopCount = -1)
        {
            _seq.SetRemainingCycles(loopCount);
            return this;
        }

        public void Stop()
        {
            _seq.Stop();
        }

        public AnimSequence OnComplete<T>(T target, System.Action<T> cb) where T : class
        {
            _seq.OnComplete(target: target, onComplete: cb);
            return this;
        }
    }
}


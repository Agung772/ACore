using UnityEngine;

namespace ACore.Animation
{
    public class AnimationTimeBase : AnimationBase
    {
        [SerializeField] protected float startDelay;
        [SerializeField] protected float time = 1;
        [SerializeField] protected LeanTweenType type;

        protected override void OnEnable()
        {
            if (autoPlay && startDelay > 0)
            {
                base.descr = gameObject.LeanDelayedCall(startDelay, Play);
                return;
            }
            
            base.OnEnable();
        }

        public override void Play()
        {
            base.descr.setEase(type);
            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            base.ToDefault(fasted);
            if (!fasted) base.descr.setEase(type);
        }
    }
}

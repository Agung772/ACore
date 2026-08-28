using System;
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
                base.descr = gameObject.LeanDelayedCall(startDelay, () => Play());
                return;
            }
            
            base.OnEnable();
        }

        public override void Play(Action onComplete = null)
        {
            if (descr != null)
            {
                descr.setEase(type);
            }
            base.Play(onComplete);
        }

        public override void ToDefault(bool instant = false, Action onComplete = null)
        {
            if (!instant && descr != null)
            {
                descr.setEase(type);
            }
            base.ToDefault(instant, onComplete);
        }
    }
}

using UnityEngine;

namespace ACore.Animation
{
    public class AnimationTimeBase : AnimationBase
    {
        [SerializeField] protected float startDelay;
        [SerializeField] protected float time = 1;
        [SerializeField] protected LeanTweenType type;

        public override void Play()
        {
            base.Play();
            
            base.descr.setEase(type);
            
            if (startDelay > 0)
            {
                base.descr.setDelay(startDelay);
            }
        }

        public override void ToDefault(bool fasted = false)
        {
            base.ToDefault(fasted);
            if (!fasted) base.descr.setEase(type);
        }
    }
}

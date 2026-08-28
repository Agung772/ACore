using System;
using Sirenix.OdinInspector;

namespace ACore.Animation
{
    [Serializable, InlineProperty]
    public class AnimationGroup
    {
        public AnimationBase[] animations;

        public void Play(Action onComplete = null)
        {
            if (animations == null) return;
            for (int i = 0; i < animations.Length; i++)
            {
                var anim = animations[i];
                if (anim == null) continue;
                if (i == animations.Length - 1)
                    anim.Play(onComplete);
                else
                    anim.Play();
            }
        }
        
        public void Stop()
        {
            if (animations == null) return;
            foreach (var _animation in animations)
            {
                if (_animation == null) continue;
                _animation.Stop();
            }
        }        
        
        public void ToDefault(bool instant = false, Action onComplete = null)
        {
            if (animations == null) return;
            for (int i = 0; i < animations.Length; i++)
            {
                var anim = animations[i];
                if (anim == null) continue;
                if (i == animations.Length - 1)
                    anim.ToDefault(instant, onComplete);
                else
                    anim.ToDefault(instant);
            }
        }
    }
}

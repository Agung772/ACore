using System;
using Sirenix.OdinInspector;

namespace ACore.Animation
{
    [Serializable, InlineProperty]
    public class AnimationGroup
    {
        public AnimationBase[] animations;

        public void Play()
        {
            foreach (var _animation in animations)
            {
                if (_animation == null) continue;
                _animation.Play();
            }
        }
        
        public void Stop()
        {
            foreach (var _animation in animations)
            {
                if (_animation == null) continue;
                _animation.Stop();
            }
        }        
        
        public void ToDefault()
        {
            foreach (var _animation in animations)
            {
                if (_animation == null) continue;
                _animation.ToDefault();
            }
        }
    }
}
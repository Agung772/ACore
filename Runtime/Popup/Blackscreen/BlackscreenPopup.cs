using System;
using ACore.Animation;
using UnityEngine;

namespace ACore
{
    public class BlackscreenPopup : UIBehaviour
    {
        [SerializeField] private ValueThis fade;

        public void Fade(FadeType type, bool instant = false, Action onComplete = null)
        {
            if (type == FadeType.In) In(instant, onComplete);
            else Out(instant, onComplete);
        }

        public void In(bool instant = false, Action onComplete = null)
        {
            fade.Set(0, 1, instant, onComplete);
        }
    
        public void Out(bool instant, Action onComplete = null)
        {
            fade.Set(0, 1, instant, onComplete);
        }
    }
}

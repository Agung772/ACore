using System;
using ACore.Animation;
using UnityEngine;

namespace ACore
{
    public class BlackscreenPopup : UIBehaviour
    {
        [SerializeField] private ValueThis fade;
        
        public void In(bool instant = false, Action onComplete = null)
        {
            fade.Set(0, 1, instant, onComplete);
            fade.onComplete.RemoveAllListeners();
            fade.onComplete.AddListener(() => onComplete?.Invoke());
        }
    
        public void Out(bool instant, Action onComplete = null)
        {
            fade.Set(0, 1, instant, onComplete);
            fade.onComplete.RemoveAllListeners();
            fade.onComplete.AddListener(() => onComplete?.Invoke());
        }
    }
}

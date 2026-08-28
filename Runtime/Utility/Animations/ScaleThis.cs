using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class ScaleThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom))] private Vector3 from;
        [SerializeField] private Vector3 to;

        private void Awake()
        {
            if (isFrom && !base.autoPlay)
            {
                transform.localScale = from;
            }
        }
        
        public override void Play(Action onComplete = null)
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            {
                transform.localScale = from;
            }

            base.descr = gameObject.LeanScale(to, time);
            base.Play(onComplete);
        }
        
        public override void ToDefault(bool instant = false, Action onComplete = null)
        {
            if (!isFrom)
            {
                Debug.LogWarning("To Default not available because it is not from.");
                onComplete?.Invoke();
                return;
            }
            
            base.Stop();
            
            if (instant)
            {
                transform.localScale = from;
                base.ToDefault(true, onComplete);
            }
            else
            {
                base.descr = gameObject.LeanScale(from, time);
                base.ToDefault(false, onComplete);
            }
        }
    }
}

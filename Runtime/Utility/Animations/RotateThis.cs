using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class RotateThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom))] private Vector3 from;
        [SerializeField] private Vector3 to;
        
        [SerializeField] private float add = 360;

        private void Awake()
        {
            if (isFrom && !base.autoPlay)
            {
                transform.eulerAngles = from;
            }
        }

        public override void Play(Action onComplete = null)
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            {
                transform.eulerAngles = from;
            }
            base.descr = gameObject.LeanRotateAroundLocal(to, add, time);
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
                transform.eulerAngles = from;
                base.ToDefault(true, onComplete);
            }
            else
            {
                base.descr = gameObject.LeanRotateAroundLocal(from, add, time);
                base.ToDefault(false, onComplete);
            }
        }
    }
}

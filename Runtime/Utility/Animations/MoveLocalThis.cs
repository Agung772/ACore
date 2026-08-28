using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class MoveLocalThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom)), PickFromScene] private Vector3 from;
        [SerializeField, PickFromScene] private Vector3 to;

        private void Awake()
        {
            if (isFrom && !base.autoPlay)
            { 
                transform.localPosition = from;
            }
        }

        public override void Play(Action onComplete = null)
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            { 
                transform.localPosition = from;
            }
            base.descr = gameObject.LeanMoveLocal(to, time);
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
                transform.localPosition = from;
                base.ToDefault(true, onComplete);
            }
            else
            {
                base.descr = gameObject.LeanMoveLocal(from, time);
                base.ToDefault(false, onComplete);
            }
        }
    }
}

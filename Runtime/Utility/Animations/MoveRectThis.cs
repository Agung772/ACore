using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class MoveRectThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom)), PickFromScene] private Vector3 from;
        [SerializeField, PickFromScene] private Vector3 to;
        
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (isFrom && !base.autoPlay)
            { 
                rectTransform.anchoredPosition = from;
            }
        }

        public override void Play(Action onComplete = null)
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            { 
                rectTransform.anchoredPosition = from;
            }
            base.descr = rectTransform.LeanMove(to, time);
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
                rectTransform.anchoredPosition = from;
                base.ToDefault(true, onComplete);
            }
            else
            {
                base.descr = rectTransform.LeanMove(from, time);
                base.ToDefault(false, onComplete);
            }
        }
    }
}

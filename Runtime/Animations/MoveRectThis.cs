using System;
using ACore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class MoveRectThis : AnimationBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom)), PickFromScene] private Vector3 from;
        [SerializeField, PickFromScene] private Vector3 to;
        
        [SerializeField] private float time = 1;
        [SerializeField] private LeanTweenType type;
        
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (isFrom && !base.autoPlay)
            { 
                rectTransform.anchoredPosition= from;
            }
        }

        public override void Play()
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            { 
                rectTransform.anchoredPosition= from;
            }
            base.descr = rectTransform.LeanMove(to, time).setEase(type).setIgnoreTimeScale(base.useUnScaledTime);
            base.Play();
        }
        
        public override void ToDefault(bool fasted = false)
        {
            base.Stop();
            base.ToDefault(fasted);
            if (isFrom)
            {
                if (fasted) rectTransform.anchoredPosition= from;
                else rectTransform.LeanMove(from, time).setEase(type).setIgnoreTimeScale(base.useUnScaledTime);
            }
        }
    }
}

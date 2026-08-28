using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class ValueThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom)), Range(0f, 1f)] private float from;
        [SerializeField, Range(0f, 1f)] private float to = 1;
        
        private CanvasGroup canvasGroup;
        private WorldCanvasGroup worldCanvasGroup;

        private void Awake()
        {
            TryInitialize();
            ApplyValueInstant();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ApplyValueInstant();
        }

        private void TryInitialize()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (worldCanvasGroup == null)
                worldCanvasGroup = GetComponent<WorldCanvasGroup>();
        }

        private void ApplyValueInstant()
        {
            if (!isFrom && !autoPlay) return;
            if (!autoPlay) return;
            
            if (canvasGroup) canvasGroup.alpha = from;
            if (worldCanvasGroup) worldCanvasGroup.alpha = from;
        }

        public void Set(float fromValue, float toValue, bool instant = false, Action onComplete = null)
        {
            TryInitialize();
            if (instant)
            {
                if (canvasGroup) canvasGroup.alpha = toValue;
                if (worldCanvasGroup) worldCanvasGroup.alpha = toValue;
                Complete(onComplete);
            }
            else
            {
                if (canvasGroup) canvasGroup.alpha = fromValue;
                if (worldCanvasGroup) worldCanvasGroup.alpha = fromValue;
                
                Stop();
                descr = Fade(fromValue, toValue);
                base.Play(onComplete);
            }
        }

        public void Set(float value, bool instant = false, Action onComplete = null)
        {
            TryInitialize();
            if (instant)
            {
                if (canvasGroup) canvasGroup.alpha = value;
                if (worldCanvasGroup) worldCanvasGroup.alpha = value;
                Complete(onComplete);
            }
            else
            {
                Stop();
                descr = Fade(GetFrom(), value);
                base.Play(onComplete);
            }
        }

        public float Get()
        {
            if (canvasGroup) return canvasGroup.alpha;
            if (worldCanvasGroup) return worldCanvasGroup.alpha;
            return 0;
        }

        public override void Play(Action onComplete = null)
        {
            Stop();
            descr = Fade(GetFrom(), to);
            base.Play(onComplete);
        }

        public override void ToDefault(bool instant = false, Action onComplete = null)
        {
            Stop();
            if (instant)
            {
                var _target = GetFrom();
                if (canvasGroup) canvasGroup.alpha = _target;
                if (worldCanvasGroup) worldCanvasGroup.alpha = _target;
                base.ToDefault(true, onComplete);
            }
            else
            {
                descr = Fade(to, GetFrom());
                base.ToDefault(false, onComplete);
            }
        }

        private LTDescr Fade(float fromValue, float toValue)
        {
            return gameObject.LeanValue(fromValue, toValue, time).setOnUpdate(v => 
            { 
                if (canvasGroup) canvasGroup.alpha = v; 
                if (worldCanvasGroup) worldCanvasGroup.alpha = v; 
            });
        }

        private float GetFrom()
        {
            if (isFrom) return from;
            if (canvasGroup) return canvasGroup.alpha;
            if (worldCanvasGroup) return worldCanvasGroup.alpha;
            return 0;
        }
    }
}

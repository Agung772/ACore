using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ACore.Animation
{
    public class AnimationBase : MonoBehaviour
    {
        [SerializeField] protected bool autoPlay = true;
        [SerializeField] protected bool useUnScaledTime;
        
        [SerializeField] protected bool isLoop;
        [SerializeField, ShowIf(nameof(isLoop))] protected int loopCount = -1;
        [SerializeField, ShowIf(nameof(isLoop))] protected LeanTweenType loopType = LeanTweenType.clamp;
        
        [PropertyOrder(100)] private UnityEvent onComplete;
        
        protected LTDescr descr;
        
        protected virtual void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

        public virtual void Play(Action onComplete = null)
        {
            if (descr == null) return;

            if (isLoop)
            {
                switch (loopType)
                {
                    case LeanTweenType.pingPong:
                        descr.setLoopPingPong(loopCount);
                        break;
                    case LeanTweenType.clamp:
                        descr.setLoopCount(loopCount);
                        break;
                    case LeanTweenType.once:
                        descr.setLoopOnce();
                        break;
                    default:
                        descr.setLoopCount(loopCount);
                        break;
                }
            }
            
            descr.setIgnoreTimeScale(useUnScaledTime);
            descr.setOnComplete(() =>
            {
                Complete(onComplete);
            });
        }
        
        public virtual void Stop()
        {
            if (descr != null && descr.uniqueId > 0)
            {
                gameObject.LeanCancel(descr.uniqueId);
            }
            descr = null;
        }

        public virtual void ToDefault(bool instant = false, Action onComplete = null)
        {
            if (instant || descr == null)
            {
                Complete(onComplete);
                return;
            }

            descr.setIgnoreTimeScale(useUnScaledTime);
            descr.setOnComplete(() =>
            {
                Complete(onComplete);
            });
        }

        public void Complete(Action onOtherComplete)
        {
            onComplete?.Invoke();
            onOtherComplete?.Invoke();
        }
    }
}

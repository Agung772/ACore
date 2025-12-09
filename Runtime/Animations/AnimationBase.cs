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
        
        [PropertyOrder(-100)] public UnityEvent onComplete;
        
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

        public virtual void Play()
        {
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
            
            descr.setOnComplete(() => onComplete?.Invoke());
        }
        
        public virtual void Stop()
        {
            if (descr != null)
            {
                gameObject.LeanCancel(descr.id);
            }
        }
        
        public virtual void ToDefault(bool fasted = false) { }
    }
}

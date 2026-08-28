using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ACore.Animation
{
    public class ShakeThis : AnimationBase
    {
        private Vector3 from;

        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private float shakeOffsetStrength = 0.1f;
        [SerializeField] private float returnDuration = 0.2f;
        
        public override void Play(Action onComplete = null)
        {
            base.Stop();
            from = transform.localPosition;
            base.descr = gameObject.LeanMoveLocal(from, moveDuration)
                .setEase(LeanTweenType.easeShake)
                .setFrom(from + Random.insideUnitSphere * shakeOffsetStrength);

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
                gameObject.LeanMoveLocal(from, returnDuration)
                    .setIgnoreTimeScale(useUnScaledTime)
                    .setOnComplete(() =>
                    {
                        Complete(onComplete);
                    });
            });
        }

        public override void ToDefault(bool instant = false, Action onComplete = null)
        {
            base.Stop();
            if (instant)
            {
                transform.localPosition = from;
                Complete(onComplete);
            }
            else
            {
                base.descr = gameObject.LeanMoveLocal(from, returnDuration);
                base.ToDefault(false, onComplete);
            }
        }
    }
}

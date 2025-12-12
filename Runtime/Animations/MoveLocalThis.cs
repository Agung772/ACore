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

        public override void Play()
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            { 
                transform.localPosition = from;
            }
            base.descr = gameObject.LeanMoveLocal(to, time);
            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            if (!isFrom)
            {
                Debug.LogWarning("To Default not available because it is not from.");
                return;
            }
            
            base.Stop();
            
            if (fasted) transform.localPosition = from;
            else base.descr = gameObject.LeanMoveLocal(from, time);
            
            base.ToDefault(fasted);
        }
    }
}

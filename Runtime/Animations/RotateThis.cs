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
        public override void Play()
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            {
                transform.eulerAngles = from;
            }
            base.descr = gameObject.LeanRotateAroundLocal(to, add, time);
            base.Play();
        }
        
        public override void ToDefault(bool fasted = false)
        {
            base.Stop();
            base.ToDefault(fasted);
            if (isFrom)
            {
                if (fasted) transform.eulerAngles = from;
                else gameObject.LeanRotateAroundLocal(from, add, time);
            }
        }
    }
}


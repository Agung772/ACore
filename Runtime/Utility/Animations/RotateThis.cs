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
            if (!isFrom)
            {
                Debug.LogWarning("To Default not available because it is not from.");
                return;
            }
            
            base.Stop();
            
            if (fasted) transform.eulerAngles = from;
            else base.descr = gameObject.LeanRotateAroundLocal(from, add, time);
            
            base.ToDefault(fasted);
        }
    }
}


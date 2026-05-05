using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class ScaleThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom))] private Vector3 from;
        [SerializeField] private Vector3 to;

        private void Awake()
        {
            if (isFrom && !base.autoPlay)
            {
                transform.localScale = from;
            }
        }
        
        public override void Play()
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            {
                transform.localScale = from;
            }

            base.descr = gameObject.LeanScale(to, time);
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
            
            if (fasted) transform.localScale = from;
            else base.descr = gameObject.LeanScale(from, time);
            
            base.ToDefault(fasted);
        }
    }
}

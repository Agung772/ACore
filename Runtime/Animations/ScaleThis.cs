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
            base.Stop();
            base.ToDefault(fasted);
            if (isFrom)
            {
                if (fasted) transform.localScale = from;
                else gameObject.LeanScale(from, time);
            }
        }
    }
}

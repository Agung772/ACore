using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore.Animation
{
    public class ValueThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom)), MinMaxSlider(0, 1)] private float from;
        [SerializeField, MinMaxSlider(0, 1)] private float to = 1;
        
        private CanvasGroup canvasGroup;
        private WorldCanvasGroup worldCanvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            worldCanvasGroup = GetComponent<WorldCanvasGroup>();
            
            if (isFrom && !autoPlay) ApplyValueInstant();
        }
        
        private void ApplyValueInstant()
        {
            if (canvasGroup) canvasGroup.alpha = from;
            if (worldCanvasGroup) worldCanvasGroup.alpha = from;
        }

        public override void Play()
        {
            Stop();
            descr = Fade(GetFrom(), to);
            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            Stop();
            descr = Fade(to, GetFrom());
            base.ToDefault(fasted);
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

using UnityEngine;

namespace ACore
{
    public class WaitForSecondsDeltaTime : CustomYieldInstruction
    {
        private float timer;

        public WaitForSecondsDeltaTime(float seconds)
        {
            timer = seconds > 0f ? seconds : 0f;
        }
        
        public override bool keepWaiting
        {
            get
            {
                if (timer <= 0f) return false;
                timer -= Time.deltaTime;
                return timer > 0f;
            }
        }
    }
}

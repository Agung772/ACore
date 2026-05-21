using UnityEngine;

namespace ACore
{
    public class WaitForSecondsDeltaTime : CustomYieldInstruction
    {
        private float timer;

        public WaitForSecondsDeltaTime(float seconds)
        {
            timer = seconds;
        }
        
        public override bool keepWaiting
        {
            get
            {
                timer -= Time.deltaTime;
                return timer > 0f;
            }
        }
    }
}
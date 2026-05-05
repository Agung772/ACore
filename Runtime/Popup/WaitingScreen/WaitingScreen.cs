using UnityEngine;

namespace ACore
{
    public class WaitingScreen : UIBehaviour
    {
        public override void Awake()
        {
            base.Awake();
            Time.timeScale = 0f;
        }

        public override void Remove()
        {
            base.Remove();
            Time.timeScale = 1f;
        }
    }
}

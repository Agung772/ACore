using UnityEngine;

namespace ACore
{
    public class LoadingScreenPopup : PopupBehaviour
    {
        public override void Initialize()
        {
            base.Initialize();
            Time.timeScale = 0f;
        }

        public override void OnClose()
        {
            base.OnClose();
            Time.timeScale = 1f;
        }
    }
}

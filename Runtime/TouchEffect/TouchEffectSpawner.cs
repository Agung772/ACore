using UnityEngine;

namespace ACore
{
    public class TouchEffectSpawner : GlobalBehaviour
    {
        public override void PostInitialize()
        {
            if (POPUP.Resources.ContainsKey(typeof(TouchEffect)))
            {
                GAME.Manager.OnUpdate += OnUpdate;
            }
        }

        private void OnUpdate()
        {
#if UNITY_EDITOR 
            if (Input.GetMouseButtonDown(0))
            {
                var _mousePos = Input.mousePosition;
                SpawnEffectUI(_mousePos);
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var _touchPos = Input.GetTouch(0).position;
                SpawnEffectUI(_touchPos);
            }
#endif
        }

        private void SpawnEffectUI(Vector3 screenPos)
        {
            var _effect = POPUP.Show<TouchEffect>();
            _effect.SetPositionByScreenPoint(screenPos);
        }
    }
}

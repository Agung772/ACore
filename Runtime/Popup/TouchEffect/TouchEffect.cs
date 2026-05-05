using UnityEngine;

namespace ACore
{
    public class TouchEffect : UIBehaviour
    {
        [SerializeField] private GameObject prefab;
        
        private new Camera camera;
        private RectTransform rect;

        public override void Awake()
        {
            base.Awake();
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
#if UNITY_EDITOR 
            if (Input.GetMouseButtonDown(0))
            {
                var _mousePos = Input.mousePosition;
                Spawn(_mousePos);
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var _touchPos = Input.GetTouch(0).position;
                Spawn(_touchPos);
            }
#endif
        }

        private void Spawn(Vector3 screenPos)
        {
            var _effect = OBJECT.Show<TouchEffect>();
            _effect.SetPositionByScreenPoint(screenPos);
        }
        
        public void SetPositionByWorld(Vector3 position)
        {
            var _screenPos = camera.WorldToScreenPoint(position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                _screenPos,
                camera,
                out var _localPos
            );

            rect.anchoredPosition = _localPos;
        }

        public void SetPositionByScreenPoint(Vector3 position)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                position,
                camera,
                out var _localPos
            );

            rect.anchoredPosition = _localPos;
        }
    }
}

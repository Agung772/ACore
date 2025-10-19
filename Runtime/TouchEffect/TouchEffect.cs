using UnityEngine;

namespace ACore
{
    public class TouchEffect : MultiPopupBehaviour
    {
        public void SetPositionByWorld(Vector3 position)
        {
            var _canvas = Game.Manager.Canvas.GetComponent<Canvas>();
            var _cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;
            var _screenPos = Camera.main.WorldToScreenPoint(position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                _screenPos,
                _cam,
                out var _localPos
            );

            GetComponent<RectTransform>().anchoredPosition = _localPos;
        }

        public void SetPositionByScreenPoint(Vector3 position)
        {
            var _canvas = Game.Manager.Canvas.GetComponent<Canvas>();
            var _cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                position,
                _cam,
                out var _localPos
            );

            GetComponent<RectTransform>().anchoredPosition = _localPos;
        }
    }
}

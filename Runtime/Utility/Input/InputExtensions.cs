using UnityEngine;
using UnityEngine.EventSystems;

namespace ACore
{
    public class InputExtensions : MonoBehaviour
    {
        private static EventSystem EventSystem => EventSystem.current;

        public static bool IsPointerDown(bool ignoreUI = true)
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
                return !ignoreUI || !IsOverUI();
    #endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Began)
                    return !ignoreUI || !IsOverUI(_t.fingerId);
            }

            return false;
        }

        public static bool IsPointerHeld(bool ignoreUI = true)
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
                return !ignoreUI || !IsOverUI();
    #endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Moved || _t.phase == TouchPhase.Stationary)
                    return !ignoreUI || !IsOverUI(_t.fingerId);
            }

            return false;
        }

        public static bool IsPointerUp(bool ignoreUI = true)
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0))
                return !ignoreUI || !IsOverUI();
    #endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Ended || _t.phase == TouchPhase.Canceled)
                    return !ignoreUI || !IsOverUI(_t.fingerId);
            }

            return false;
        }

        public static bool IsPointerDrag(bool ignoreUI = true)
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) &&
                (Input.mousePosition != lastMousePos))
            {
                lastMousePos = Input.mousePosition;
                return !ignoreUI || !IsOverUI();
            }
    #endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Moved)
                    return !ignoreUI || !IsOverUI(_t.fingerId);
            }

            return false;
        }

        public static bool IsAnyPointerActive(bool ignoreUI = true)
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
                return !ignoreUI || !IsOverUI();
    #endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                return !ignoreUI || !IsOverUI(_t.fingerId);
            }

            return false;
        }

        private static Vector3 lastMousePos;

        private static bool IsOverUI(int fingerId = -1)
        {
            if (EventSystem == null)
                return false;

            return fingerId >= 0
                ? EventSystem.IsPointerOverGameObject(fingerId)
                : EventSystem.IsPointerOverGameObject();
        }
    }
}

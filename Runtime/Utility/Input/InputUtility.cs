using UnityEngine;
using UnityEngine.EventSystems;

namespace ACore
{
    public static class InputUtility
    {
        private static EventSystem EventSystem => EventSystem.current;
        private static Vector3 lastMousePosition;

        public static bool IsPointerActive()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && IsOverUI())
                return true;
#endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (IsOverUI(_t.fingerId))
                    return true;
            }

            return false;
        }
        private static bool IsOverUI(int fingerId = -1)
        {
            if (EventSystem == null)
                return false;

            return fingerId >= 0
                ? EventSystem.IsPointerOverGameObject(fingerId)
                : EventSystem.IsPointerOverGameObject();
        }

        public static bool IsPointerDown()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0) && IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Began && IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsPointerHold()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if ((_t.phase == TouchPhase.Moved || _t.phase == TouchPhase.Stationary) && IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsPointerUp()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0) && IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if ((_t.phase == TouchPhase.Ended || _t.phase == TouchPhase.Canceled) && IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsPointerDrag()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && IsOverUI() && Input.mousePosition != lastMousePosition)
            {
                lastMousePosition = Input.mousePosition;
                return true;
            }
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Moved && IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsMouseButtonActive()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && !IsOverUI())
                return true;
#endif

            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (!IsOverUI(_t.fingerId))
                    return true;
            }

            return false;
        }
        public static bool IsMouseButtonDown()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0) && !IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Began && !IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsMouseButtonHold()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && !IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if ((_t.phase == TouchPhase.Moved || _t.phase == TouchPhase.Stationary) && !IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsMouseButtonUp()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0) && !IsOverUI())
                return true;
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if ((_t.phase == TouchPhase.Ended || _t.phase == TouchPhase.Canceled) && !IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }

        public static bool IsMouseButtonDrag()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && !IsOverUI() && Input.mousePosition != lastMousePosition)
            {
                lastMousePosition = Input.mousePosition;
                return true;
            }
#endif
            if (Input.touchCount > 0)
            {
                var _t = Input.GetTouch(0);
                if (_t.phase == TouchPhase.Moved && !IsOverUI(_t.fingerId))
                    return true;
            }
            return false;
        }
    }
}

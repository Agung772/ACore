using UnityEngine;

namespace ACore
{
    public static class TransformExtensions
    {
        public static void DestroyAllChildren(this Transform parent)
        {
            if (parent == null) return;
            
            for (int _i = parent.childCount - 1; _i >= 0; _i--)
            {
                var _child = parent.GetChild(_i).gameObject;
                DestroyObject(_child);
            }
        }
        
        public static void DestroyFirstChild(this Transform parent)
        {
            if (parent == null) return;
            if (parent.childCount == 0) return;

            var _child = parent.GetChild(0).gameObject;
            DestroyObject(_child);
        }
        
        public static void DestroyLastChild(this Transform parent)
        {
            if (parent == null) return;
            if (parent.childCount == 0) return;

            var _child = parent.GetChild(parent.childCount - 1).gameObject;
            DestroyObject(_child);
        }

        private static void DestroyObject(GameObject go)
        {
            if (Application.isPlaying)
                Object.Destroy(go);
            else
                Object.DestroyImmediate(go);
        }

        public static Vector3 GetCenter(this Transform[] transforms)
        {
            if (transforms == null || transforms.Length == 0)
                return Vector3.zero;

            var _sum = Vector3.zero;

            for (int _i = 0; _i < transforms.Length; _i++)
            {
                if (transforms[_i] != null)
                    _sum += transforms[_i].position;
            }

            return _sum / transforms.Length;
        }
    }
}
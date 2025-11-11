using UnityEngine;

namespace ACore
{
    public static class TransformExtensions
    {
        public static void DestroyAllChildren(this Transform parent, bool immediate = false)
        {
            if (parent == null) return;
            
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var _child = parent.GetChild(i).gameObject;
                DestroyObject(_child, immediate);
            }
        }
        
        public static void DestroyFirstChild(this Transform parent, bool immediate = false)
        {
            if (parent == null) return;
            if (parent.childCount == 0) return;

            var _child = parent.GetChild(0).gameObject;
            DestroyObject(_child, immediate);
        }
        
        public static void DestroyLastChild(this Transform parent, bool immediate = false)
        {
            if (parent == null) return;
            if (parent.childCount == 0) return;

            var _child = parent.GetChild(parent.childCount - 1).gameObject;
            DestroyObject(_child, immediate);
        }

        private static void DestroyObject(GameObject go, bool immediate)
        {
            if (immediate)
            {
                Object.DestroyImmediate(go);
            }
            else
            {
                Object.Destroy(go);
            }
        }
    }
}

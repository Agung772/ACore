using UnityEngine;

namespace ACore
{
    public static class Vector2Extensions
    {
        public static Vector2 Random(this Vector2Range range)
        {
            return new Vector2(
                UnityEngine.Random.Range(range.min.x, range.max.x),
                UnityEngine.Random.Range(range.min.y, range.max.y)
            );
        }
    }
}
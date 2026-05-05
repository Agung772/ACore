using UnityEngine;

namespace ACore
{
    public static class Vector3Extensions
    {
        public static Vector3 Random(this Vector3Range range)
        {
            return new Vector3(
                UnityEngine.Random.Range(range.min.x, range.max.x),
                UnityEngine.Random.Range(range.min.y, range.max.y),
                UnityEngine.Random.Range(range.min.z, range.max.z)
            );
        }
    }
}
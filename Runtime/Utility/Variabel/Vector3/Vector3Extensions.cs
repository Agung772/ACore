using UnityEngine;

namespace ACore
{
    public static class Vector3Extensions
    {
        public static Vector3 RandomRange(this Vector3Range range)
        {
            return new Vector3(
                Random.Range(range.min.x, range.max.x),
                Random.Range(range.min.y, range.max.y),
                Random.Range(range.min.z, range.max.z)
            );
        }
        
        public static Vector3 RandomSign(this Vector3Range range)
        {
            return new Vector3(
                Random.value > 0.5f ? range.min.x : range.max.x,
                Random.value > 0.5f ? range.min.y : range.max.y,
                Random.value > 0.5f ? range.min.z : range.max.z
            );
        }
        
        public static Vector3 RandomNonCenter(this Vector3Range range, float excludePercent)
        {
            var _minExclude = new Vector3(
                range.min.x * excludePercent,
                range.min.y * excludePercent,
                range.min.z * excludePercent);
            
            var _maxExclude = new Vector3(
                range.max.x * excludePercent,
                range.max.y * excludePercent,
                range.max.z * excludePercent);
            
            return new Vector3(
                Random.value > 0.5f ? Random.Range(_minExclude.x, range.min.x) : Random.Range(_maxExclude.x, range.max.x),
                Random.value > 0.5f ? Random.Range(_minExclude.y, range.min.y) : Random.Range(_maxExclude.y, range.max.y),
                Random.value > 0.5f ? Random.Range(_minExclude.z, range.min.z) : Random.Range(_maxExclude.z, range.max.z)
            );
        }
    }
}
using UnityEngine;

namespace ACore
{
    public static class FloatRangeExtensions
    {
        public static float RandomRange(this FloatRange range)
        {
            return Random.Range(range.min, range.max);
        }
        
        public static float RandomSign(this FloatRange range)
        {
            return Random.value > 0.5f ? range.min : range.max;
        }
        
        public static Vector2 RandomNonCenter(this Vector2Range range, float excludePercent)
        {
            var _minExclude = new Vector2(
                range.min.x * excludePercent,
                range.min.y * excludePercent);
            
            var _maxExclude = new Vector2(
                range.max.x * excludePercent,
                range.max.y * excludePercent);
            
            return new Vector2(
                Random.value > 0.5f ? Random.Range(_minExclude.x, range.min.x) : Random.Range(_maxExclude.x, range.max.x),
                Random.value > 0.5f ? Random.Range(_minExclude.y, range.min.y) : Random.Range(_maxExclude.y, range.max.y)
            );
        }
    }
}
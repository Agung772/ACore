using UnityEngine;

namespace ACore
{
    public static class IntRangeExtensions
    {
        public static int RandomRange(this IntRange range)
        {
            return Random.Range(range.min, range.max);
        }
        
        public static float RandomSign(this FloatRange range)
        {
            return Random.value > 0.5f ? range.min : range.max;
        }
        
        public static float RandomNonCenter(this FloatRange range, float excludePercent)
        {
            var _minExclude = range.min * excludePercent;
            var _maxExclude = range.max * excludePercent;

            return Random.value > 0.5f ? Random.Range(_minExclude, range.min) : Random.Range(_maxExclude, range.max);
        }
    }
}
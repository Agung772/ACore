using System;
using Sirenix.OdinInspector;

namespace ACore
{
    [Serializable, InlineProperty]
    public struct FloatRange
    {
        [HorizontalGroup("Range")]
        public float min;
        [HorizontalGroup("Range")]
        public float max;
        public void Set(float minValue, float maxValue) { min = minValue; max = maxValue; }
        public void Set(float value) { min = value; max = value; }
        public void Add(float value) { min += value; max += value; }
    }
}
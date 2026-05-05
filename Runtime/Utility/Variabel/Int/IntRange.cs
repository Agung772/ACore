using System;
using Sirenix.OdinInspector;

namespace ACore
{
    [Serializable, InlineProperty]
    public struct IntRange
    {
        [HorizontalGroup("Range")]
        public int min;
        [HorizontalGroup("Range")]
        public int max;
        public void Set(int minValue, int maxValue) { min = minValue; max = maxValue; }
        public void Set(int value) { min = value; max = value; }
        public void Add(int value) { min += value; max += value; }
    }
}
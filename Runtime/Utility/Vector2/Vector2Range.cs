using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore
{
    [Serializable, InlineProperty]
    public struct Vector2Range
    {
        [VerticalGroup("Range")]
        public Vector2 min;

        [VerticalGroup("Range")]
        public Vector2 max;

        public void Set(Vector2 minValue, Vector2 maxValue)
        {
            min = minValue;
            max = maxValue;
        }

        public void Set(Vector2 value)
        {
            min = value;
            max = value;
        }

        public void Add(Vector2 value)
        {
            min += value;
            max += value;
        }
    }
}
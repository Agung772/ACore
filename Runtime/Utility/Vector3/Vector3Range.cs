using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore
{
    [Serializable, InlineProperty]
    public struct Vector3Range
    {
        [VerticalGroup("Range")]
        public Vector3 min;

        [VerticalGroup("Range")]
        public Vector3 max;

        public void Set(Vector3 minValue, Vector3 maxValue)
        {
            min = minValue;
            max = maxValue;
        }

        public void Set(Vector3 value)
        {
            min = value;
            max = value;
        }

        public void Add(Vector3 value)
        {
            min += value;
            max += value;
        }
    }
}
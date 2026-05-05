namespace ACore
{
    public static class FloatRangeExtensions
    {
        public static float Random(this FloatRange range)
        {
            return UnityEngine.Random.Range(range.min, range.max);
        }
    }
}
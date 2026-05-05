namespace ACore
{
    public static class IntRangeExtensions
    {
        public static int Random(this IntRange range)
        {
            return UnityEngine.Random.Range(range.min, range.max);
        }
    }
}
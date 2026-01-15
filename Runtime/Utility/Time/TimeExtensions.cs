using UnityEngine;

namespace ACore
{
    public static class TimeExtensions
    {
        /// <summary>
        /// MM:SS → 02:35
        /// </summary>
        public static string ToMinuteSecond(this float time)
        {
            time = Mathf.Max(time, 0f);

            var _minutes = Mathf.FloorToInt(time / 60f);
            var _seconds = Mathf.FloorToInt(time % 60f);

            return $"{_minutes:00}:{_seconds:00}";
        }

        /// <summary>
        /// HH:MM → 01:25
        /// </summary>
        public static string ToHourMinute(this float time)
        {
            time = Mathf.Max(time, 0f);

            var _hours = Mathf.FloorToInt(time / 3600f);
            var _minutes = Mathf.FloorToInt((time % 3600f) / 60f);

            return $"{_hours:00}:{_minutes:00}";
        }

        /// <summary>
        /// HH:MM:SS → 01:25:42
        /// </summary>
        public static string ToHourMinuteSecond(this float time)
        {
            time = Mathf.Max(time, 0f);

            var _hours = Mathf.FloorToInt(time / 3600f);
            var _minutes = Mathf.FloorToInt((time % 3600f) / 60f);
            var _seconds = Mathf.FloorToInt(time % 60f);

            return $"{_hours:00}:{_minutes:00}:{_seconds:00}";
        }

        /// <summary>
        /// MM only → 05
        /// </summary>
        public static int ToMinutes(this float time)
        {
            return Mathf.FloorToInt(Mathf.Max(time, 0f) / 60f);
        }

        /// <summary>
        /// SS only → 42
        /// </summary>
        public static int ToSeconds(this float time)
        {
            return Mathf.FloorToInt(Mathf.Max(time, 0f));
        }

        /// <summary>
        /// Stopwatch style → 2m 35s
        /// </summary>
        public static string ToStopwatch(this float time)
        {
            time = Mathf.Max(time, 0f);

            var _minutes = Mathf.FloorToInt(time / 60f);
            var _seconds = Mathf.FloorToInt(time % 60f);

            return _minutes > 0
                ? $"{_minutes}m {_seconds}s"
                : $"{_seconds}s";
        }
    }
}

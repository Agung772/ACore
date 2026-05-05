using System.Globalization;

namespace ACore
{
    public static class TextExtensions
    {
        private static readonly CultureInfo Culture = new("id-ID");

        public static string ToThousands(this int value)
        {
            return value.ToString("N0", Culture);
        }

        public static string ToThousands(this long value)
        {
            return value.ToString("N0", Culture);
        }
    }
}
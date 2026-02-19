using System.Linq;

namespace ACore
{
    public static class StringExtensions
    {
        public static string ToSpace(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return string.Concat(text.Select((ch, i) => 
                i > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));
        }
    }
}

using System.Text;

namespace ACore
{
    public static class StringExtensions
    {
        public static string ToSpace(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var _length = text.Length;
            var _sb = new StringBuilder(_length + 8);

            for (var _i = 0; _i < _length; _i++)
            {
                var _current = text[_i];

                if (_i > 0)
                {
                    var _prev = text[_i - 1];

                    if (
                        (char.IsUpper(_current) && char.IsLower(_prev)) ||
                        (char.IsDigit(_current) && !char.IsDigit(_prev)) ||
                        (!char.IsDigit(_current) && char.IsDigit(_prev)) ||
                        (char.IsUpper(_current) && char.IsUpper(_prev) && _i + 1 < _length && char.IsLower(text[_i + 1]))
                    )
                    {
                        _sb.Append(' ');
                    }
                }

                _sb.Append(_current);
            }

            return _sb.ToString();
        }
    }
}
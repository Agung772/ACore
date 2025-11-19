using System;
using System.Linq;

namespace ACore
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string text) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), text, true);
        }
        
        public static T ToEnumOrDefault<T>(this string text) where T : struct, Enum
        {
            return Enum.TryParse(text, true, out T _value) ? _value : default;
        }
        
        public static int GetLength<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Length;
        }
    
        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }
    
        public static string ToSpace(this Enum enumValue)
        {
            var _result = enumValue.ToString();
            if (string.IsNullOrEmpty(_result)) return _result;

            return string.Concat(_result.Select((ch, i) => 
                i > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));
        }
    }
}


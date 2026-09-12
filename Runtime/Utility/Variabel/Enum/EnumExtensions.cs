using System;

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
            return enumValue.ToString().ToSpace();
        }
        
        public static bool SameName(this Enum enum1, Enum enum2)
        {
            if (enum1 == null || enum2 == null) return false;
            return string.Equals(enum1.ToString(), enum2.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool SameValue(this Enum enum1, Enum enum2)
        {
            if (enum1 == null || enum2 == null) return false;
            return enum1.ToInt() == enum2.ToInt();
        }
    }
}


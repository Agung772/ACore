using System;

namespace ACore
{
    public static class ArrayExtensions
    {
        public static T[] SetLength<T>(T[] array, int length, T defaultValue = default)
        {
            var _newArray = new T[length];
            
            if (array == null || array.Length == 0)
            {
                for (int i = 0; i < length; i++)
                {
                    _newArray[i] = typeof(T).IsValueType
                        ? defaultValue
                        : (T)Activator.CreateInstance(typeof(T));
                }
                return _newArray;
            }
            
            Array.Copy(array, _newArray, Math.Min(array.Length, length));
            
            for (int i = array.Length; i < length; i++)
            {
                _newArray[i] = typeof(T).IsValueType
                    ? defaultValue
                    : (T)Activator.CreateInstance(typeof(T));
            }

            return _newArray;
        }

        public static T[] Copy<T>(this T[] array) where T : ICloneable
        {
            if (array == null || array.Length == 0)
            {
                return new T[0];
            }
    
            T[] _copy = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                _copy[i] = (T)array[i].Clone();
            }

            return _copy;
        }
    }
}

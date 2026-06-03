using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static class ScriptableObjectUtility
    {
        private static readonly Dictionary<Type, List<ScriptableObjectAuto>> Cache = new();

        public static T GetSO<T>() where T : ScriptableObjectAuto
        {
            var _list = GetAllSO<T>();
            return _list.Count > 0 ? _list[0] : null;
        }

        public static List<T> GetAllSO<T>() where T : ScriptableObjectAuto
        {
            CacheAllSO();

            if (!Cache.TryGetValue(typeof(T), out var _list))
            {
                return new List<T>();
            }

            var _result = new List<T>(_list.Count);

            foreach (var _item in _list)
            {
                _result.Add((T)_item);
            }

            return _result;
        }

        private static void CacheAllSO()
        {
            if (Cache.Count > 0)
                return;

            var _allSO = Resources.LoadAll<ScriptableObjectAuto>("");

            foreach (var _item in _allSO)
            {
                var _type = _item.GetType();

                while (_type != null && typeof(ScriptableObjectAuto).IsAssignableFrom(_type))
                {
                    if (!Cache.TryGetValue(_type, out var _list))
                    {
                        _list = new List<ScriptableObjectAuto>();
                        Cache.Add(_type, _list);
                    }

                    _list.Add(_item);

                    _type = _type.BaseType;
                }
            }
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
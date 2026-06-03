using System;
using System.Collections.Generic;
using System.Linq;
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
            var _type = typeof(T);

            if (!Cache.TryGetValue(_type, out var _list))
            {
                CacheAllSO();

                if (!Cache.TryGetValue(_type, out _list))
                {
                    return new List<T>();
                }
            }

            return _list.Cast<T>().ToList();
        }

        private static void CacheAllSO()
        {
            if (Cache.Count > 0) return;

            var _allSO = Resources.LoadAll<ScriptableObjectAuto>("");

            foreach (var _item in _allSO)
            {
                var _type = _item.GetType();

                if (!Cache.TryGetValue(_type, out var _list))
                {
                    _list = new List<ScriptableObjectAuto>();
                    Cache.Add(_type, _list);
                }

                _list.Add(_item);
            }
        }
    }
}
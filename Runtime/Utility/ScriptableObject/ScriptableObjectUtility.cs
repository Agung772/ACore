using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static class ScriptableObjectUtility
    {
        private static Dictionary<Type, ScriptableObjectAuto> so = new();
        
        public static T GetSO<T>() where T : ScriptableObjectAuto
        {
            var _type = typeof(T);

            if (so.TryGetValue(_type, out var _so))
            {
                return _so as T;
            }
            
            var _allSO = Resources.LoadAll<ScriptableObjectAuto>("");
            foreach (var _item in _allSO)
            {
                var _itemType = _item.GetType();
                so.TryAdd(_itemType, _item);
            }
                
            so.TryGetValue(_type, out _so);

            return _so as T;
        }
    }
}

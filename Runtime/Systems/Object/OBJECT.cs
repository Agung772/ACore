using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ACore
{
    public static class OBJECT
    {
        public static readonly Dictionary<Type, ObjectBehaviour> Active = new();
        public static readonly Dictionary<Type, ObjectBehaviour> Resources = new();

        public static void Initialize()
        {
            var _objs = UnityEngine.Resources.LoadAll<ObjectBehaviour>("");

            foreach (var _obj in _objs)
                Debug.Log($"[OBJECT] Loaded: {_obj.name} | {_obj.GetType().FullName}");

            foreach (var _obj in _objs)
                Resources.Add(_obj.GetType(), _obj);
        }

        public static T Show<T>(Transform parent = null) where T : ObjectBehaviour
        {
            var _prefab = (T)Resources[typeof(T)];
            return Show(_prefab, parent);
        }

        public static T Show<T>(T prefab, Transform parent = null) where T : ObjectBehaviour
        {
            var _obj = Spawn(prefab, parent);

            if (!_obj.canMulti)
            {
                Active.Add(_obj.GetType(), _obj);
            }

            return (T)_obj;
        }
        
        public static bool TryShow<T>(out T obj, Transform parent = null) where T : ObjectBehaviour
        {
            if (!IsActive<T>())
            {
                obj = Show<T>(parent); 
                return true;
            }

            obj = null;
            return false;
        }

        private static ObjectBehaviour Spawn(ObjectBehaviour prefab, Transform parent = null)
        {
            if (parent)
            {
                return Object.Instantiate(prefab, parent);
            }
            if (prefab.isGlobal)
            {
                return Object.Instantiate(prefab, GAME.Manager.transform);
            }
            
            return Object.Instantiate(prefab);
        }

        public static void RemoveOnLoaded(bool withGlobal = false)
        {
            foreach (var _popup in Active.Values.ToArray())
            {
                if (withGlobal || !_popup.isGlobal)
                {
                    _popup.Remove();
                }
            }
        }
        
        public static bool Remove<T>() where T : ObjectBehaviour
        {
            if (TryGet<T>(out var _obj))
            {
                _obj.Remove();
                return true;
            }
            
            return false;
        }
        
        public static void Remove(ObjectBehaviour obj) 
        {
            obj.Remove();
        }
        
        internal static void RemoveInternal(ObjectBehaviour obj)
        {
            if (!obj.canMulti)
            {
                Active.Remove(obj.GetType());
            }

            Object.Destroy(obj.gameObject);
        }
        
        public static bool IsActive<T>() where T : ObjectBehaviour
        {
            return Active.ContainsKey(typeof(T));
        }

        public static bool TryGet<T>(out T obj) where T : ObjectBehaviour
        {
            if (IsActive<T>())
            {
                obj = Active[typeof(T)] as T;
                return true;
            }
            
            obj = null;
            return false;
        }
        
        public static T Get<T>() where T : ObjectBehaviour
        {
            TryGet<T>(out var _obj);
            return _obj;
        }
    }
}
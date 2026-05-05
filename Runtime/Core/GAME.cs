using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ACore
{
    public static class GAME
    {
        public static GameManager Manager { get; internal set; }
        public static string CurrentScene { get; set; }
        private static Dictionary<Type, GlobalBehaviour> globals;
        private static Dictionary<Type, LocalBehaviour> locals = new();


        public static void Initialize()
        {
            CreateGlobal();
            SCENE.OnUnloaded += UnloadedLocal;
        }

        public static IEnumerator InitializeCoroutine()
        {
            foreach (var _global in globals.Values) { yield return _global.InitializeCoroutine(); }
        }
        
        private static void CreateGlobal()
        {
            OBJECT.Initialize();
            STORAGE.Initialize();
            
            globals = InstanceUtility.Create<GlobalBehaviour>();
            var _orderGlobal = OrderGlobal(globals.Values.ToArray());
            foreach (var _global in _orderGlobal) { _global.Initialize(); }
            foreach (var _global in _orderGlobal) { _global.PostInitialize(); }
        }

        private static GlobalBehaviour[] OrderGlobal(GlobalBehaviour[] global)
        {
            var _global = global.OrderBy(local => local.Order).ToArray();
            return _global;
        }

        private static void UnloadedLocal()
        {
            locals.Clear();
        }

        public static bool TryGet<T>(out T behaviour) where T : class, IBehaviour
        {
            if (typeof(GlobalBehaviour).IsAssignableFrom(typeof(T)))
            {
                if (globals.TryGetValue(typeof(T), out var _global) && _global is T _castedGlobal)
                {
                    behaviour = _castedGlobal;
                    return true;
                }

                behaviour = null;
                return false;
            }
            
            if (typeof(LocalBehaviour).IsAssignableFrom(typeof(T)))
            {
                if (locals.TryGetValue(typeof(T), out var _local) && _local is T _castedLocal)
                {
                    behaviour = _castedLocal;
                    return true;
                }

                if (Object.FindObjectOfType(typeof(T), true) is T _found)
                {
                    locals[typeof(T)] = (LocalBehaviour)(object)_found;
                    behaviour = _found;
                    return true;
                }
            }

            behaviour = null;
            return false;
        }
        
        public static T Get<T>() where T : class, IBehaviour
        {
            return TryGet<T>(out var _behaviour) ? _behaviour : null;
        }
        
        public static T GetSO<T>() where T : ScriptableObjectAuto
        {
            return ScriptableObjectUtility.GetSO<T>();
        }
        
        public static void Quit()
        {
            Application.Quit();
        }
    }
}


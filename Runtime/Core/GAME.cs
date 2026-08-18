using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public static IEnumerator Initialize()
        {
            Debug.Log($"[{nameof(ACore)}] Start Booting...");
            
            OBJECT.Initialize();
            STORAGE.Initialize();
            SCENE.OnUnloaded += UnloadedLocal;
            
            CreateGlobal();
            
            yield return InitializeCoroutine();
            Debug.Log($"[{nameof(ACore)}] Initialize Coroutine Completed");
            
            var _task = InitializeAsync();
            yield return new WaitUntil(() => _task.IsCompleted);
            Debug.Log($"[{nameof(ACore)}] Initialize Async Completed");
        }
        
        private static void CreateGlobal()
        {
            globals = InstanceUtility.Create<GlobalBehaviour>();
            var _orderGlobal = OrderGlobal(globals.Values.ToArray());
            foreach (var _global in _orderGlobal) { _global.Initialize(); }
            foreach (var _global in _orderGlobal) { _global.PostInitialize(); }
        }

        private static IEnumerator InitializeCoroutine()
        {
            foreach (var _global in globals.Values) { yield return _global.InitializeCoroutine(); }
            foreach (var _global in globals.Values) { yield return _global.PostInitializeCoroutine(); }
        }
        
        private static async Task InitializeAsync()
        {
            foreach (var _global in globals.Values) { await _global.InitializeAsync(); }
            foreach (var _global in globals.Values) { await _global.PostInitializeAsync(); }
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
        
        public static List<T> GetAllSO<T>() where T : ScriptableObjectAuto
        {
            return ScriptableObjectUtility.GetAllSO<T>();
        }
        
        public static void Quit()
        {
            Application.Quit();
        }
    }
}


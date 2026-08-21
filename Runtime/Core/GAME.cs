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
        
        public static IEnumerator Initialize(Action<float> onProgress = null)
        {
            SCENE.OnUnloaded += UnloadedLocal;
            globals = InstanceUtility.Create<GlobalBehaviour>();

            var _globals = globals.Values.ToArray();
            var _count = _globals.Length;
            
            InitializeGlobals(_globals, _count, onProgress);
            yield return InitializeCoroutines(_globals, _count, onProgress);
            yield return InitializeAsync(_globals, _count, onProgress);
            PostInitializeGlobals(_globals, _count, onProgress);
            yield return PostInitializeCoroutines(_globals, _count, onProgress);
            yield return PostInitializeAsync(_globals, _count, onProgress);
            
            onProgress?.Invoke(1f);
            yield return null;
        }

        private static void InitializeGlobals(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                globals[_i].Initialize();
                ReportProgress(onProgress, 0, _i, count);
            }
        }

        private static IEnumerator InitializeCoroutines(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                yield return globals[_i].InitializeCoroutine();
                ReportProgress(onProgress, 1, _i, count);
            }
        }

        private static IEnumerator InitializeAsync(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                var _task = globals[_i].InitializeAsync();
                while (!_task.IsCompleted) yield return null;
                if (_task.IsFaulted) throw _task.Exception;

                ReportProgress(onProgress, 2, _i, count);
            }
        }

        private static void PostInitializeGlobals(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                globals[_i].PostInitialize();
                ReportProgress(onProgress, 3, _i, count);
            }
        }

        private static IEnumerator PostInitializeCoroutines(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                yield return globals[_i].PostInitializeCoroutine();
                ReportProgress(onProgress, 4, _i, count);
            }
        }

        private static IEnumerator PostInitializeAsync(GlobalBehaviour[] globals, int count, Action<float> onProgress)
        {
            for (int _i = 0; _i < count; _i++)
            {
                var _task = globals[_i].PostInitializeAsync();
                while (!_task.IsCompleted) yield return null;
                if (_task.IsFaulted) throw _task.Exception;

                ReportProgress(onProgress, 5, _i, count);
            }
        }

        private static void ReportProgress(Action<float> onProgress, int phase, int index, int count)
        {
            onProgress?.Invoke((phase + (index + 1f) / count) / 6f);
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
        
        public static bool TryGetSO<T>(out T so) where T : ScriptableObjectAuto
        {
            so = ScriptableObjectUtility.GetSO<T>();
            return so != null;
        }

        public static bool TryGetAllSO<T>(out List<T> so) where T : ScriptableObjectAuto
        {
            so = ScriptableObjectUtility.GetAllSO<T>();
            return so != null;
        }
        
        public static void Quit()
        {
            Application.Quit();
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static Dictionary<GameObject, List<Coroutine>> Coroutines = new();

        private static void TryAddCoroutine(GameObject key, Coroutine routine)
        {
            if (key == null || routine == null) return;
            if (!Coroutines.ContainsKey(key))
            {
                Coroutines[key] = new List<Coroutine>();
            }
            Coroutines[key].Add(routine);
        }

        private static void RemoveCoroutine(GameObject key, Coroutine routine)
        {
            if (key == null || routine == null) return;
            if (!Coroutines.TryGetValue(key, out var _list)) return;
            _list.Remove(routine);
            if (_list.Count == 0) Coroutines.Remove(key);
        }

        private static Coroutine ExecuteCoroutine(GameObject key, IEnumerator routine)
        {
            if (key == null || routine == null || GAME.Manager == null) return null;

            Coroutine _coroutineHandle = null;

            IEnumerator Wrapper()
            {
                yield return routine;
                RemoveCoroutine(key, _coroutineHandle);
            }

            _coroutineHandle = GAME.Manager.StartCoroutine(Wrapper());
            return _coroutineHandle;
        }

        public static void StartCoroutine(this GameObject key, Func<IEnumerator> routineFunc)
        {
            if (key == null || routineFunc == null || GAME.Manager == null) return;
            var _coroutine = ExecuteCoroutine(key, routineFunc.Invoke());
            TryAddCoroutine(key, _coroutine);
        }

        public static void StartCoroutine(this GameObject key, float startDelay, Func<IEnumerator> routineFunc)
        {
            if (key == null || routineFunc == null || GAME.Manager == null) return;
            var _coroutine = ExecuteCoroutine(key, StartCoroutineDelayed(startDelay, routineFunc.Invoke()));
            TryAddCoroutine(key, _coroutine);
        }

        private static IEnumerator StartCoroutineDelayed(float startDelay, IEnumerator routine)
        {
            yield return new WaitForSeconds(startDelay);
            yield return routine;
        }

        public static void StopCoroutine(this GameObject key)
        {
            if (key == null || GAME.Manager == null) return;

            if (Coroutines.TryGetValue(key, out var _routines))
            {
                var _toStop = new List<Coroutine>(_routines);
                foreach (var _routine in _toStop)
                {
                    if (_routine != null)
                    {
                        GAME.Manager.StopCoroutine(_routine);
                    }
                }
                Coroutines.Remove(key);
            }

            if (CoroutinesWithID.TryGetValue(key, out var _dict))
            {
                var _kvpList = new List<KeyValuePair<string, List<Coroutine>>>(_dict);
                foreach (var _kvp in _kvpList)
                {
                    var _list = _kvp.Value;
                    if (_list == null) continue;

                    var _toStop = new List<Coroutine>(_list);
                    foreach (var _coroutine in _toStop)
                    {
                        if (_coroutine != null)
                        {
                            GAME.Manager.StopCoroutine(_coroutine);
                        }
                    }
                }

                CoroutinesWithID.Remove(key);
            }
        }
        
        public static bool IsCoroutine(this GameObject key)
        {
            if (key == null) return false;
            return Coroutines.ContainsKey(key) || CoroutinesWithID.ContainsKey(key);
        }
    }
}

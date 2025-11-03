using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static Dictionary<GameObject, Dictionary<string, Coroutine>> CoroutinesWithID = new ();

        private static void TryAddCoroutine(GameObject key, string id, Coroutine routine)
        {
            if (!CoroutinesWithID.TryGetValue(key, out var _dict))
            {
                _dict = new Dictionary<string, Coroutine>();
                CoroutinesWithID[key] = _dict;
            }

            if (_dict.TryGetValue(id, out var _existing) && _existing != null)
            {
                Game.Manager.StopCoroutine(_existing);
            }

            _dict[id] = routine;
        }

        private static void RemoveCoroutine(GameObject key, string id)
        {
            if (!CoroutinesWithID.TryGetValue(key, out var _dict)) return;
            _dict.Remove(id);
            if (_dict.Count == 0) CoroutinesWithID.Remove(key);
        }

        private static Coroutine ExecuteCoroutine(GameObject key, string id, IEnumerator routine)
        {
            Coroutine _handle = null;

            IEnumerator Wrapper()
            {
                yield return routine;
                RemoveCoroutine(key, id);
            }

            _handle = Game.Manager.StartCoroutine(Wrapper());
            return _handle;
        }

        public static void StartCoroutine(this GameObject key, string id, Func<IEnumerator> routineFunc)
        {
            var _coroutine = ExecuteCoroutine(key, id, routineFunc.Invoke());
            TryAddCoroutine(key, id, _coroutine);
        }

        public static void StopCoroutine(this GameObject key, string id)
        {
            if (Game.Manager == null) return;
            if (!CoroutinesWithID.TryGetValue(key, out var _dict)) return;

            if (_dict.TryGetValue(id, out var _coroutine) && _coroutine != null)
            {
                Game.Manager.StopCoroutine(_coroutine);
                _dict.Remove(id);
            }

            if (_dict.Count == 0) CoroutinesWithID.Remove(key);
        }

        public static bool IsCoroutine(this GameObject key, string id)
        {
            return CoroutinesWithID.TryGetValue(key, out var _dict) && _dict.ContainsKey(id);
        }
    }
}

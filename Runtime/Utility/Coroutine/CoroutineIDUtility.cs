using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static Dictionary<GameObject, Dictionary<string, List<Coroutine>>> CoroutinesWithID = new();

        private static void TryAddCoroutine(GameObject key, string id, Coroutine routine)
        {
            if (key == null || string.IsNullOrEmpty(id) || routine == null) return;

            if (!CoroutinesWithID.TryGetValue(key, out var _dict))
            {
                _dict = new Dictionary<string, List<Coroutine>>();
                CoroutinesWithID[key] = _dict;
            }

            if (!_dict.TryGetValue(id, out var _list))
            {
                _list = new List<Coroutine>();
                _dict[id] = _list;
            }

            _list.Add(routine);
        }

        private static void RemoveCoroutine(GameObject key, string id, Coroutine routine)
        {
            if (key == null || string.IsNullOrEmpty(id) || routine == null) return;
            if (!CoroutinesWithID.TryGetValue(key, out var _dict)) return;
            if (!_dict.TryGetValue(id, out var _list)) return;

            _list.Remove(routine);

            if (_list.Count == 0)
                _dict.Remove(id);

            if (_dict.Count == 0)
                CoroutinesWithID.Remove(key);
        }

        private static Coroutine ExecuteCoroutine(GameObject key, string id, IEnumerator routine)
        {
            if (key == null || string.IsNullOrEmpty(id) || routine == null || GAME.Manager == null) return null;

            Coroutine _handle = null;

            IEnumerator Wrapper()
            {
                yield return routine;
                RemoveCoroutine(key, id, _handle);
            }

            _handle = GAME.Manager.StartCoroutine(Wrapper());
            return _handle;
        }

        public static void StartCoroutine(this GameObject key, string id, Func<IEnumerator> routineFunc)
        {
            if (key == null || string.IsNullOrEmpty(id) || routineFunc == null || GAME.Manager == null) return;
            var _coroutine = ExecuteCoroutine(key, id, routineFunc.Invoke());
            TryAddCoroutine(key, id, _coroutine);
        }

        public static void StopCoroutine(this GameObject key, string id)
        {
            if (key == null || string.IsNullOrEmpty(id) || GAME.Manager == null) return;
            if (!CoroutinesWithID.TryGetValue(key, out var _dict)) return;
            if (!_dict.TryGetValue(id, out var _list)) return;

            var _toStop = new List<Coroutine>(_list);
            foreach (var _coroutine in _toStop)
            {
                if (_coroutine != null)
                    GAME.Manager.StopCoroutine(_coroutine);
            }

            _dict.Remove(id);

            if (_dict.Count == 0)
                CoroutinesWithID.Remove(key);
        }

        public static bool IsCoroutine(this GameObject key, string id)
        {
            if (key == null || string.IsNullOrEmpty(id)) return false;
            return CoroutinesWithID.TryGetValue(key, out var _dict) &&
                   _dict.TryGetValue(id, out var _list) &&
                   _list.Count > 0;
        }
    }
}

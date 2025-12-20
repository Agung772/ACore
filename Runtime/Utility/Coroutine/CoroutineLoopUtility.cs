using System;
using System.Collections;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static void StartCoroutineLoop(this GameObject key, Func<IEnumerator> routineFunc)
        {
            var _coroutine = ExecuteCoroutine(key, LoopCoroutine(key, routineFunc));
            TryAddCoroutine(key, _coroutine);
        }

        public static void StartCoroutineLoop(this GameObject key, float startDelay, Func<IEnumerator> routineFunc)
        {
            var _coroutine = ExecuteCoroutine(key, StartCoroutineDelayed(startDelay, LoopCoroutine(key, routineFunc)));
            TryAddCoroutine(key, _coroutine);
        }

        private static IEnumerator LoopCoroutine(GameObject key, Func<IEnumerator> routineFunc)
        {
            while (true)
            {
                if (key == null)
                {
                    Coroutines.Remove(key);
                    yield break;
                }

                yield return routineFunc();
            }
        }
    }
}
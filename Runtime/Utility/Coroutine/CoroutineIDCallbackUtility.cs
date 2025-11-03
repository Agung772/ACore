using System;
using System.Collections;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static void StartCoroutine(this GameObject key, string id, float startDelay, Action callBack)
        {
            var _coroutine = ExecuteCoroutine(key, id, CallBackCoroutine(key, id, startDelay, callBack));
            TryAddCoroutine(key, id, _coroutine);
        }

        private static IEnumerator CallBackCoroutine(GameObject key, string id, float startDelay, Action callBack)
        {
            yield return new WaitForSeconds(startDelay);

            if (key == null)
            {
                RemoveCoroutine(key, id);
                yield break;
            }

            callBack?.Invoke();
            RemoveCoroutine(key, id);
        }
    }
}
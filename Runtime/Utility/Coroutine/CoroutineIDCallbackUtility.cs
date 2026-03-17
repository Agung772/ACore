using System;
using System.Collections;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static void StartCoroutine(this GameObject key, string id, float startDelay, Action callBack)
        {
            Coroutine _handle = null;

            IEnumerator Wrapper()
            {
                yield return CallBackCoroutine(key, id, startDelay, callBack, _handle);
            }

            _handle = Game.Manager.StartCoroutine(Wrapper());
            TryAddCoroutine(key, id, _handle);
        }

        private static IEnumerator CallBackCoroutine(GameObject key, string id, float startDelay, Action callBack, Coroutine handle)
        {
            yield return new WaitForSeconds(startDelay);

            if (key == null)
            {
                RemoveCoroutine(key, id, handle);
                yield break;
            }

            callBack?.Invoke();
            RemoveCoroutine(key, id, handle);
        }
    }
}
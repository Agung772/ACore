using System;
using System.Collections;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static void StartCoroutine(this GameObject key, float startDelay, Action callBack)
        {
            if (key == null || callBack == null || GAME.Manager == null) return;

            Coroutine _handle = null;

            IEnumerator Wrapper()
            {
                yield return CallBackCoroutine(key, startDelay, callBack, _handle);
            }

            _handle = GAME.Manager.StartCoroutine(Wrapper());
            TryAddCoroutine(key, _handle);
        }

        private static IEnumerator CallBackCoroutine(GameObject key, float startDelay, Action callBack, Coroutine handle)
        {
            yield return new WaitForSeconds(startDelay);
            if (key == null)
            {
                RemoveCoroutine(key, handle);
                yield break;
            }
            
            callBack?.Invoke();
            RemoveCoroutine(key, handle);
        }
    }
}

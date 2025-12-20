using System;
using System.Collections;
using UnityEngine;

namespace ACore
{
    public static partial class CoroutineUtility
    {
        public static void StartCoroutineLoop(this GameObject key, float repeating, Action callBack)
        {
            var _coroutine = ExecuteCoroutine(key, LoopCallbackCoroutine(key, repeating, callBack)); 
            TryAddCoroutine(key, _coroutine);
        }
        public static void StartCoroutineLoop(this GameObject key, float repeating, float startDelay, Action callBack)
        {
            var _coroutine = ExecuteCoroutine(key, StartCoroutineDelayed(startDelay, LoopCallbackCoroutine(key, repeating, callBack))); 
            TryAddCoroutine(key, _coroutine);
        }
        private static IEnumerator LoopCallbackCoroutine(GameObject key, float repeating, Action callBack)
        {
            while (true)
            {
                if (key == null)
                {
                    Coroutines.Remove(key);
                    yield break;
                }
            
                callBack?.Invoke();
                yield return new WaitForSeconds(repeating);
            }
        }
    }
}

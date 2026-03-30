using System.Collections;
using TMPro;
using UnityEngine;

namespace ACore
{
    public static class TextExtensions
    {
        public static void AnimatedNumber(this TextMeshProUGUI text, int from, int to, float duration = 0.5f)
        {
            Game.Manager.StartCoroutine(AnimatedNumberCoroutine(text, from, to, duration));
        }

        public static IEnumerator AnimatedNumberCoroutine(this TextMeshProUGUI text, int from, int to, float duration = 0.5f)
        {
            var _time = 0f;

            while (_time < duration)
            {
                _time += Time.deltaTime;
                var _t = _time / duration;

                var _value = Mathf.RoundToInt(Mathf.Lerp(from, to, _t));
                text.text = _value.ToString("N0");

                yield return null;
            }

            text.text = to.ToString("N0");
        }
    }
}
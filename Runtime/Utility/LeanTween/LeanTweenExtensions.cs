using UnityEngine;
using UnityEngine.UI;
using System;

namespace ACore
{
    public static class LeanTweenExtensions
    {
        public static LTDescr LeanColor(this Image image, Color to, float time)
        {
            return LeanTween.value(image.gameObject, image.color, to, time)
                .setOnUpdate((Color c) => { image.color = c; });
        }
        
        public static LTDescr LeanNumber(this GameObject gameObject, int from, int to, 
            Action<int> onValueChanged, float duration = 0.5f)
        {
            return LeanTween.value(gameObject, from, to, duration).setOnUpdate(val => 
            {
                onValueChanged?.Invoke(Mathf.RoundToInt(val));
            });
        }
    }
}
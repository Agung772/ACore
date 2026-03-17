using UnityEngine;
using UnityEngine.UI;

namespace ACore
{
    public static class LeanTweenExtensions
    {
        public static LTDescr LeanColor(this Image image, Color to, float time)
        {
            return LeanTween.value(image.gameObject, image.color, to, time)
                .setOnUpdate((Color c) => { image.color = c; });
        }
    }
}
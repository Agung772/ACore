using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ACore.Animation
{
    public class ColorThis : AnimationTimeBase
    {
        [SerializeField] private bool isFrom;
        [SerializeField, ShowIf(nameof(isFrom))] private Color from = Color.white;
        [SerializeField] private Color to = Color.white;
        
        private SpriteRenderer spriteRenderer;
        private Image image;
        private Renderer meshRenderer;
        private Material meshMat;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            meshRenderer = GetComponent<Renderer>();
            if (meshRenderer) SetupMeshRenderer();

            if (isFrom && !base.autoPlay)
            {
                if (spriteRenderer) spriteRenderer.color = from;
                else if (image) image.color = from;
            }
        }

        private void SetupMeshRenderer()
        {
            meshMat = meshRenderer.material;
            if (isFrom && !base.autoPlay) meshMat.color = from;

            meshMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshMat.SetInt("_ZWrite", 0);
            meshMat.DisableKeyword("_ALPHATEST_ON");
            meshMat.EnableKeyword("_ALPHABLEND_ON");
            meshMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshMat.renderQueue = 3000;
        }

        public override void Play()
        {
            base.Stop();
            if (isFrom && base.autoPlay)
            {
                if (spriteRenderer) spriteRenderer.color = from;
                else if (image) image.color = from;
                else if (meshMat) meshMat.color = from;
            }

            if (spriteRenderer)
            {
                base.descr = gameObject.LeanColor(to, time);
            }
            else if (image)
            {
                base.descr = image.LeanColor(to, time);
            }
            else if (meshMat)
            {
                var _start = meshMat.color;
                base.descr = LeanTween.value(gameObject, _start, to, time)
                    .setOnUpdate((Color v) => { meshMat.color = v; });
            }

            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            base.Stop();
            base.ToDefault(fasted);

            if (!isFrom) return;

            if (fasted)
            {
                if (spriteRenderer) spriteRenderer.color = from;
                else if (image) image.color = from;
                else if (meshMat) meshMat.color = from;
            }
            else
            {
                if (spriteRenderer) gameObject.LeanColor(from, time);
                else if (image) image.LeanColor(from, time);
                else if (meshMat)
                {
                    var _cur = meshMat.color;
                    LeanTween.value(gameObject, _cur, from, time)
                        .setOnUpdate((Color v) => { meshMat.color = v; });
                }
            }
        }
    }
}

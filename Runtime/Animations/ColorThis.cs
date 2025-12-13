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

        private bool meshTransparentPrepared;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            meshRenderer = GetComponent<Renderer>();
            if (meshRenderer)
            {
                meshMat = meshRenderer.material;
            }

            if (isFrom && !base.autoPlay)
            {
                if (spriteRenderer) spriteRenderer.color = from;
                else if (image) image.color = from;
                else if (meshRenderer)
                {
                    meshMat.color = from;
                }
            }
        }

        private bool NeedTransparency()
        {
            if (isFrom) return from.a < 1f || to.a < 1f;
            return to.a < 1f;
        }

        private void PrepareMeshTransparencyIfNeeded()
        {
            if (!meshRenderer || meshTransparentPrepared) return;
            if (meshMat.color.a < 1f)
            {
                meshTransparentPrepared = true;
                return;
            }
            if (!NeedTransparency()) return;
            
            meshMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshMat.SetInt("_ZWrite", 0);
            meshMat.DisableKeyword("_ALPHATEST_ON");
            meshMat.EnableKeyword("_ALPHABLEND_ON");
            meshMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshMat.renderQueue = 3000;

            meshTransparentPrepared = true;
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
            else if (meshRenderer)
            {
                var _start = meshMat.color;
                base.descr = gameObject.LeanValue(_start, to, time)
                    .setOnUpdate(c => meshMat.color = c);
                base.descr.setOnStart(PrepareMeshTransparencyIfNeeded);
            }

            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            base.Stop();

            if (!isFrom)
            {
                Debug.LogWarning("To Default not available because it is not from.");
                return;
            }

            if (fasted)
            {
                if (spriteRenderer) spriteRenderer.color = from;
                else if (image) image.color = from;
                else if (meshMat) meshMat.color = from;
            }
            else
            {
                if (spriteRenderer) base.descr = gameObject.LeanColor(from, time);
                else if (image) base.descr = image.LeanColor(from, time);
                else if (meshMat)
                {
                    var _cur = meshMat.color;
                    base.descr = LeanTween.value(gameObject, _cur, from, time)
                        .setOnUpdate(c => meshMat.color = c);
                }
            }

            base.ToDefault(fasted);
        }
    }
}

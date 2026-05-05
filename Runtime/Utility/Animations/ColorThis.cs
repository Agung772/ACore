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
        private Material[] meshMats;
        private bool meshTransparentPrepared;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            meshRenderer = GetComponent<Renderer>();

            if (meshRenderer)
                meshMats = meshRenderer.materials;

            if (isFrom && !autoPlay)
                ApplyColorInstant(from);
        }

        private void ApplyColorInstant(Color color)
        {
            if (spriteRenderer) spriteRenderer.color = color;
            else if (image) image.color = color;
            else if (meshMats != null)
            {
                foreach (var _mat in meshMats)
                    SetMaterialColor(_mat, color);
            }
        }

        private void SetMaterialColor(Material mat, Color color)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
        }

        private Color GetCurrentColor()
        {
            if (meshMats == null || meshMats.Length == 0)
                return Color.white;

            var _mat = meshMats[0];

            if (_mat.HasProperty("_BaseColor"))
                return _mat.GetColor("_BaseColor");
            if (_mat.HasProperty("_Color"))
                return _mat.GetColor("_Color");

            return Color.white;
        }

        private bool NeedTransparency()
        {
            if (isFrom) return from.a < 1f || to.a < 1f;
            return to.a < 1f;
        }

        private void PrepareMeshTransparencyIfNeeded()
        {
            if (meshMats == null || meshTransparentPrepared) return;
            if (!NeedTransparency()) return;

            foreach (var _mat in meshMats)
            {
                if (_mat.HasProperty("_Surface"))
                {
                    _mat.SetFloat("_Surface", 1);
                    _mat.SetFloat("_Blend", 0);
                    _mat.SetFloat("_ZWrite", 0);
                    _mat.renderQueue = 3000;
                }
                else
                {
                    _mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _mat.SetInt("_ZWrite", 0);
                    _mat.DisableKeyword("_ALPHATEST_ON");
                    _mat.EnableKeyword("_ALPHABLEND_ON");
                    _mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    _mat.renderQueue = 3000;
                }
            }

            meshTransparentPrepared = true;
        }

        public override void Play()
        {
            Stop();

            if (isFrom && autoPlay)
                ApplyColorInstant(from);

            if (spriteRenderer)
            {
                descr = gameObject.LeanColor(to, time);
            }
            else if (image)
            {
                descr = image.LeanColor(to, time);
            }
            else if (meshRenderer)
            {
                var _startColor = GetCurrentColor();

                descr = LeanTween.value(gameObject, _startColor, to, time)
                    .setOnStart(PrepareMeshTransparencyIfNeeded)
                    .setOnUpdate(c =>
                    {
                        foreach (var _mat in meshMats)
                            SetMaterialColor(_mat, c);
                    });
            }

            base.Play();
        }

        public override void ToDefault(bool fasted = false)
        {
            Stop();

            if (!isFrom)
                return;

            if (fasted)
            {
                ApplyColorInstant(from);
            }
            else
            {
                if (spriteRenderer)
                {
                    descr = gameObject.LeanColor(from, time);
                }
                else if (image)
                {
                    descr = image.LeanColor(from, time);
                }
                else if (meshRenderer)
                {
                    var _current = GetCurrentColor();

                    descr = LeanTween.value(gameObject, _current, from, time)
                        .setOnUpdate(c =>
                        {
                            foreach (var _mat in meshMats)
                                SetMaterialColor(_mat, c);
                        });
                }
            }

            base.ToDefault(fasted);
        }
    }
}

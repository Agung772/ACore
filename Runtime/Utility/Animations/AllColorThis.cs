using System;
using UnityEngine;
using UnityEngine.UI;

namespace ACore.Animation
{
    [ExecuteAlways]
    public class AllColorThis : AnimationTimeBase
    {
        [SerializeField] private Color from = Color.white;
        [SerializeField] private Color to = Color.white;

        private SpriteRenderer[] spriteRenderers;
        private Image[] images;
        private Renderer[] renderers;

        private Material[][] meshMats;
        private bool meshTransparentPrepared;

        public AllColorThis()
        {
            autoPlay = false;
        }

        private void Awake()
        {
            CacheTargets();

            if (Application.isPlaying)
            {
                if (!autoPlay)
                    ApplyColorInstant(from);
            }
            else
            {
                ApplyColorInstant(from);
            }
        }

        private void OnEnable()
        {
            CacheTargets();

            if (!Application.isPlaying)
                ApplyColorInstant(from);
        }

        private void OnValidate()
        {
            CacheTargets();
            ApplyColorInstant(from);
        }

        private void CacheTargets()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            images = GetComponentsInChildren<Image>(true);
            renderers = GetComponentsInChildren<Renderer>(true);

            meshMats = new Material[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] is SpriteRenderer)
                    continue;

                meshMats[i] = Application.isPlaying
                    ? renderers[i].materials
                    : renderers[i].sharedMaterials;
            }
        }

        private void ApplyColorInstant(Color color)
        {
            if (spriteRenderers != null)
            {
                foreach (var _sprite in spriteRenderers)
                {
                    if (_sprite)
                        _sprite.color = color;
                }
            }

            if (images != null)
            {
                foreach (var _image in images)
                {
                    if (_image)
                        _image.color = color;
                }
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!renderers[i])
                        continue;

                    if (renderers[i] is SpriteRenderer)
                        continue;

                    var _mats = meshMats[i];

                    if (_mats == null)
                        continue;

                    foreach (var _mat in _mats)
                    {
                        if (_mat)
                            SetMaterialColor(_mat, color);
                    }
                }
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
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                foreach (var _sprite in spriteRenderers)
                {
                    if (_sprite)
                        return _sprite.color;
                }
            }

            if (images != null && images.Length > 0)
            {
                foreach (var _image in images)
                {
                    if (_image)
                        return _image.color;
                }
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!renderers[i])
                        continue;

                    if (renderers[i] is SpriteRenderer)
                        continue;

                    var _mats = meshMats[i];

                    if (_mats == null || _mats.Length == 0)
                        continue;

                    var _mat = _mats[0];

                    if (_mat.HasProperty("_BaseColor"))
                        return _mat.GetColor("_BaseColor");

                    if (_mat.HasProperty("_Color"))
                        return _mat.GetColor("_Color");
                }
            }

            return Color.white;
        }

        private bool NeedTransparency()
        {
            return from.a < 1f || to.a < 1f;
        }

        private void PrepareMeshTransparencyIfNeeded()
        {
            if (meshTransparentPrepared)
                return;

            if (!NeedTransparency())
                return;

            if (renderers == null)
                return;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (!renderers[i])
                    continue;

                if (renderers[i] is SpriteRenderer)
                    continue;

                var _mats = meshMats[i];

                if (_mats == null)
                    continue;

                foreach (var _mat in _mats)
                {
                    if (!_mat)
                        continue;

                    if (_mat.HasProperty("_Surface"))
                    {
                        _mat.SetFloat("_Surface", 1);
                        _mat.SetFloat("_Blend", 0);
                        _mat.SetFloat("_ZWrite", 0);
                        _mat.renderQueue = 3000;
                    }
                    else
                    {
                        _mat.SetInt(
                            "_SrcBlend",
                            (int)UnityEngine.Rendering.BlendMode.SrcAlpha
                        );

                        _mat.SetInt(
                            "_DstBlend",
                            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
                        );

                        _mat.SetInt("_ZWrite", 0);

                        _mat.DisableKeyword("_ALPHATEST_ON");
                        _mat.EnableKeyword("_ALPHABLEND_ON");
                        _mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                        _mat.renderQueue = 3000;
                    }
                }
            }

            meshTransparentPrepared = true;
        }

        private void ApplyColor(Color color)
        {
            if (spriteRenderers != null)
            {
                foreach (var _sprite in spriteRenderers)
                {
                    if (_sprite)
                        _sprite.color = color;
                }
            }

            if (images != null)
            {
                foreach (var _image in images)
                {
                    if (_image)
                        _image.color = color;
                }
            }

            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!renderers[i])
                        continue;

                    if (renderers[i] is SpriteRenderer)
                        continue;

                    var _mats = meshMats[i];

                    if (_mats == null)
                        continue;

                    foreach (var _mat in _mats)
                    {
                        if (_mat)
                            SetMaterialColor(_mat, color);
                    }
                }
            }
        }

        public override void Play(Action onComplete = null)
        {
            Stop();

            CacheTargets();

            ApplyColorInstant(from);

            PrepareMeshTransparencyIfNeeded();

            descr = LeanTween.value(gameObject, from, to, time)
                .setOnUpdate(ApplyColor);

            base.Play(onComplete);
        }

        public override void ToDefault(
            bool instant = false,
            Action onComplete = null
        )
        {
            Stop();

            CacheTargets();

            if (instant)
            {
                ApplyColorInstant(from);
                base.ToDefault(true, onComplete);
                return;
            }

            var _current = GetCurrentColor();

            descr = LeanTween.value(gameObject, _current, from, time)
                .setOnUpdate(ApplyColor);

            base.ToDefault(false, onComplete);
        }
    }
}
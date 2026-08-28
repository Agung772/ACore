using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ACore
{
    public class SpriteAnimationThis : MonoBehaviour
    {
        [SerializeField] protected bool autoPlay = true;
        [SerializeField] private bool isLoop;
        [SerializeField] private float fps = 12;
        [SerializeField] private Sprite[] sprites;
        [SerializeField, HideIf(nameof(isLoop))] private UnityEvent onComplete;
        
        private SpriteRenderer spriteRenderer;
        private Image image;
        private Coroutine playCoroutine;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            if (autoPlay)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play(Action onComplete = null)
        {
            Stop();
            if (isLoop)
            {
                playCoroutine = StartCoroutine(PlayCoroutine(null));
            }
            else
            {
                playCoroutine = StartCoroutine(PlayCoroutine(onComplete));
            }
        }

        public void Stop()
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }
        }

        private IEnumerator PlayCoroutine(Action onComplete)
        {
            if (sprites == null || sprites.Length == 0) yield break;

            foreach (var _sprite in sprites)
            {
                if (spriteRenderer) spriteRenderer.sprite = _sprite;
                else if (image) image.sprite = _sprite;
                yield return new WaitForSeconds(1f / Mathf.Max(fps, 0.01f));
            }

            if (!isLoop)
            {
                this.onComplete?.Invoke();
                onComplete?.Invoke();
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}

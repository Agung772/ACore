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
            gameObject.StopCoroutine();
        }

        public void Play()
        {
            if (isLoop)
            {
                gameObject.StartCoroutineLoop(PlayCoroutine);
            }
            else
            {
                gameObject.StartCoroutine(PlayCoroutine);
            }
        }

        public void Stop()
        {
            gameObject.StopCoroutine();
        }

        private IEnumerator PlayCoroutine()
        {
            foreach (var _sprite in sprites)
            {
                if (spriteRenderer) spriteRenderer.sprite = _sprite;
                else if (image) image.sprite = _sprite;
                yield return new WaitForSeconds(1 / fps);
            }

            if (!isLoop)
            {
                onComplete?.Invoke();
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}

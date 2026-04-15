using System;
using UnityEngine;

namespace ACore
{
    public static class AudioExtensions
    {
        public static void PlayLoop(this AudioClip clip)
        {
            if (clip == null) return;
            var _source = GAME.Get<AudioManager>().BGMSource;
            if (_source.clip == clip) return;
            
            StopLoop(onComplete: () =>
            {
                _source.clip = clip;
                _source.Play();
                
                var _currentVolume = GAME.Get<AudioManager>().BGMVolume;
                _source.gameObject.LeanValue(value => _source.volume = value, 0, _currentVolume, 0.5f);
            });
        }

        public static void StopLoop(Action onComplete = null)
        {
            var _source = GAME.Get<AudioManager>().BGMSource;
            if (_source.clip == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            _source.gameObject.LeanValue(value => _source.volume = value, _source.volume, 0, 0.5f)
                .setOnComplete(() =>
                {
                    _source.clip = null;
                    onComplete?.Invoke();
                });
        }
        
        public static void PlayOneShot(this AudioClip clip)
        {
            if (clip == null) return;
            var _source = GAME.Get<AudioManager>().SFXSource;
            _source.PlayOneShot(clip);
        }
    }
}

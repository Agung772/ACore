using System;
using UnityEngine;

namespace ACore
{
    public static class AUDIO
    {
        private static AudioManager manager;

        internal static void Initialize()
        {
            manager = GAME.Get<AudioManager>();
        }
        
        public static float BGMVolume
        {
            get => PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            set
            {
                manager.BGMSource.volume = value;
                PlayerPrefs.SetFloat("BGMVolume", value);
            }
        }
        
        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat("SFXVolume", 1);
            set
            {
                manager.SFXSource.volume = value;
                PlayerPrefs.SetFloat("SFXVolume", value);
            }
        }
        
        public static void PlayLoop()
        {
            var _source = manager.BGMSource;
            StopLoop(onComplete: () =>
            {
                _source.Play();
                
                _source.gameObject.LeanCancel();
                _source.gameObject.LeanValue(value => _source.volume = value, 0, BGMVolume, 0.5f);
            });
        }
        
        public static void PlayLoop(this AudioClip clip)
        {
            if (clip == null) return;
            var _source = manager.BGMSource;
            if (_source.clip == clip) return;
            
            StopLoop(onComplete: () =>
            {
                _source.clip = clip;
                _source.Play();
                
                _source.gameObject.LeanCancel();
                _source.gameObject.LeanValue(value => _source.volume = value, 0, BGMVolume, 0.5f);
            });
        }

        public static void StopLoop(Action onComplete = null)
        {
            var _source = manager.BGMSource;
            if (_source.clip == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            _source.gameObject.LeanCancel();
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
            manager.SFXSource.PlayOneShot(clip);
        }
    }
}

using UnityEngine;

namespace ACore
{
    public class AudioManager : GlobalBehaviour
    {
        public AudioSource BGMSource { get; private set; }
        public AudioSource SFXSource { get; private set; }

        public override void Initialize()
        {
            BGMSource = CreateSource("BGMSource");
            BGMSource.loop = true;
            BGMSource.volume = BGMVolume;
            
            SFXSource = CreateSource("SFXSource");
            SFXSource.volume = SFXVolume;
        }

        private AudioSource CreateSource(string name)
        {
            var _source = new GameObject(name).AddComponent<AudioSource>();
            _source.transform.SetParent(GAME.Manager.transform);
            return _source;
        }
        
        public float BGMVolume
        {
            get => PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            set
            {
                BGMSource.volume = value;
                PlayerPrefs.SetFloat("BGMVolume", value);
            }
        }
        
        public float SFXVolume
        {
            get => PlayerPrefs.GetFloat("SFXVolume", 1);
            set
            {
                SFXSource.volume = value;
                PlayerPrefs.SetFloat("SFXVolume", value);
            }
        }
    }
}

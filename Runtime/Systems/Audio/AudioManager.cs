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
            BGMSource.volume = AUDIO.BGMVolume;
            
            SFXSource = CreateSource("SFXSource");
            SFXSource.volume = AUDIO.SFXVolume;
            
            AUDIO.Initialize();
        }

        private AudioSource CreateSource(string name)
        {
            var _source = new GameObject(name).AddComponent<AudioSource>();
            _source.transform.SetParent(GAME.Manager.transform);
            return _source;
        }
    }
}

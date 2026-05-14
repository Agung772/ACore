using UnityEngine;

namespace ACore
{
    public static class ParticleExtensions
    {
        public static void PlayAndRelease(this ParticleSystem particle)
        {
            if (!particle) return;
            particle.transform.Release();
            particle.Play();
        }
        
        public static void StopAndRelease(this ParticleSystem particle)
        {
            if (!particle) return;
            particle.transform.Release();
            particle.Stop();
        }
        
        public static void Loop(this ParticleSystem particle, bool active, bool releaseParent = false)
        {
            if (!particle) return;
            if (releaseParent) particle.transform.Release();
            var _main = particle.main;
            _main.loop = active;
        }
    }
}
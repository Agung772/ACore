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
    }
}
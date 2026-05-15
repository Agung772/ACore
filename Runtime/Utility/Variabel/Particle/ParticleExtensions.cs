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
        
        public static void Loop(this ParticleSystem particle, bool active, bool withChildren = true)
        {
            if (!particle) return;

            if (withChildren)
            {
                var _particles = particle.GetComponentsInChildren<ParticleSystem>(true);

                foreach (var _ps in _particles)
                {
                    var _main = _ps.main;
                    _main.loop = active;
                }
            }
            else
            {
                var _main = particle.main;
                _main.loop = active;
            }
        }
    }
}
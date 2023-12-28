using System.Collections.Generic;
using UnityEngine;

namespace _Game.DesignPattern
{
    public enum VFX
    {
        DUST = 0,
    }
    public class PoolController : Singleton<PoolController>
    {
        [Header("Pool")] public PoolAmount[] pool;

        [Header("Particle")] public ParticleAmount[] particle;
        public Dictionary<VFX, ParticleSystem> Particles;

        public void Awake()
        {
            for (int i = 0; i < particle.Length; i++)
                ParticlePool.Preload(particle[i].prefab, particle[i].amount, particle[i].root);

            for (int i = 0; i < pool.Length; i++)
                SimplePool.Preload(pool[i].prefab, pool[i].amount, pool[i].root, pool[i].collect, pool[i].clamp);

        }
    }
}

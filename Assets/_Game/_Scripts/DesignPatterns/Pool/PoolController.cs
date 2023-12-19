using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.DesignPattern
{
    public class PoolController : MonoBehaviour
    {
        [Header("Pool")] public PoolAmount[] pool;

        [Header("Particle")] public ParticleAmount[] particle;


        public void Awake()
        {
            for (int i = 0; i < particle.Length; i++)
                ParticlePool.Preload(particle[i].prefab, particle[i].amount, particle[i].root);

            for (int i = 0; i < pool.Length; i++)
                SimplePool.Preload(pool[i].prefab, pool[i].amount, pool[i].root, pool[i].collect, pool[i].clamp);

        }
    }
}

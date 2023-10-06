using _Game._Scripts.Data;
using DesignPattern;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        public AudioData AudioData => audioData;

        [SerializeField] private WorldData worldData;

        public WorldData WorldData => worldData;
    }
}

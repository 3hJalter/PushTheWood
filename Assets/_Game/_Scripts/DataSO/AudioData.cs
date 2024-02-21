using System.Collections.Generic;
using AudioEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData", order = 1)]
    public class AudioData : SerializedScriptableObject
    {
        [Title("BGM")]
        [SerializeField]
        private readonly Dictionary<BgmType, AudioClip> _bgmDict;
        
        [Title("SFX")]
        [SerializeField]
        private readonly Dictionary<SfxType, AudioClip> _sfxDict;
        
        [Title("Environment")]
        [SerializeField] Dictionary<EnvironmentType, AudioClip> _environmentDict;

        public Dictionary<BgmType, AudioClip> BGMDict => _bgmDict;

        public Dictionary<SfxType, AudioClip> SfxDict => _sfxDict;
        
        public Dictionary<EnvironmentType, AudioClip> EnvironmentDict => _environmentDict;
    }
}

namespace AudioEnum
{
    public enum SfxType
    {
        None = -1,
        SplashWater = 0,
        PushChump = 1,
        PushChumpStand = 2,
        PushStone = 3,
        PushTree = 4,
        EnemyDetectPlayer = 5,
        EnemyShotArrow = 6,
        Undo = 7,
        // UI
        Click = 100,
        Win = 101,
        Lose = 102,
    }

    public enum BgmType
    {
        None = -1,
        MainMenu = 0,
        InGame = 1,
    }
    
    public enum EnvironmentType
    {
        None = -1,
        Ocean = 0,
    }
}

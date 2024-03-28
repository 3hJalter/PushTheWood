using System;
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
        [SerializeField] private readonly Dictionary<BgmType, Audio> _bgmAudioDict;
        
        [Title("SFX")]
        [SerializeField] private readonly Dictionary<SfxType, Audio> _sfxAudioDict;
        
        [Title("Environment")]
        [SerializeField] private readonly Dictionary<EnvironmentType, Audio> _environmentAudioDict;
        
        public Dictionary<BgmType, Audio> BgmAudioDict => _bgmAudioDict;
        
        public Dictionary<SfxType, Audio> SfxAudioDict => _sfxAudioDict;
        
        public Dictionary<EnvironmentType, Audio> EnvironmentAudioDict => _environmentAudioDict;
    }

    [Serializable]
    public class Audio
    {
        public AudioClip clip;
        [Range(0,1)]
        public float multiplier = 1;
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
        Walk = 8,
        OpenChest = 9,
        Sleep = 10,
        Happy = 11,
        Whistling1 = 12,
        Whistling2 = 13,
        Whistling3 = 14,
        PushEnemy = 15,
        BlockTree = 16,
        Hint = 17,
        EnemyDie = 18,
        GrowTree   = 19,
        // UI
        ClickOpen = 100,
        Win = 101,
        Lose = 102,
        BuyItem = 103,
        CollectReward = 104,
        CollectItem = 105,
        ClickClose = 106,
        ClickToggle = 107,
        ClockTick = 108,
        PopupOpen = 109,
        WarningLevel = 110,
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

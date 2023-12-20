using System.Collections.Generic;
using AudioEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObjects/AudioData", order = 1)]
    public class AudioData : SerializedScriptableObject
    {
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [Title("BGM")] [SerializeField] private readonly Dictionary<BgmType, AudioClip> _bgmDic;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [Title("SFX")] [SerializeField] private readonly Dictionary<SfxType, AudioClip> _sfxDic;

        public Dictionary<BgmType, AudioClip> BGMDic => _bgmDic;

        public Dictionary<SfxType, AudioClip> SfxDic => _sfxDic;
    }
}

namespace AudioEnum
{
    public enum SfxType
    {
        None = -1
    }

    public enum BgmType
    {
        None = -1,
        MainMenu = 0
    }
}

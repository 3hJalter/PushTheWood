using System;
using System.Collections.Generic;
using _Game.Data;
using _Game.DesignPattern;
using AudioEnum;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Managers
{
    [Serializable]
    public struct AudioSourceData
    {
        [SerializeField]
        private AudioSource audioSource;
        [HideInInspector]
        public ObjectContainer audioSourcePool;
        [ReadOnly]
        public Audio currentAudio;
        [ReadOnly]
        [SerializeField] private float baseVolume;

        private List<AudioSource> audioSources;

        public void SetLoop(bool loop)
        {
            audioSource.loop = loop;
        }
        
        public void SetAudio(Audio audio)
        {
            if (audio is null) return;
            if (audioSourcePool && audioSource.isPlaying)
            {
                audioSourcePool.Push(0, audioSource.gameObject.transform);
                audioSource = audioSourcePool.Pop(0).AudioSource;
            }

            currentAudio = audio;
            audioSource.clip = audio.clip;
            Volume = audio.multiplier;
        }
        
        public void Play()
        {
            audioSource.Play();
        }
        
        public void Pause()
        {
            audioSource.Pause();
        }
        
        public void Stop()
        {
            audioSource.Stop();
        }
        
        public float BaseVolume
        {
            get => baseVolume;
            set
            {
                baseVolume = value;
                Volume = currentAudio?.multiplier ?? 1;
            }
        }

        public float Volume
        {
            get => audioSource.volume;
            set => audioSource.volume = baseVolume * value;
        }

        public bool IsMute => audioSource.volume < 0.01 || audioSource.mute;
        
    }
    
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioData audioData;
        [SerializeField] ObjectContainer audioSourcePool;

        [SerializeField] private AudioSourceData bgm;
        [SerializeField] private AudioSourceData sfx;
        [SerializeField] private AudioSourceData environment;

        private void Awake()
        {
            audioSourcePool.OnInit();
            bgm.BaseVolume = DataManager.Ins.GameData.setting.bgmVolume;
            sfx.BaseVolume = DataManager.Ins.GameData.setting.sfxVolume;
            sfx.audioSourcePool = audioSourcePool;
            environment.BaseVolume = DataManager.Ins.GameData.setting.envSoundVolume;
            
            audioData = DataManager.Ins.AudioData;
            
        }

        public float BgmVolume => bgm.BaseVolume;
        public float SfxVolume => sfx.BaseVolume;
        
        // <summary>
        // Get a background music from AudioData
        // </summary>
        // <param name="type">Type of BGM</param>
        // <returns>Audio</returns>
        private Audio GetBgmAudio(BgmType type)
        {
            return GetAudio(audioData.BgmAudioDict, type);
        }

        // <summary>
        // Get a sound effect from AudioData
        // </summary>
        // <param name="type">Type of SFX</param>
        // <returns>Audio</returns>
        private Audio GetSfxAudio(SfxType type)
        {
            return GetAudio(audioData.SfxAudioDict, type);
        }
        
        // <summary>
        // Get an environment sound from AudioData
        // </summary>
        // <param name="type">Type of Environment</param>
        // <returns>Audio</returns>
        private Audio GetEnvironmentAudio(EnvironmentType type)
        {
            return GetAudio(audioData.EnvironmentAudioDict, type);
        }

        // <summary>
        // Get an audio from a certain dictionary
        // </summary>
        // <param name="audioDictionary">Dictionary of audio</param>
        // <param name="type">Type of audio</param>
        // <returns>Audio</returns>
        private static Audio GetAudio<T>(IReadOnlyDictionary<T, Audio> audioDictionary, T type)
        {
            return audioDictionary.GetValueOrDefault(type);
        }

        // <summary>
        // Play a background music
        // </summary>
        // <param name="type">Type of BGM</param>
        // <param name="fadeFloat">Fade out time</param>
        public void PlayBgm(BgmType type, float fadeOut = 0.3f)
        {
            Audio audioIn = GetBgmAudio(type);
            if (audioIn is null) return;
            if (audioIn == bgm.currentAudio) return;
            bgm.SetLoop(true);
            if (fadeOut == 0f || bgm.IsMute)
            {
                bgm.SetAudio(audioIn);
                bgm.Play();
            } else
            {
                DOVirtual.Float(bgm.currentAudio.multiplier, 0, fadeOut, value => bgm.Volume = value)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        bgm.SetAudio(audioIn);
                        bgm.Play();
                    });
            }
        }
        
        // <summary>
        // Play an environment sound
        // </summary>
        // <param name="type">Type of Environment</param>
        // <param name="fadeFloat">Fade out time</param>
        // <param name="fadeIn">Fade in time</param>
        public void PlayEnvironment(EnvironmentType type, float fadeFloat = 0.3f, float fadeIn = 0.5f)
        {
            Audio audioIn = GetEnvironmentAudio(type);
            if (audioIn is null) return;
            environment.SetLoop(true);
            if (fadeFloat == 0f || environment.IsMute)
            {
                environment.SetAudio(audioIn);
                environment.Play();
            } else
            {
                DOVirtual.Float(environment.currentAudio.multiplier, 0, fadeFloat, value => environment.Volume = value)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        environment.SetAudio(audioIn);
                        environment.Play();
                        if (fadeIn > 0)
                        {
                            DOVirtual.Float(0, audioIn.multiplier, fadeIn, value => environment.Volume = value);
                        }
                        else
                        {
                            environment.Volume = audioIn.multiplier;
                        }
                    });
            }
        }
        
        // <summary>
        // Play a sound effect
        // </summary>
        // <param name="type">Type of SFX</param>
        public void PlaySfx(SfxType type)
        {
            Audio audioIn = GetSfxAudio(type);
            if (audioIn is null) return;
            sfx.SetAudio(audioIn);
            sfx.Play();
        }
        

        // <summary>
        // Play a random sound effect from a list
        // </summary>
        // <param name="sfxTypes">List of SFX</param>
        public void PlayRandomSfx(List<SfxType> sfxTypes)
        {
            PlaySfx(sfxTypes[Random.Range(0, sfxTypes.Count)]);
        }
        
        public void PauseBgm()
        {
            bgm.Pause();
        }

        public void UnPauseBgm()
        {
            bgm.Play();
        }
        
        public void StopBgm()
        {
            bgm.Stop();
        }
        
        public void PauseEnvironment()
        {
            environment.Pause();
        }
        
        public void UnPauseEnvironment()
        {
            environment.Play();
        }
        
        public void StopEnvironment()
        {
            environment.Stop();
        }

        public void StopSfx()
        {
            sfx.Stop();
        }

        public bool IsBgmMute()
        {
            return bgm.IsMute;
        }

        public bool IsEnvironmentMute()
        {
            return environment.IsMute;
        }
        
        public bool IsSfxMute()
        {
            return sfx.IsMute;
        }

        public void ToggleBgmVolume(float value)
        {
            bgm.BaseVolume = value;
            DataManager.Ins.GameData.setting.bgmVolume = value;
        }
        
        public void ToggleEnvironmentVolume(float value)
        {
            environment.BaseVolume = value;
            DataManager.Ins.GameData.setting.envSoundVolume = value;
        }
        
        public void ToggleSfxVolume(float value)
        {
            sfx.BaseVolume = value;
            DataManager.Ins.GameData.setting.sfxVolume = value;
        }
    }
}

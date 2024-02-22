using System;
using System.Collections.Generic;
using _Game.Data;
using _Game.DesignPattern;
using AudioEnum;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioData audioData;
        [SerializeField] private AudioSource bgm;
        [SerializeField] private AudioSource sfx;
        [SerializeField] private AudioSource environment;
        
        private BgmType _currentBGM = BgmType.None;
        private float _currentBGMVolume = 1f;
        private float _currentEnvironmentVolume = 1f;

        private void Awake()
        {
            audioData = DataManager.Ins.AudioData;
        }

        // <summary>
        // Get a background music from AudioData
        // </summary>
        // <param name="type">Type of BGM</param>
        // <returns>AudioClip</returns>
        public AudioClip GetBgm(BgmType type)
        {
            return GetAudio(audioData.BGMDict, type);
        }

        // <summary>
        // Get a sound effect from AudioData
        // </summary>
        // <param name="type">Type of SFX</param>
        // <returns>AudioClip</returns>
        public AudioClip GetSfx(SfxType type)
        {
            return GetAudio(audioData.SfxDict, type);
        }
        
        // <summary>
        // Get an environment sound from AudioData
        // </summary>
        // <param name="type">Type of Environment</param>
        // <returns>AudioClip</returns>
        public AudioClip GetEnvironment(EnvironmentType type)
        {
            return GetAudio(audioData.EnvironmentDict, type);
        }

        // <summary>
        // Get an audio from a certain dictionary
        // </summary>
        // <param name="audioDictionary">Dictionary of audio</param>
        // <param name="type">Type of audio</param>
        // <returns>AudioClip</returns>
        private static AudioClip GetAudio<T>(IReadOnlyDictionary<T, AudioClip> audioDictionary, T type)
        {
            return audioDictionary.GetValueOrDefault(type);
        }

        // <summary>
        // Play a background music
        // </summary>
        // <param name="type">Type of BGM</param>
        // <param name="fadeFloat">Fade out time</param>
        // <param name="targetVolume">Volume of BGM</param>
        public void PlayBgm(BgmType type, float fadeFloat = 0.3f, float targetVolume = 1f)
        {
            if (type == _currentBGM) return;
            bgm.loop = true;
            if (fadeFloat == 0f || bgm.mute)
            {
                SetBGM();
                PlayAudio(bgm, audioData.BGMDict, type);
            }
            // FadeOut, Then Play
            else
            {
                DOVirtual.Float(_currentBGMVolume, 0, fadeFloat, value => VolumeDown(bgm, value))
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        SetBGM();
                        PlayAudio(bgm, audioData.BGMDict, type);
                    });
            }
            
            return;

            void SetBGM()
            {
                _currentBGM = type;
                _currentBGMVolume = targetVolume;
                bgm.volume = targetVolume;
            }
        }
        
        // <summary>
        // Play an environment sound
        // </summary>
        // <param name="type">Type of Environment</param>
        // <param name="fadeFloat">Fade out time</param>
        // <param name="fadeIn">Fade in time</param>
        // <param name="targetVolume">Volume of Environment</param>
        public void PlayEnvironment(EnvironmentType type, float fadeFloat = 0.3f, float fadeIn = 0.5f, float targetVolume = 1f)
        {
            environment.loop = true;
            if (fadeFloat == 0f || environment.mute)
            {
                _currentEnvironmentVolume = targetVolume;
                environment.volume = targetVolume;
                PlayAudio(environment, audioData.EnvironmentDict, type);
            }
            // FadeOut, Then Play
            else
            {
                DOVirtual.Float(_currentEnvironmentVolume, 0, fadeFloat, value => VolumeDown(environment, value))
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        PlayAudio(environment, audioData.EnvironmentDict, type);
                        if (fadeIn > 0)
                        {
                            DOVirtual.Float(0, targetVolume, fadeIn, value =>
                            {
                                environment.volume = value;
                                _currentEnvironmentVolume = value;
                            });
                        }
                        else
                        {
                            environment.volume = targetVolume;
                            _currentEnvironmentVolume = targetVolume;
                        }
                    });
            }
        }

        // <summary>
        // Play a sound effect
        // </summary>
        // <param name="type">Type of SFX</param>
        // <param name="targetVolume">Volume of SFX</param>
        public void PlaySfx(SfxType type, float targetVolume = 1f)
        {
            sfx.volume = targetVolume;
            PlayAudio(sfx, audioData.SfxDict, type);
        }

        // <summary>
        // Play a sound effect
        // </summary>
        // <param name="audioClip">SFX audio</param>
        // <param name="targetVolume">Volume of SFX</param>
        public void PlaySfx(AudioClip audioClip, float targetVolume = 1f)
        {
            sfx.volume = targetVolume;
            sfx.clip = audioClip;
            sfx.Play();
        }

        // <summary>
        // Turn down the volume of an audio (BGM, SFX, Environment)
        // </summary>
        // <param name="audioSource">AudioSource</param>
        // <param name="value">Volume</param>
        private static void VolumeDown(AudioSource audioSource, float value)
        {
            audioSource.volume = value;
        }

        // <summary>
        // Play an audio
        // </summary>
        // <param name="audioSource">AudioSource (BGM, SFX, Environment)</param>
        // <param name="audioDictionary">Dictionary of audio</param>
        // <param name="type">Type of audio</param>
        private static void PlayAudio<T>(AudioSource audioSource, IReadOnlyDictionary<T, AudioClip> audioDictionary,
            T type)
        {
            AudioClip audioClip = GetAudio(audioDictionary, type);
            if (audioClip == null) return;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // <summary>
        // Play a random sound effect from a list
        // </summary>
        // <param name="sfxTypes">List of SFX</param>
        public void PlayRandomSfx(List<SfxType> sfxTypes)
        {
            List<AudioClip> audioClips = new();
            for (int i = 0; i < sfxTypes.Count; i++)
            {
                AudioClip audioClip = audioData.SfxDict.GetValueOrDefault(sfxTypes[i]);
                if (audioClip == null) continue;
                audioClips.Add(audioClip);
            }

            if (audioClips.Count == 0) return;
            bgm.clip = audioClips[Random.Range(0, audioClips.Count)];
            bgm.Play();
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

        public void MuteBgm()
        {
            bgm.mute = true;
        }

        public void UnMuteBgm()
        {
            bgm.mute = false;
        }

        public void MuteEnvironment()
        {
            environment.mute = true;
        }
        
        public void UnMuteEnvironment()
        {
            environment.mute = false;
        }
        
        public void MuteSfx()
        {
            sfx.mute = true;
        }

        public void UnMuteSfc()
        {
            sfx.mute = false;
        }
    }
}

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
        
        private float _currentBGMVolume = 1f;

        private void Awake()
        {
            audioData = DataManager.Ins.AudioData;
        }

        public AudioClip GetBgm(BgmType type)
        {
            return GetAudio(audioData.BGMDict, type);
        }

        public AudioClip GetSfx(SfxType type)
        {
            return GetAudio(audioData.SfxDict, type);
        }

        private static AudioClip GetAudio<T>(IReadOnlyDictionary<T, AudioClip> audioDictionary, T type)
        {
            return audioDictionary.GetValueOrDefault(type);
        }

        public void PlayBgm(BgmType type, float fadeFloat = 0.3f, float targetVolume = 1f)
        {
            bgm.loop = true;
            if (fadeFloat == 0f || bgm.mute) PlayAudio(bgm, audioData.BGMDict, type);
            // FadeOut, Then Play
            else
                DOVirtual.Float(_currentBGMVolume, 0, fadeFloat, value => VolumeDown(bgm, value))
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        PlayAudio(bgm, audioData.BGMDict, type);
                        
                    });
            _currentBGMVolume = targetVolume;
            bgm.volume = targetVolume;
        }

        public void PlaySfx(SfxType type, float targetVolume = 1f)
        {
            sfx.volume = targetVolume;
            PlayAudio(sfx, audioData.SfxDict, type);
        }

        public void PlaySfx(AudioClip audioClip, float targetVolume = 1f)
        {
            sfx.volume = targetVolume;
            sfx.clip = audioClip;
            sfx.Play();
        }

        private static void VolumeDown(AudioSource audioSource, float value)
        {
            audioSource.volume = value;
        }

        private static void PlayAudio<T>(AudioSource audioSource, IReadOnlyDictionary<T, AudioClip> audioDictionary,
            T type)
        {
            AudioClip audioClip = GetAudio(audioDictionary, type);
            if (audioClip == null) return;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void PlayRandom(List<SfxType> sfxTypes)
        {
            List<AudioClip> audioClips = new();
            for (int i = 0; i < sfxTypes.Count; i++)
            {
                AudioClip audioClip = audioData.SfxDict.TryGetValue(sfxTypes[i], out AudioClip sfx1) ? sfx1 : null;
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

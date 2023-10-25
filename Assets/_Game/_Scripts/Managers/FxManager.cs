using System;
using _Game.DesignPattern;
using DG.Tweening;
using UB.Simple2dWeatherEffects.Standard;
using UnityEngine;

namespace _Game.Managers
{
    public class FxManager : Singleton<FxManager>
    {
        private const float DEFAULT_PLAY_FOG_DENSITY = 1f;
        private const float DEFAULT_STOP_FOG_DENSITY = 0f;
        private const float DEFAULT_PLAY_FOG_DURATION = 1f;
        private const float DEFAULT_STOP_FOG_DURATION = 1f;

        [Header("Fog")] [SerializeField] private D2FogsPE fogControl;
        [Header("Water")] [SerializeField] private GameObject water;

        private void Awake()
        {
            water.SetActive(true);
        }

        public void PlayTweenFog(bool isStopAfterDone = false, float fogDensity = DEFAULT_PLAY_FOG_DENSITY,
            float duration = DEFAULT_PLAY_FOG_DURATION, Action onCompleteAction = null)
        {
            if (!fogControl) return;
            DOTween.To(GetDensity, SetDensity, fogDensity, duration).SetEase(Ease.OutSine).Play()
                .OnComplete(
                    () =>
                    {
                        if (isStopAfterDone) StopTweenFog();
                        onCompleteAction?.Invoke();
                    });
        }

        public void StopTweenFog(float duration = DEFAULT_STOP_FOG_DURATION)
        {
            if (!fogControl) return;
            DOTween.To(GetDensity, SetDensity, DEFAULT_STOP_FOG_DENSITY, duration).SetEase(Ease.OutSine).Play();
        }

        private float GetDensity()
        {
            return fogControl.Density;
        }

        private void SetDensity(float value)
        {
            fogControl.Density = value;
        }
    }
}

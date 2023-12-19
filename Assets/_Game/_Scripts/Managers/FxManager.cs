using System;
using _Game.DesignPattern;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VinhLB;

namespace _Game.Managers
{
    public class FxManager : Singleton<FxManager>
    {
        private const float DEFAULT_PLAY_FOG_DENSITY = 1f;
        private const float DEFAULT_STOP_FOG_DENSITY = 0f;
        private const float DEFAULT_PLAY_FOG_DURATION = 1f;
        private const float DEFAULT_STOP_FOG_DURATION = 1f;
        
        // [Header("Fog")] [SerializeField] private D2FogsSprite fogControl;
        [Header("Water")] [SerializeField] private GameObject water;
        [Header("Grid")] [SerializeField] private GameObject grid;
        [Header("Grass")] [SerializeField] private UniversalRendererData rendererData;

        private GrassTrampleFeature feature;
        
        private void Awake()
        {
            water.SetActive(true);
            grid.SetActive(true);
            
            VinhLB.Utilities.TryGetRendererFeature<GrassTrampleFeature>(rendererData, out feature);
        }

        public void PlayTweenFog(bool isStopAfterDone = false, float fogDensity = DEFAULT_PLAY_FOG_DENSITY,
            float duration = DEFAULT_PLAY_FOG_DURATION, Action onCompleteAction = null)
        {
            // if (!fogControl) return;
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
            // if (!fogControl) return;
            DOTween.To(GetDensity, SetDensity, DEFAULT_STOP_FOG_DENSITY, duration).SetEase(Ease.OutSine).Play();
        }

        public void ResetTrackedTrampleObjectList()
        {
            feature.ResetTrackedTrampleList();
        }

        private float GetDensity()
        {
            // return fogControl.Density;
            return 0;
        }
        

        private void SetDensity(float value)
        {
            // fogControl.Density = value;
        }
        
    }
}

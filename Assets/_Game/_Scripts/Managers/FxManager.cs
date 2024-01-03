using _Game.DesignPattern;
using AmazingAssets.CurvedWorld;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VinhLB;

namespace _Game.Managers
{
    public class FxManager : Singleton<FxManager>
    {
        // [SerializeField]
        // private CurvedWorldController curvedWorldController;
        [SerializeField]
        private GameObject _groundGO;
        [SerializeField]
        private GameObject _waterGO;
        [SerializeField]
        private GameObject _cloudsGO;
        [SerializeField]
        private GameObject _gridGO;
        [SerializeField]
        private UniversalRendererData _rendererData;
        [SerializeField]
        private UniversalRenderPipelineAsset renderPipelineAsset;
        
        private GrassTrampleFeature _feature;

        private void Awake()
        {
            _groundGO.SetActive(true);
            _waterGO.SetActive(true);
            // _cloudsGO.SetActive(true);
            _gridGO.SetActive(false);

            VinhLB.Utilities.TryGetRendererFeature(_rendererData, out _feature);
        }

        public void ChangeShadowDistance(float distance)
        {
            renderPipelineAsset.shadowDistance = distance;
            // if distance >= 150, normal bias is 0 
            // else increase it until normal = 1 when distance <= 50
            float normalBias = distance >= 150 ? 0f : 1f - (distance - 50f) / 100f;
            renderPipelineAsset.shadowNormalBias = normalBias;
        }

        private Tween _changePlantCurveTween;
        
        // public void ChangePlanetCurvatureSize(float curve = 7f, float time = 1f)
        // {
        //     _changePlantCurveTween?.Kill();
        //     float currentCurve = curvedWorldController.bendCurvatureSize;
        //     _changePlantCurveTween = DOVirtual.Float(currentCurve, curve, time, value => curvedWorldController.bendCurvatureSize = value);
        // }
        
        public void ResetTrackedTrampleObjectList()
        {
            _feature.ResetTrackedTrampleList();
        }

        public void SwitchGridActive(bool manual = false, bool active = true)
        {
            if (!manual)
            {
                _gridGO.SetActive(!_gridGO.activeSelf);
            }
            else
            {
                _gridGO.SetActive(active);
            }
        }
    }
}
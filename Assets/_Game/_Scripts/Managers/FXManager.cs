using _Game._Scripts.InGame;
using _Game.DesignPattern;
using AmazingAssets.CurvedWorld;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VinhLB;

namespace _Game.Managers
{
    public class FXManager : Singleton<FXManager>
    {
        [Title("General")]
        [SerializeField]
        private UniversalRendererData _rendererData;
        [SerializeField]
        private UniversalRenderPipelineAsset _renderPipelineAsset;
        [SerializeField]
        private UniversalAdditionalCameraData _cameraData;
        
        [Title("Environment Effects")]
        [SerializeField]
        private HintLineTrail trailHint;
        [SerializeField]
        private GameObject _gridGO;
        [SerializeField]
        private GameObject _groundGO;
        [SerializeField]
        private GameObject _waterGO;
        [SerializeField]
        private GameObject _rollingFogGO;
        [SerializeField]
        private GameObject _windDustGO;
        [SerializeField]
        private GameObject _rainGO;
        [SerializeField]
        private bool _activeGrid = false;
        [SerializeField]
        private bool _activeGround = true;
        [SerializeField]
        private bool _activeWater = true;
        [SerializeField]
        private bool _activeRollingFog = true;
        [SerializeField]
        private bool _activeWindDust = true;
        [SerializeField]
        private bool _activeRain = false;

        private GrassTrampleFeature _feature;

        public HintLineTrail TrailHint => trailHint;
        public bool IsGridOn => _gridGO.activeInHierarchy;

        private void Awake()
        {
            _gridGO.SetActive(_activeGrid);
            _groundGO.SetActive(_activeGround);
            _waterGO.SetActive(_activeWater);
            _rollingFogGO.SetActive(_activeRollingFog);
            _windDustGO.SetActive(_activeWindDust);
            _rainGO.SetActive(_activeRain);

            VinhLB.Utilities.TryGetRendererFeature(_rendererData, out _feature);
        }

        public void ChangeShadowDistance(float distance)
        {
            _renderPipelineAsset.shadowDistance = distance;
            // if distance >= 150, normal bias is 0 
            // else increase it until normal = 1 when distance <= 50
            float normalBias = distance >= 150 ? 0f : 1f - (distance - 50f) / 100f;
            _renderPipelineAsset.shadowNormalBias = normalBias;
        }

        public void TogglePostProcessing()
        {
            _cameraData.renderPostProcessing = !_cameraData.renderPostProcessing;
        }

        public void ToggleWater()
        {
            _waterGO.SetActive(!_waterGO.activeInHierarchy);
        }

        public void ToggleGrasses()
        {
            GameObject[] grassGOs = VinhLB.Utilities.FindGameObjectsInLayer("Grass", true);
            for (int i = 0; i < grassGOs.Length; i++)
            {
                grassGOs[i].SetActive(!grassGOs[i].activeInHierarchy);
            }
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
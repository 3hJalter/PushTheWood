using _Game.DesignPattern;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VinhLB;

namespace _Game.Managers
{
    public class FxManager : Singleton<FxManager>
    {
        [SerializeField]
        private GameObject _groundGO;
        [SerializeField]
        private GameObject _waterGO;
        [SerializeField]
        private GameObject _cloudGO;
        [SerializeField]
        private GameObject _gridGO;
        [SerializeField]
        private UniversalRendererData _rendererData;

        private GrassTrampleFeature _feature;

        private void Awake()
        {
            _groundGO.SetActive(true);
            _waterGO.SetActive(true);
            // _cloudGO.SetActive(true);
            _gridGO.SetActive(false);

            VinhLB.Utilities.TryGetRendererFeature(_rendererData, out _feature);
        }

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
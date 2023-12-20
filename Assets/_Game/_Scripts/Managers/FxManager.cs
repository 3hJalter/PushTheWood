using _Game.DesignPattern;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VinhLB;

namespace _Game.Managers
{
    public class FxManager : Singleton<FxManager>
    {
        // [Header("Fog")] [SerializeField] private D2FogsSprite fogControl;
        [Header("Water")] [SerializeField] private GameObject water;
        [Header("Grid")] [SerializeField] private GameObject grid;
        [Header("Grass")] [SerializeField] private UniversalRendererData rendererData;

        private GrassTrampleFeature feature;

        private void Awake()
        {
            water.SetActive(true);
            grid.SetActive(false);

            VinhLB.Utilities.TryGetRendererFeature(rendererData, out feature);
        }

        public void ResetTrackedTrampleObjectList()
        {
            feature.ResetTrackedTrampleList();
        }

        public void SwitchGridActive()
        {
            grid.SetActive(!grid.activeSelf);
        }
    }
}

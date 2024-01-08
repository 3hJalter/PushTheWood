using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using UnityEngine;

namespace _Game.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        [SerializeField] private GridData gridData;

        [SerializeField] private MaterialData materialData;

        public AudioData AudioData => audioData;

        public int CountLevel => gridData.CountLevel;

        public int CountSurfaceMaterial => materialData.CountSurfaceMaterial;
        
        public Material GetTransparentMaterial()
        {
            return materialData.GetTransparentMaterial();
        }
        
        public Material GetSurfaceMaterial(MaterialEnum materialEnum)
        {
            return materialData.GetSurfaceMaterial(materialEnum);
        }
        
        public Material GetGrassMaterial(MaterialEnum materialEnum)
        {
            return materialData.GetGrassMaterial(materialEnum);
        }
        
        // public TutorialContext GetTutorial(int index)
        // {
        //     return tutorialData.GetTutorial(index);
        // }
        
        public TextAsset GetGridTextData(int index)
        {
            return gridData.GetGridTextData(index);
        }

        public GridSurface GetGridSurface(PoolType poolType)
        {
            return gridData.GetGridSurface(poolType);
        }

        public GridUnit GetGridUnit(PoolType poolType)
        {
            return gridData.GetGridUnit(poolType);
        }
    }
}

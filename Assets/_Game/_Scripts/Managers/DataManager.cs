using _Game._Scripts.Data;
using _Game._Scripts.DesignPattern;
using _Game.GameGrid.GridSurface;
using GameGridEnum;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        public AudioData AudioData => audioData;

        [SerializeField] private WorldData worldData;

        public WorldData WorldData => worldData;
        
        [SerializeField] private GridData gameData;
        
        public TextAsset GetGridTextData(int index)
        {
            return gameData.GetGridTextData(index);
        }
        
        public GridSurfaceBase GetGridSurface(GridSurfaceType gridSurfaceType)
        {
            return gameData.GetGridSurface(gridSurfaceType);
        }
    }
}

using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using GameGridEnum;
using UnityEngine;

namespace _Game.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        [SerializeField] private WorldData worldData;

        [SerializeField] private GridData gameData;

        public AudioData AudioData => audioData;

        public WorldData WorldData => worldData;

        public TextAsset GetGridTextData(int index)
        {
            return gameData.GetGridTextData(index);
        }

        public GridSurfaceBase GetGridSurface(GridSurfaceType gridSurfaceType)
        {
            return gameData.GetGridSurface(gridSurfaceType);
        }

        public GridUnitDynamic GetGridUnitDynamic(GridUnitDynamicType gridUnitDynamicType)
        {
            return gameData.GetGridUnitDynamic(gridUnitDynamicType);
        }

        public GridUnitStatic GetGridUnitStatic(GridUnitStaticType gridUnitStaticType)
        {
            return gameData.GetGridUnitStatic(gridUnitStaticType);
        }
    }
}

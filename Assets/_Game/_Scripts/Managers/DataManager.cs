using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using GameGridEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private AudioData audioData;

        [SerializeField] private WorldData worldData; // OLD, Remove later

        [SerializeField] private GridData gridData;

        public AudioData AudioData => audioData;

        public WorldData WorldData => worldData;

        public int CountLevel => gridData.CountLevel;
        
        public TextAsset GetGridTextData(int index)
        {
            return gridData.GetGridTextData(index);
        }

        public GridSurfaceBase GetGridSurface(GridSurfaceType gridSurfaceType)
        {
            return gridData.GetGridSurface(gridSurfaceType);
        }

        public GridUnitDynamic GetGridUnitDynamic(GridUnitDynamicType gridUnitDynamicType)
        {
            return gridData.GetGridUnitDynamic(gridUnitDynamicType);
        }

        public GridUnitStatic GetGridUnitStatic(GridUnitStaticType gridUnitStaticType)
        {
            return gridData.GetGridUnitStatic(gridUnitStaticType);
        }
    }
}

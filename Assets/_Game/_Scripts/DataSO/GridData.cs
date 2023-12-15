using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using GameGridEnum;
using Sirenix.OdinInspector;
using UnityEngine;
using VinhLB;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
    public class GridData : SerializedScriptableObject
    {
        [Title("Generate Grid Text Data")] 
        [SerializeField] private List<TextAsset> gridTextDataList = new();
        [Title("Surface")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        // ReSharper disable once CollectionNeverUpdated.Local
        [SerializeField] private readonly Dictionary<PoolType, GridSurface> _surfaceDic = new();
        [Title("Static Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<PoolType, GridUnitStatic> _staticUnitDic = new();
        [Title("Dynamic Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<PoolType, GridUnitDynamic> _dynamicUnitDic = new();
        [Title("Building Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<PoolType, BuildingUnit> _buildingUnitDic = new();

        public int CountLevel => gridTextDataList.Count;
        
        public TextAsset GetGridTextData(int index)
        {
            return gridTextDataList[index];
        }

        public GridSurface GetGridSurface(PoolType poolType)
        {
            return _surfaceDic.TryGetValue(poolType, value: out GridSurface surface) ? surface : null;
        }
        
        public GridUnit GetGridUnit(PoolType poolType)
        {
            // Get from dictionary static unit first
            if (_staticUnitDic.TryGetValue(poolType, value: out GridUnitStatic staticUnit)) return staticUnit;
            // else get from dictionary dynamic unit
            if (_dynamicUnitDic.TryGetValue(poolType, value: out GridUnitDynamic dynamicUnit)) return dynamicUnit;
            // else get from dictionary building unit
            if (_buildingUnitDic.TryGetValue(poolType, value: out BuildingUnit buildingUnit)) return buildingUnit;
            // else return null
            return null;
        }
    }
}

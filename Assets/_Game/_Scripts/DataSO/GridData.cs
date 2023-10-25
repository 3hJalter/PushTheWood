using System.Collections.Generic;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using GameGridEnum;
using Sirenix.OdinInspector;
using UnityEngine;

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
        [SerializeField] private readonly Dictionary<GridSurfaceType, GridSurfaceBase> _surfaceDic = new();
        [Title("Static Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<GridUnitStaticType, GridUnitStatic> _staticUnitDic = new();
        [Title("Dynamic Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<GridUnitDynamicType, GridUnitDynamic> _dynamicUnitDic = new();

        public int CountLevel => gridTextDataList.Count;
        
        public TextAsset GetGridTextData(int index)
        {
            return gridTextDataList[index];
        }

        public GridSurfaceBase GetGridSurface(GridSurfaceType gridSurfaceType)
        {
            return _surfaceDic.TryGetValue(gridSurfaceType, value: out GridSurfaceBase surface) ? surface : null;
        }
        
        public GridUnitDynamic GetGridUnitDynamic(GridUnitDynamicType gridUnitDynamicType)
        {
            return _dynamicUnitDic.TryGetValue(gridUnitDynamicType, value: out GridUnitDynamic unit) ? unit : null;
        }

        public GridUnitStatic GetGridUnitStatic(GridUnitStaticType gridUnitStaticType)
        {
            return _staticUnitDic.TryGetValue(gridUnitStaticType, value: out GridUnitStatic unit) ? unit : null;
        }
    }
}

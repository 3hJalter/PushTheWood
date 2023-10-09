using System.Collections.Generic;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit.Base;
using GameGridEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game._Scripts.Data
{
    [CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
    public class GridData : SerializedScriptableObject
    {
        [Title("Generate Grid Text Data")] 
        [SerializeField] private List<TextAsset> gridTextDataList = new();
        [Title("Surface")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<GridSurfaceType, GridSurfaceBase> _surfaceDic = new();
        [Title("Static Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<GridUnitStaticType, GridUnitStatic> _staticUnitDic = new();
        [Title("Dynamic Unit")] 
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<GridUnitDynamicType, GridUnitDynamic> _dynamicUnitDic = new();

        public TextAsset GetGridTextData(int index)
        {
            return gridTextDataList[index];
        }

        public GridSurfaceBase GetGridSurface(GridSurfaceType gridSurfaceType)
        {
            return _surfaceDic.TryGetValue(gridSurfaceType, value: out GridSurfaceBase surface) ? surface : null;
        }
    }
}

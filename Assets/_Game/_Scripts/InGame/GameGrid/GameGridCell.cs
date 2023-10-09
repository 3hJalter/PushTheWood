using System.Collections.Generic;
using _Game._Scripts.Utilities.Grid;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit.Base;
using GameGridEnum;

namespace _Game.GameGrid
{
    public class GameGridCell : GridCell<GameGridCellData>
    {
        public GameGridCell()
        {
            data = new GameGridCellData();
        }
        
        public GameGridCellData GetData()
        {
            return data;
        }
        
        
        
        public void SetSurface(GridSurfaceBase surface)
        {
            data.gridSurface = surface;
            data.gridSurfaceType = surface.SurfaceType;
        }
        
        public GridUnitDynamic GetDynamicUnit(GridUnitDynamicType type)
        {
            return !data.gridUnitDynamic.ContainsKey(type) ? null : data.gridUnitDynamic[type];
        }
        
        public GridUnitStatic GetStaticUnit(GridUnitStaticType type)
        {
            return !data.gridUnitStatic.ContainsKey(type) ? null : data.gridUnitStatic[type];
        }
        
        public void AddDynamicUnit(GridUnitStatic unit)
        {
            if (data.gridUnitStatic.ContainsKey(unit.GridUnitStaticType)) return;
                data.gridUnitStatic.Add(unit.GridUnitStaticType, unit);
        }
        
        public void RemoveDynamicUnit(GridUnitStaticType type)
        {
            if (!data.gridUnitStatic.ContainsKey(type)) return;
                data.gridUnitStatic.Remove(type);
        }
        
        public void AddStaticUnit(GridUnitDynamic unit)
        {
            if (data.gridUnitDynamic.ContainsKey(unit.GridUnitDynamicType)) return;
                data.gridUnitDynamic.Add(unit.GridUnitDynamicType, unit);
        }
        
        public void RemoveStaticUnit(GridUnitDynamicType type)
        {
            if (!data.gridUnitDynamic.ContainsKey(type)) return;
                data.gridUnitDynamic.Remove(type);
        }
    }
    
    public class GameGridCellData {
        // Type of cell
        public GridSurfaceType gridSurfaceType = GridSurfaceType.Water;
        public GridSurfaceBase gridSurface;
        // Units of that cell
        public readonly Dictionary<GridUnitStaticType, GridUnitStatic> gridUnitStatic = new();
        public readonly Dictionary<GridUnitDynamicType, GridUnitDynamic> gridUnitDynamic = new();
    }
}

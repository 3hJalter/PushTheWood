using System;
using _Game.GameGrid.GridSurface;
using _Game.Utilities.Grid;
using GameGridEnum;

namespace _Game.GameGrid
{
    public class GameGridCell : GridCell<GameGridCellData>
    {
        public GameGridCell()
        {
            data = new GameGridCellData();
            data.OnInit();
        }

        public GridSurfaceType SurfaceType => data.gridSurfaceType;

        public int IslandID => data.gridSurface == null ? -1 : data.gridSurface.IslandID;

        public void SetSurface(GridSurfaceBase surface, bool canMoving = true)
        {
            data.gridSurface = surface;
            data.gridSurfaceType = surface.SurfaceType;
            data.canMovingDirectly = canMoving;
        }


        public GridUnit.GridUnit GetGridUnitAtHeight(HeightLevel heightLevel)
        {
            return data.gridUnits[(int)heightLevel];
        }

        public void ClearGridUnit()
        {
            for (int i = (int)Constants.dirFirstHeightOfSurface[data.gridSurfaceType]; i < data.gridUnits.Length; i++)
                data.gridUnits[i] = null;
        }

        public void RemoveGridUnit(GridUnit.GridUnit removeUnit)
        {
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] == removeUnit)
                    data.gridUnits[i] = null;
        }

        public void AddGridUnit(GridUnit.GridUnit addUnit)
        {
            for (int i = (int)addUnit.StartHeight; i <= (int)addUnit.EndHeight; i++) data.gridUnits[i] = addUnit;
        }

        public void RemoveGridUnitAtHeight(HeightLevel heightLevel)
        {
            if (data.gridUnits[(int)heightLevel] is null) return;
            data.gridUnits[(int)heightLevel] = null;
        }

        public void AddGridUnit(HeightLevel heightLevel, GridUnit.GridUnit unit)
        {
            data.gridUnits[(int)heightLevel] = unit;
        }

        public void AddGridUnit(HeightLevel startHeight, HeightLevel endHeight, GridUnit.GridUnit unit)
        {
            for (int i = (int)startHeight; i <= (int)endHeight; i++) data.gridUnits[i] = unit;
        }

        public HeightLevel GetMaxHeight()
        {
            HeightLevel maxHeight = HeightLevel.One;
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] is not null && (HeightLevel)i > maxHeight)
                    maxHeight = (HeightLevel)i;
            return maxHeight;
        }
    }

    public class GameGridCellData
    {
        public bool canMovingDirectly;

        public GridSurfaceBase gridSurface;

        // Type of cell
        public GridSurfaceType gridSurfaceType;
        public GridUnit.GridUnit[] gridUnits;

        public void OnInit()
        {
            gridSurfaceType = gridSurface == null ? GridSurfaceType.Water : gridSurface.SurfaceType;
            gridUnits = new GridUnit.GridUnit[Enum.GetValues(typeof(HeightLevel)).Length - 1];
        }
    }
}

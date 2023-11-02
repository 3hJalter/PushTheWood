using System;
using System.Collections.Generic;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.Utilities.Grid;
using GameGridEnum;
using Unity.VisualScripting;
using UnityEngine;

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
            // return data.gridUnitDic.TryGetValue(heightLevel, out GridUnit.GridUnit height) ? height : null;
        }

        public void ClearGridUnit()
        {
            for (int i = (int) Constants.dirFirstHeightOfSurface[data.gridSurfaceType]; i < data.gridUnits.Length; i++) data.gridUnits[i] = null;
            // foreach (HeightLevel height in data.gridUnitDic.Keys.ToList())
            // {
            //     if (data.gridUnitDic[height] != null)
            //     {
            //         data.gridUnitDic[height] = null;
            //     }
            // }
        }

        public void RemoveGridUnit(GridUnit.GridUnit removeUnit)
        {
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] == removeUnit)
                    data.gridUnits[i] = null;

            // loop all height level and set value to null if it is the removeUnit
            // foreach (HeightLevel height in data.gridUnitDic.Keys.ToList())
            // {
            //     if (data.gridUnitDic[height] == removeUnit) data.gridUnitDic[height] = null;
            // }
        }

        public void AddGridUnit(GridUnit.GridUnit addUnit)
        {
            for (int i = (int)addUnit.StartHeight; i <= (int)addUnit.EndHeight; i++) data.gridUnits[i] = addUnit;
            // for (HeightLevel i = addUnit.StartHeight; i <= addUnit.EndHeight; i++) data.gridUnitDic[i] = addUnit;
        }

        public void RemoveGridUnitAtHeight(HeightLevel heightLevel)
        {
            if (data.gridUnits[(int)heightLevel] is null) return;
            data.gridUnits[(int)heightLevel] = null;

            // if (!data.gridUnitDic.TryGetValue(heightLevel, out GridUnit.GridUnit unit)) return;
            // if (unit is null) return;
            // data.gridUnitDic[heightLevel] = null;
            // unit.OnExitCell();
        }

        public void AddGridUnit(HeightLevel heightLevel, GridUnit.GridUnit unit)
        {
            data.gridUnits[(int)heightLevel] = unit;
            // data.gridUnitDic[heightLevel] = unit;
            // unit.OnEnterCell(this);
        }

        public void AddGridUnit(HeightLevel startHeight, HeightLevel endHeight, GridUnit.GridUnit unit)
        {
            for (int i = (int)startHeight; i <= (int)endHeight; i++) data.gridUnits[i] = unit;
            // for (HeightLevel i = startHeight; i <= endHeight; i++) data.gridUnitDic[i] = unit;
        }

        public HeightLevel GetMaxHeight()
        {
            HeightLevel maxHeight = HeightLevel.One;
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] is not null && (HeightLevel)i > maxHeight)
                    maxHeight = (HeightLevel)i;
            return maxHeight;
            // foreach (HeightLevel height in data.gridUnitDic.Keys)
            // {
            //     if (data.gridUnitDic[height] is not null && height > maxHeight) maxHeight = height;
            // }
            //
            // return maxHeight;
        }
    }

    public class GameGridCellData
    {
        // Units of that cell
        // public readonly Dictionary<HeightLevel, GridUnit.GridUnit> gridUnitDic = new();

        public GridSurfaceBase gridSurface;

        // Type of cell
        public GridSurfaceType gridSurfaceType;
        public GridUnit.GridUnit[] gridUnits;
        public bool canMovingDirectly;

        public void OnInit()
        {
            gridSurfaceType = gridSurface == null ? GridSurfaceType.Water : gridSurface.SurfaceType;
            gridUnits = new GridUnit.GridUnit[Enum.GetValues(typeof(HeightLevel)).Length - 1];
            // for (HeightLevel i = HeightLevel.OnePointFive;
            //      i < (HeightLevel)(Enum.GetValues(typeof(HeightLevel)).Length - 1);
            //      i++)
            //     gridUnitDic.Add(i, null);
        }
    }
}

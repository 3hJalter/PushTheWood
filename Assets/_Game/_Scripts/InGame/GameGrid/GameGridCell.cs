﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public void OnInitData()
        {
            data.OnInit();
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

        public GridSurfaceType GetSurfaceType()
        {
            return data.gridSurfaceType;
        }

        public GridUnit.GridUnit GetGridUnitAtHeight(HeightLevel heightLevel)
        {
            return data.gridUnitDic.TryGetValue(heightLevel, out GridUnit.GridUnit height) ? height : null;
        }


        public void RemoveGridUnit(GridUnit.GridUnit removeUnit)
        {
            // loop all height level and set value to null if it is the removeUnit
            foreach (HeightLevel height in data.gridUnitDic.Keys.ToList()
                         .Where(height => data.gridUnitDic[height] == removeUnit))
                data.gridUnitDic[height] = null;
        }

        public void AddGridUnit(GridUnit.GridUnit addUnit)
        {
            for (HeightLevel i = addUnit.StartHeight; i <= addUnit.EndHeight; i++) data.gridUnitDic[i] = addUnit;
        }

        public void RemoveGridUnitAtHeight(HeightLevel heightLevel)
        {
            if (!data.gridUnitDic.TryGetValue(heightLevel, out GridUnit.GridUnit unit)) return;
            if (unit == null) return;
            data.gridUnitDic[heightLevel] = null;
            // unit.OnExitCell();
        }

        public void AddGridUnit(HeightLevel heightLevel, GridUnit.GridUnit unit)
        {
            data.gridUnitDic[heightLevel] = unit;
            // unit.OnEnterCell(this);
        }

        public void AddGridUnit(HeightLevel startHeight, HeightLevel endHeight, GridUnit.GridUnit unit)
        {
            for (HeightLevel i = startHeight; i <= endHeight; i++) data.gridUnitDic[i] = unit;
        }
    }

    public class GameGridCellData
    {
        // Units of that cell
        public readonly Dictionary<HeightLevel, GridUnit.GridUnit> gridUnitDic = new();

        public GridSurfaceBase gridSurface;

        // Type of cell
        public GridSurfaceType gridSurfaceType;

        public void OnInit()
        {
            gridSurfaceType = gridSurface == null ? GridSurfaceType.Water : gridSurface.SurfaceType;
            for (int i = 0; i < Enum.GetValues(typeof(HeightLevel)).Length - 1; i++)
                gridUnitDic.Add((HeightLevel)i, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Utilities.Grid;
using GameGridEnum;
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

        public void SetSurface(GridSurface.GridSurface surface, bool canMoving = true)
        {
            data.gridSurface = surface;
            data.gridSurfaceType = surface.SurfaceType;
            data.canMovingDirectly = canMoving;
        }


        public Unit.GridUnit GetGridUnitAtHeight(HeightLevel heightLevel)
        {
            return data.gridUnits[(int)heightLevel];
        }

        public void ClearGridUnit()
        {
            for (int i = (int)Constants.dirFirstHeightOfSurface[data.gridSurfaceType]; i < data.gridUnits.Length; i++)
                data.gridUnits[i] = null;
        }

        public void RemoveGridUnit(Unit.GridUnit removeUnit)
        {
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] == removeUnit)
                    data.gridUnits[i] = null;
        }

        public void AddGridUnit(Unit.GridUnit addUnit)
        {
            for (int i = (int)addUnit.StartHeight; i <= (int)addUnit.EndHeight; i++) data.gridUnits[i] = addUnit;
            if (data.gridSurface is not null) data.gridSurface.OnUnitEnter(addUnit);
        }

        public void RemoveGridUnitAtHeight(HeightLevel heightLevel)
        {
            if (data.gridUnits[(int)heightLevel] is null) return;
            data.gridUnits[(int)heightLevel] = null;
        }

        public void AddGridUnit(HeightLevel heightLevel, Unit.GridUnit unit)
        {
            data.gridUnits[(int)heightLevel] = unit;
        }

        public void AddGridUnit(HeightLevel startHeight, HeightLevel endHeight, Unit.GridUnit unit)
        {
            for (int i = (int)startHeight; i <= (int)endHeight; i++) data.gridUnits[i] = unit;
            if (data.gridSurface is not null) data.gridSurface.OnUnitEnter(unit);
        }

        public HeightLevel GetMaxHeight()
        {
            HeightLevel maxHeight = HeightLevel.One;
            for (int i = 0; i < data.gridUnits.Length; i++)
                if (data.gridUnits[i] is not null && (HeightLevel)i > maxHeight)
                    maxHeight = (HeightLevel)i;
            return maxHeight;
        }
        
        public List<GameGridCell> GetNeighborCells(Grid<GameGridCell, GameGridCellData> grid,
            int width, int height, Direction direction)
        {
            List<GameGridCell> gameGridCellList = new List<GameGridCell>();
            switch (direction)
            {
                default:
                case Direction.Forward:
                case Direction.Back:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Vector2Int cellPosition = GetCellPosition() + new Vector2Int(i, j);
                            GameGridCell gameGridCell = grid.GetGridCell(cellPosition.x, cellPosition.y);
                            gameGridCellList.Add(gameGridCell);
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Vector2Int cellPosition = GetCellPosition() + new Vector2Int(i, j);
                            GameGridCell gameGridCell = grid.GetGridCell(cellPosition.x, cellPosition.y);
                            gameGridCellList.Add(gameGridCell);
                        }
                    }
                    break;
            }

            return gameGridCellList;
        }
        
        public void DestroyGridUnits()
        {
            for (int i = (int)Constants.dirFirstHeightOfSurface[data.gridSurfaceType]; i < data.gridUnits.Length; i++)
            {
                if (data.gridUnits[i] is not null)
                {
                    UnityEngine.Object.Destroy(data.gridUnits[i].gameObject);
                    UnityEngine.Object.Destroy(data.gridUnits[i]);
                }
            }
        }
        
        public bool CanBuild()
        {
            int placedUnitCount = data.gridUnits.Count(unit => unit is not null);
            
            return data.gridSurfaceType == GridSurfaceType.Ground && placedUnitCount == 0;
        }
        
        public bool HasBuilding()
        {
            int placedUnitCount = data.gridUnits.Count(unit => unit is not null);

            return placedUnitCount > 0;
        }
    }

    public class GameGridCellData
    {
        public bool canMovingDirectly;

        public GridSurface.GridSurface gridSurface;

        // Type of cell
        public GridSurfaceType gridSurfaceType;
        public Unit.GridUnit[] gridUnits;
        
        public void OnInit()
        {
            gridSurfaceType = gridSurface == null ? GridSurfaceType.Water : gridSurface.SurfaceType;
            gridUnits = new Unit.GridUnit[Enum.GetValues(typeof(HeightLevel)).Length - 1];
        }
    }
}

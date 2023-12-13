using System;
using System.Collections.Generic;
using System.Linq;
using _Game.GameGrid.Unit.DynamicUnit;
using _Game.Utilities.Grid;
using GameGridEnum;
using UnityEngine;
using VinhLB;

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
        
        public bool CanBuild(GridSurfaceType surfaceType)
        {
            int unitCount = data.gridUnits.Count(unit => unit is not null);
            
            return data.gridSurfaceType == surfaceType && unitCount == 0;
        }
        
        public bool HasBuilding()
        {
            int unitCount = data.gridUnits.Count(unit => unit is BuildingUnit);

            return unitCount > 0;
        }
        
        public List<GameGridCell> GetAdjacentCells(Grid<GameGridCell, GameGridCellData> grid,
            int width, int height, Direction direction, int depth = 1, bool includingCorners = false)
        {
            List<GameGridCell> gameGridCellList = new List<GameGridCell>();
            switch (direction)
            {
                default:
                case Direction.Forward:
                case Direction.Back:
                    for (int i = -depth; i < width + depth; i++)
                    {
                        for (int j = -depth; j < height + depth; j++)
                        {
                            if (!includingCorners &&
                                ((i == -depth && j == -depth) ||
                                 (i == -depth && j == height + depth - 1) ||
                                 (i == width + depth - 1 && j == -depth) ||
                                 (i == width + depth - 1 && j == height + depth - 1)))
                            {
                                continue;
                            }

                            if (i > -depth && i < width + depth - 1 &&
                                j > -depth && j < height + depth - 1)
                            {
                                continue;
                            }
                            
                            Vector2Int cellPosition = GetCellPosition() + new Vector2Int(i, j);
                            GameGridCell gameGridCell = grid.GetGridCell(cellPosition.x, cellPosition.y);
                            gameGridCellList.Add(gameGridCell);
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int i = -depth; i < height + depth; i++)
                    {
                        for (int j = -depth; j < width + depth; j++)
                        {
                            if (!includingCorners &&
                                ((i == -depth && j == -depth) ||
                                 (i == -depth && j == width + depth - 1) ||
                                 (i == height + depth - 1 && j == -depth) ||
                                 (i == height + depth - 1 && j == width + depth - 1)))
                            {
                                continue;
                            }
                            
                            if (i > -depth && i < height + depth - 1 &&
                                j > -depth && j < width + depth - 1)
                            {
                                continue;
                            }
                            
                            Vector2Int cellPosition = GetCellPosition() + new Vector2Int(i, j);
                            GameGridCell gameGridCell = grid.GetGridCell(cellPosition.x, cellPosition.y);
                            gameGridCellList.Add(gameGridCell);
                        }
                    }
                    break;
            }

            return gameGridCellList;
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

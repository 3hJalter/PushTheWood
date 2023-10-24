using System.Collections.Generic;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public static class GridUnitFunc
    {
        public static void DebugCellInformation(GameGridCell cell)
        {
            // debug cell information
            // Cell position
            Debug.Log("Cell position: " + cell.X + ", " + cell.Y);
            // World position
            Debug.Log("World position: " + cell.WorldPos);
            // Name of unit in each height
            for (HeightLevel i = Constants.dirFirstHeightOfSurface[cell.SurfaceType]; i < cell.GetMaxHeight(); i++)
            {
                Debug.Log("Unit name at height " + i + ": " + cell.GetGridUnitAtHeight(i)?.name);
            }
        }
        
        public static Direction InvertDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Forward:
                    return Direction.Back;
                case Direction.Back:
                    return Direction.Forward;
                case Direction.None:
                default:
                    return Direction.None;
            }
        }
        
        public static Vector3Int RotateSize(Direction direction, Vector3Int sizeIn)
        {
            switch (direction)
            {
                case Direction.Left:
                case Direction.Right:
                    return new Vector3Int(sizeIn.y, sizeIn.x, sizeIn.z);
                case Direction.Forward:
                case Direction.Back:
                    return new Vector3Int(sizeIn.x, sizeIn.z, sizeIn.y);
                case Direction.None:
                default:
                    return Vector3Int.zero;
            }
        }
        
        // Get next main cell
        public static GameGridCell GetNextMainCell(GridUnit unit, Direction direction)
        {
            return LevelManager2.Ins.GetNeighbourCell(unit.MainCell, direction);
        }
        
        // Get next cells from cellInUnits
        public static HashSet<GameGridCell> GetNextCells(GridUnit unit, Direction direction)
        {
            HashSet<GameGridCell> nextCells = new();

            for (int i = 0; i < unit.cellInUnits.Count; i++)
            {
                GameGridCell nextCell = LevelManager2.Ins.GetNeighbourCell(unit.cellInUnits[i], direction);
                if (nextCell == null) continue;
                nextCells.Add(nextCell);
            }
            return nextCells;
        }
        
    }
}

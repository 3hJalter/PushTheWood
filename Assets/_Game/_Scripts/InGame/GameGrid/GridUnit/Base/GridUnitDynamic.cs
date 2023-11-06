﻿using System;
using System.Collections.Generic;
using _Game.DesignPattern;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public abstract class GridUnitDynamic : GridUnit
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;

        [SerializeField] protected Anchor anchor;
        public bool isInAction;
        public bool isLockedAction;
        protected UnitType nextUnitType;

        public GridUnitDynamicType GridUnitDynamicType => gridUnitDynamicType;
        public PoolType? PoolType => ConvertToPoolType(gridUnitDynamicType);

        public override void OnDespawn()
        {
            isInAction = false;
            base.OnDespawn();
        }

        public void OnFall(int numHeightDown, Action callback = null)
        {
            // ----- Falling Logic ----- //
            isInAction = true;
            RemoveUnitFromCell();
            // Set new startHeight and endHeight
            startHeight -= numHeightDown;
            endHeight -= numHeightDown;
            for (int i = cellInUnits.Count - 1; i >= 0; i--)
                cellInUnits[i].AddGridUnit(this);
            Vector3 newPos = GetUnitNextWorldPos(mainCell);
            Tf.DOMove(newPos, Constants.FALLING_TIME)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    isInAction = false;
                    callback?.Invoke();
                });
        }

        public bool CanFall(out int numHeightDown)
        {
            numHeightDown = int.MinValue;
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                int numHeightDownInCell = 0;
                for (HeightLevel j = startHeight - 1;
                     j >= Constants.dirFirstHeightOfSurface[cell.SurfaceType];
                     j--)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(j);
                    if (unit is null) numHeightDownInCell--;
                    else break;
                }

                if (numHeightDownInCell > numHeightDown) numHeightDown = numHeightDownInCell;
            }

            // invert numHeightDown to positive number
            numHeightDown = -numHeightDown;
            return numHeightDown > 0;
        }

        protected void OnNotMove(Direction direction, HashSet<GridUnit> nextUnits, GridUnit interactUnit = null,
            bool interactWithNextUnit = true)
        {
            if (isLockedAction) return;
            DOVirtual.DelayedCall(0.04f, () => { isLockedAction = false; });
            isLockedAction = true;
            if (!interactWithNextUnit) return;
            foreach (GridUnit unit in nextUnits) unit.OnInteract(direction, interactUnit);
        }

        public void OnOutCurrentCells()
        {
            RemoveUnitFromCell();
            cellInUnits.Clear();
        }

        protected void OnEnterNextCells(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null,
            Action fallCallback = null)
        {
            InitCellsToUnit(nextMainCell, nextCells);
            if (CanFall(out int numHeightDown)) OnFall(numHeightDown, fallCallback);
            else OnNotFall();
        }

        protected void OnEnterNextCell2(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            startHeight = nextCells is not null ? GetNextStartHeight(nextCells) : GetNextStartHeight(nextMainCell);
            endHeight = startHeight + (size.y - 1) * 2;
            if (!isMinusHalfSizeY && nextUnitState == UnitState.Up) endHeight += 1;
            InitCellsToUnit(nextMainCell, nextCells);
        }

        private HeightLevel GetNextStartHeight(GameGridCell nextCell)
        {
            HeightLevel nextStartHeight = Constants.MIN_HEIGHT;
            HeightLevel initHeight = Constants.dirFirstHeightOfSurface[nextCell.SurfaceType];
            if (initHeight > nextStartHeight) nextStartHeight = initHeight;
            for (HeightLevel heightLevel = initHeight;
                 heightLevel <= BelowStartHeight;
                 heightLevel++)
            {
                if (nextCell.GetGridUnitAtHeight(heightLevel) is null) continue;
                if (heightLevel + 1 > nextStartHeight) nextStartHeight = heightLevel + 1;
            }

            return nextStartHeight;
        }

        private HeightLevel GetNextStartHeight(HashSet<GameGridCell> nextCells)
        {
            HeightLevel nextStartHeight = Constants.MIN_HEIGHT;
            foreach (GameGridCell cell in nextCells)
            {
                HeightLevel initHeight = Constants.dirFirstHeightOfSurface[cell.SurfaceType];
                if (initHeight > nextStartHeight) nextStartHeight = initHeight;
                for (HeightLevel heightLevel = initHeight;
                     heightLevel <= BelowStartHeight;
                     heightLevel++)
                {
                    if (cell.GetGridUnitAtHeight(heightLevel) is null) continue;
                    if (heightLevel + 1 > nextStartHeight) nextStartHeight = heightLevel + 1;
                }
            }

            return nextStartHeight;
        }

        public void OnEnterNextCellWithoutFall(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            InitCellsToUnit(nextMainCell, nextCells);
        }

        protected virtual void OnNotFall()
        {

        }

        private void InitCellsToUnit(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            mainCell = nextMainCell;
            // Add all nextCells to cellInUnits
            if (nextCells is not null)
                foreach (GameGridCell nextCell in nextCells)
                    AddCell(nextCell);
            else AddCell(nextMainCell);
        }

        protected bool HasObstacleIfMove(Direction direction, out GameGridCell nextMainCell,
            out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits)
        {
            nextMainCell = LevelManager.Ins.GetNeighbourCell(mainCell, direction);
            nextCells = new HashSet<GameGridCell>();
            nextUnits = new HashSet<GridUnit>();
            bool isNextCellHasUnit = false;
            bool isNextCellIsNull = false;
            // Loop to check if all next cells at direction are exist and empty
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell neighbour = LevelManager.Ins.GetNeighbourCell(cellInUnits[i], direction);
                if (neighbour is null)
                {
                    isNextCellIsNull = true;
                    continue;
                }

                for (HeightLevel j = startHeight; j <= endHeight; j++)
                {
                    GridUnit unit = neighbour.GetGridUnitAtHeight(j);
                    if (unit is null || unit == this) continue;
                    isNextCellHasUnit = true;
                    nextUnits.Add(unit);
                }

                nextCells.Add(neighbour);
            }

            return isNextCellHasUnit || isNextCellIsNull;
        }

        protected Vector3 GetUnitWorldPos()
        {
            float offsetY = (float)startHeight / 2 * Constants.CELL_SIZE;
            if (nextUnitState == UnitState.Down) offsetY -= yOffsetOnDown;
            return mainCell.WorldPos + Vector3.up * offsetY;
        }

        protected Vector3 GetUnitNextWorldPos(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            float offsetY = (float)startHeight / 2 * Constants.CELL_SIZE;
            if (nextUnitState == UnitState.Down) offsetY -= yOffsetOnDown;
            return nextMainCell.WorldPos + Vector3.up * offsetY;
        }

        protected bool HasObstacleIfRotateMove(Direction direction, out Vector3Int sizeAfterRotate,
            out HeightLevel endHeightAfterRotate, out GameGridCell nextMainCell,
            out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits)
        {
            // Setup
            bool hasNullNextCell = false;
            bool hasUnitInNextCell = false;
            nextCells = new HashSet<GameGridCell>();
            nextUnits = new HashSet<GridUnit>();
            sizeAfterRotate = size;
            endHeightAfterRotate = endHeight;
            // Get next main cell and xAxisLoop, zAxisLoop for get all next cells
            nextMainCell = GetNextMainCell(direction);
            if (nextMainCell is null) return false;
            int xAxisLoop = direction is Direction.Left or Direction.Right
                ? size.y
                : size.x;
            int zAxisLoop = direction is Direction.Left or Direction.Right
                ? size.z
                : size.y;
            Vector2Int nexMainCellPos = nextMainCell.GetCellPosition();
            // Suppose that this unit can rotate
            sizeAfterRotate = GridUnitFunc.RotateSize(direction, size);
            // endHeightAfterRotate = startHeight + sizeAfterRotate.y - 1; // Old code
            endHeightAfterRotate = startHeight + (sizeAfterRotate.y - 1) * 2;
            if (!isMinusHalfSizeY && nextUnitState == UnitState.Up) endHeightAfterRotate += 1;
            HeightLevel endHeightForChecking = endHeightAfterRotate > endHeight ? endHeightAfterRotate : endHeight;
            // Loop to check if all next cells are exist and empty
            for (int i = 0; i < xAxisLoop; i++)
            for (int j = 0; j < zAxisLoop; j++)
            {
                Vector2Int cellPos = nexMainCellPos + new Vector2Int(i, j);
                GameGridCell cell = LevelManager.Ins.GetCell(cellPos);
                if (cell is null)
                {
                    hasNullNextCell = true;
                    continue;
                }

                for (HeightLevel k = startHeight; k <= endHeightForChecking; k++)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(k);
                    if (unit is null || unit == this) continue;
                    hasUnitInNextCell = true;
                    nextUnits.Add(unit);
                }

                nextCells.Add(cell);
            }

            // False if has null cell or has unit in next cell
            return hasNullNextCell || hasUnitInNextCell;
        }

        protected HashSet<GridUnitDynamic> GetAllAboveUnit()
        {
            // NOTE: This function is only handle one height level above, made one more loop for all higher level if need more logic
            HashSet<GridUnitDynamic> aboveUnits = new();
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GridUnit aboveUnit = cellInUnits[i].GetGridUnitAtHeight(UpperEndHeight);
                if (aboveUnit is not GridUnitDynamic unitDynamic) continue;
                aboveUnits.Add(unitDynamic);
            }

            return aboveUnits;
        }

        private GameGridCell GetNextMainCell(Direction direction)
        {
            Vector2Int nextMainCellPos = mainCell.GetCellPosition() + GetOffset();
            GameGridCell nextMainCell = LevelManager.Ins.GetCell(nextMainCellPos);
            return nextMainCell;

            Vector2Int GetOffset()
            {
                switch (direction)
                {
                    case Direction.Left:
                        return new Vector2Int(-size.y, 0);
                    case Direction.Right:
                        return new Vector2Int(size.x, 0);
                    case Direction.Forward:
                        return new Vector2Int(0, size.z);
                    case Direction.Back:
                        return new Vector2Int(0, -size.y);
                    case Direction.None:
                    default:
                        return Vector2Int.zero;
                }
            }
        }

        private void RemoveUnitFromCell()
        {
            for (int i = cellInUnits.Count - 1; i >= 0; i--)
                cellInUnits[i].RemoveGridUnit(this);
        }

        private void AddCell(GameGridCell cell)
        {
            cellInUnits.Add(cell);
            cell.AddGridUnit(this);
        }
    }
}

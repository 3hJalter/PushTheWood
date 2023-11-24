using System;
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

        protected readonly NextCellPosData nextPosData = new();
        protected UnitType nextUnitType;
        public PoolType? PoolType => ConvertToPoolType(gridUnitDynamicType);

        public override void OnDespawn()
        {
            isInAction = false;
            base.OnDespawn();
        }


        public void OnOutCurrentCells()
        {
            RemoveUnitFromCell();
            cellInUnits.Clear();
        }

        protected void OnEnterNextCell(Direction direction, GameGridCell nextMainCell, bool hasInitialOffset = true,
            HashSet<GameGridCell> nextCells = null)
        {
            Vector3 initialPos = GetUnitWorldPos(nextMainCell);
            SetHeight(nextCells is not null ? GetNextStartHeight(nextCells) : GetNextStartHeight(nextMainCell));
            InitCellsToUnit(nextMainCell, nextCells);
            Vector3 finalPos = GetUnitWorldPos();
            nextPosData.SetNextPosData(direction, initialPos, finalPos, hasInitialOffset);
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

        private void InitCellsToUnit(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            mainCell = nextMainCell;
            // Add all nextCells to cellInUnits
            if (nextCells is not null)
                foreach (GameGridCell nextCell in nextCells)
                    AddCell(nextCell);
            else AddCell(nextMainCell);
        }

        protected Vector3 GetUnitWorldPos()
        {
            float offsetY = (float)startHeight / 2 * Constants.CELL_SIZE;
            if (nextUnitState == UnitState.Down) offsetY -= yOffsetOnDown;
            return mainCell.WorldPos + Vector3.up * offsetY;
        }

        protected Vector3 GetUnitWorldPos(GameGridCell cell)
        {
            float offsetY = (float)startHeight / 2 * Constants.CELL_SIZE;
            if (nextUnitState == UnitState.Down) offsetY -= yOffsetOnDown;
            return cell.WorldPos + Vector3.up * offsetY;
        }

        protected HashSet<GridUnit> GetAboveUnits()
        {
            HashSet<GridUnit> aboveUnits = new();
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                for (HeightLevel j = UpperEndHeight; j <= cell.GetMaxHeight(); j++)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(j);
                    if (unit is GridUnitDynamic) aboveUnits.Add(unit);
                }
            }
            return aboveUnits;
        }
        
        protected HashSet<GridUnitDynamic> GetAllAboveUnit()
        {
            // New Note: Remove this after change Raft function 
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

        #region Remove after done Rule approach

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
            Vector3 newPos = GetUnitWorldPos(mainCell);
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

        public void OnEnterNextCellWithoutFall(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            InitCellsToUnit(nextMainCell, nextCells);
        }

        protected virtual void OnNotFall()
        {

        }

        protected void OnNotMove(Direction direction, HashSet<GridUnit> nextUnits, GridUnit interactUnit = null,
            bool interactWithNextUnit = true)
        {
            if (isLockedAction) return;
            DOVirtual.DelayedCall(Constants.DELAY_INTERACT_TIME, () => { isLockedAction = false; });
            isLockedAction = true;
            if (!interactWithNextUnit) return;
            foreach (GridUnit unit in nextUnits) unit.OnInteract(direction, interactUnit);
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

        #endregion

        #region TEST RULE

        public void SetMove(bool canMove)
        {
            MoveAccept = canMove;
        }

        public bool MoveAccept { get; private set; }

        #endregion
    }

    public class NextCellPosData
    {
        public Vector3 finalPos;
        public Vector3 initialPos; // not consider falling
        public bool isFalling;

        public void SetNextPosData(Direction direction, Vector3 initialPosIn, Vector3 finalPosIn,
            bool hasInitialOffset = true)
        {
            initialPos = initialPosIn;
            finalPos = finalPosIn;
            isFalling = Math.Abs(finalPosIn.y - initialPosIn.y) > 0.01;
            if (!isFalling || !hasInitialOffset) return; // make falling before go to the center of next cell
            Vector3Int dirVector3 = Constants.dirVector3[direction];
            initialPos -= new Vector3(dirVector3.x, 0, dirVector3.z) * Constants.CELL_SIZE / 2;
        }
    }
}

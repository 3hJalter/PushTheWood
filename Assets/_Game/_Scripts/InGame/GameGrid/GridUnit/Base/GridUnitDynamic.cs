using System;
using System.Collections.Generic;
using _Game.GameGrid.GridUnit.StaticUnit;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public abstract class GridUnitDynamic : GridUnit
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;
        [SerializeField] protected Anchor anchor;
        [SerializeField] protected int changeStateEndHeightOffset;
        public bool isInAction;
        private Vector3Int _initSize;
        private Quaternion _initSkinRotate;
        private Vector3 _initSkinPos;

        private void Awake()
        {
            _initSize = size;
            _initSkinRotate = skin.localRotation;
            _initSkinPos = skin.localPosition;
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One)
        {
            base.OnInit(mainCellIn, startHeightIn);
            size = _initSize;
            skin.localRotation = _initSkinRotate;
            skin.localPosition = _initSkinPos;
            isInAction = false; 
        }
        
        protected virtual void OnFall(int numHeightDown, Action callback = null)
        {
            // ----- Falling Logic ----- //
            isInAction = true;
            RemoveUnitFromCell();
            // Set new startHeight and endHeight
            startHeight -= numHeightDown;
            endHeight -= numHeightDown;
            for (int i = cellInUnits.Count - 1; i >= 0; i--)
                cellInUnits[i].AddGridUnit(this);
            // Temporary Move to new position (need animation)
            Tf.DOMove(Tf.position - new Vector3(0, numHeightDown * mainCell.Size, 0), 0.25f)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    isInAction = false;
                    callback?.Invoke();
                });
            // TODO: Change it to virtual for logic spawn Bridge of ChumpUnit (or create callback)
        }

        protected virtual bool CanFall(out int numHeightDown)
        {
            numHeightDown = int.MinValue;
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                int numHeightDownInCell = 0;
                for (HeightLevel j = startHeight - 1;
                     j >= Constants.dirFirstHeightOfSurface[cell.GetSurfaceType()];
                     j--)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(j);
                    if (unit == null) numHeightDownInCell--;
                    else break;
                }

                if (numHeightDownInCell > numHeightDown) numHeightDown = numHeightDownInCell;
            }

            // invert numHeightDown to positive number
            numHeightDown = -numHeightDown;
            return numHeightDown > 0;
        }

        protected static void OnNotMove(Direction direction, HashSet<GridUnit> nextUnits, GridUnit interactUnit = null,
            bool interactWithNextUnit = true)
        {
            if (!interactWithNextUnit) return;
            foreach (GridUnit unit in nextUnits) unit.OnInteract(direction, interactUnit);
        }

        protected void OnOutCurrentCells()
        {
            RemoveUnitFromCell();
            cellInUnits.Clear();
        }

        protected virtual void OnEnterNextCells(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null, Action fallCallback = null)
        {
            InitCellsToUnit(nextMainCell, nextCells);
            if (CanFall(out int numHeightDown)) OnFall(numHeightDown, fallCallback);
            else OnNotFall();
        }

        protected virtual void OnNotFall() {}

        private void InitCellsToUnit(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells = null)
        {
            mainCell = nextMainCell;
            // Add all nextCells to cellInUnits
            if (nextCells != null) foreach (GameGridCell nextCell in nextCells) AddCell(nextCell);
            else AddCell(nextMainCell);
        }

        protected bool CanMove(Direction direction, out GameGridCell nextMainCell,
            out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits)
        {
            nextMainCell = GameGridManager.Ins.GetNeighbourCell(mainCell, direction);
            nextCells = new HashSet<GameGridCell>();
            nextUnits = new HashSet<GridUnit>();
            bool isNextCellHasUnit = false;
            bool isNextCellIsNull = false;
            // Loop to check if all next cells at direction are exist and empty
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell neighbour = GameGridManager.Ins.GetNeighbourCell(cellInUnits[i], direction);
                if (neighbour == null)
                {
                    isNextCellIsNull = true;
                    continue;
                }

                for (HeightLevel j = startHeight; j <= endHeight; j++)
                {
                    GridUnit unit = neighbour.GetGridUnitAtHeight(j);
                    if (unit == null || unit == this) continue;
                    isNextCellHasUnit = true;
                    nextUnits.Add(unit);
                }

                nextCells.Add(neighbour);
            }

            return !isNextCellHasUnit && !isNextCellIsNull;
        }

        protected Vector3 GetUnitWorldPos()
        {
            float offsetY = (int)startHeight * Constants.CELL_SIZE;
            return mainCell.WorldPos + Vector3.up * offsetY;
        }

        protected Vector3 GetUnitNextWorldPos(GameGridCell nextMainCell)
        {
            float offsetY = (int)startHeight * Constants.CELL_SIZE;
            return nextMainCell.WorldPos + Vector3.up * offsetY;
        }

        protected bool CanRotateMove(Direction direction, out Vector3Int sizeAfterRotate,
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
            if (nextMainCell == null) return false;
            int xAxisLoop = direction is Direction.Left or Direction.Right
                ? size.y
                : size.x;
            int zAxisLoop = direction is Direction.Left or Direction.Right
                ? size.z
                : size.y;
            Vector2Int nexMainCellPos = nextMainCell.GetCellPosition();
            // Suppose that this unit can rotate
            sizeAfterRotate = RotateSize(direction, size);
            endHeightAfterRotate = startHeight + sizeAfterRotate.y - 1;
            HeightLevel endHeightForChecking = endHeightAfterRotate > endHeight ? endHeightAfterRotate : endHeight;
            // Loop to check if all next cells are exist and empty
            for (int i = 0; i < xAxisLoop; i++)
            for (int j = 0; j < zAxisLoop; j++)
            {
                Vector2Int cellPos = nexMainCellPos + new Vector2Int(i, j);
                GameGridCell cell = GameGridManager.Ins.GetCell(cellPos);
                if (cell == null)
                {
                    hasNullNextCell = true;
                    continue;
                }

                for (HeightLevel k = startHeight; k <= endHeightForChecking; k++)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(k);
                    if (unit == null || unit == this) continue;
                    hasUnitInNextCell = true;
                    nextUnits.Add(unit);
                }

                nextCells.Add(cell);
            }

            // False if has null cell or has unit in next cell
            bool canRotateMove = !(hasNullNextCell || hasUnitInNextCell);
            return canRotateMove;
        }

        private GameGridCell GetNextMainCell(Direction direction)
        {
            Vector2Int nextMainCellPos = mainCell.GetCellPosition() + GetOffset();
            GameGridCell nextMainCell = GameGridManager.Ins.GetCell(nextMainCellPos);
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

        protected static Vector3Int RotateSize(Direction direction, Vector3Int sizeIn)
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

        // SPAGHETTI CODE
        public virtual void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
        {
            
        }
    }

    public enum DUnitState
    {
        Up = 0,
        Down = 1,
    }
}

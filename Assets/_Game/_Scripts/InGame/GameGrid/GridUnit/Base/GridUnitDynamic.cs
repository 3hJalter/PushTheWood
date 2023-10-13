using System;
using System.Collections;
using System.Collections.Generic;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.Base
{
    public abstract class GridUnitDynamic : GridUnit
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;
        [SerializeField] private Anchor anchor;
        public bool isInAction;

        public void ResetAction()
        {
            isInAction = false;
        }

        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            base.OnInteract(direction, interactUnit);
            Debug.Log("Interact Dynamic Unit");
            MoveDirection(direction);
        }

        private void Fall(int numHeightDown)
        {
            // ----- Falling Logic ----- //
            RemoveUnitFromCell();
            // Set new startHeight and endHeight
            startHeight -= numHeightDown;
            endHeight -= numHeightDown;
            for (int i = cellInUnits.Count - 1; i >= 0; i--)
                cellInUnits[i].AddGridUnit(this);
            // Temporary Move to new position (need animation)
            Tf.position -= new Vector3(0, numHeightDown * mainCell.Size, 0);
        }

        private bool CanFall(out int numHeightDown)
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

        protected virtual void OnNotMove(Direction direction, HashSet<GridUnit> nextUnits,
            bool interactWithNextUnit = true)
        {
            if (!interactWithNextUnit) return;
            foreach (GridUnit unit in nextUnits) unit.OnInteract(direction);
        }

        public void MoveDirection(Direction direction)
        {
            if (isInAction) return;
            if (!CanMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits);
                return;
            }

            isInAction = true;
            // ----- Moving Logic ----- //
            // Remove all gridUnit value in gridUnitDic of cellInUnits with heightLevel in heightLevel list
            // TODO: Take all GridUnitBase in cellInUnits which above this and handle them later
            InitCellsToUnit(nextMainCell, nextCells);
            // Temporary Move to new position (need animation)
            Vector2Int moveOffset = Constants.dirVector[direction];
            Tf.position += new Vector3(moveOffset.x * mainCell.Size * size.x, 0,
                moveOffset.y * mainCell.Size * size.z);
            if (CanFall(out int numHeightDown)) Fall(numHeightDown);
            // TODO: Handle the above of old cellUnits
            // ----- End of Logic ----- //
            isInAction = false;
        }

        private void InitCellsToUnit(GameGridCell nextMainCell, HashSet<GameGridCell> nextCells)
        {
            RemoveUnitFromCell();
            cellInUnits.Clear();
            mainCell = nextMainCell;
            // Add all nextCells to cellInUnits
            foreach (GameGridCell nextCell in nextCells) AddCell(nextCell);
        }

        private bool CanMove(Direction direction, out GameGridCell nextMainCell,
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

        public void RotateMove(Direction direction)
        {
            if (isInAction) return;
            if (!CanRotateMove(direction, out Vector3Int newSize, out HeightLevel endHeightAfterRotate,
                    out GameGridCell nextMainCell, out HashSet<GameGridCell> nextCells,
                    out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits);
                return;
            }

            isInAction = true;
            anchor.ChangeAnchorPos(this, direction);
            Vector3 axis = Vector3.Cross(Vector3.up, Constants.dirVector3[direction]);
            StartCoroutine(Roll(anchor.Tf.position, axis, isDone =>
            {
                if (isDone) AfterRoll();
            }));
            return;

            void AfterRoll()
            {
                size = newSize;
                endHeight = endHeightAfterRotate;
                // TODO: Take all GridUnitBase in cellInUnits which above this and handle them later
                // Get offset of new main cell with old main cell to move the skin later
                Vector3 skinOffset = nextMainCell.WorldPos - mainCell.WorldPos;
                InitCellsToUnit(nextMainCell, nextCells);
                // Move to new Position
                float offsetY = (int)startHeight * Constants.CELL_SIZE;
                Vector3 offset = new(0, offsetY, 0);
                Tf.position = mainCell.WorldPos + offset;
                // Move the skin by minus the offset of new main cell with old main cell
                skin.position -= skinOffset;
                if (CanFall(out int numHeightDown)) Fall(numHeightDown);
                // TODO: Handle the above of old cellUnits
                // ----- End of Moving Logic ----- //
                isInAction = false;
            }
        }

        private IEnumerator Roll(Vector3 anchorIn, Vector3 axis, Action<bool> callback)
        {
            for (int i = 0; i < 90 / 5; i++)
            {
                skin.RotateAround(anchorIn, axis, 5);
                yield return new WaitForSeconds(0.01f);
            }

            callback(true);
        }

        private bool CanRotateMove(Direction direction, out Vector3Int sizeAfterRotate,
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

        private static Vector3Int RotateSize(Direction direction, Vector3Int sizeIn)
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
    }
}

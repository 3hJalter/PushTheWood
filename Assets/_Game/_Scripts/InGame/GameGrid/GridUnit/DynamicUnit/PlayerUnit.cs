using System;
using System.Collections.Generic;
using _Game.GameGrid.GridUnit.StaticUnit;
using CnControls;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class PlayerUnit : GridUnitDynamic, IInteractRootTreeUnit
    {
        [SerializeField] private Animator animator;
        private string _currentAnim = Constants.INIT_ANIM;
        private int _currentIslandID;

        private Vector2Int _currentPosition;
        private Vector2 _moveInput;
        private Vector2Int _nextPosition;
        
        private void Update()
        {
            if (isInAction) return;
            Direction direction = GetInputDirection();
            OnMove(direction);
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            ChangeAnim(Constants.IDLE_ANIM);
        }

        // SPAGHETTI CODE
        public void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
        {
            // check if above treeRootUnit has unit or if above player has unit, return
            GridUnit aboveUnit = treeRootUnit.GetAboveUnit();
            if (aboveUnit != null)
            {
                aboveUnit.OnInteract(direction);
                return;
            }

            if (mainCell.GetGridUnitAtHeight(endHeight + 1) != null) return;
            // Jump to treeRootUnit
            isInAction = true;
            Vector3 nextPos = treeRootUnit.GetMainCellWorldPos();
            Vector3 offsetY = new(0, ((float)startHeight + 1) / 2 * Constants.CELL_SIZE, 0);
            // nextPos += offsetY - treeRootUnit.offsetY;
            nextPos += offsetY;
            MoveAnimation(nextPos, () =>
            {
                OnOutCurrentCells();
                // Set new mainCell
                mainCell = treeRootUnit.MainCell;
                // Set new startHeight and endHeight
                startHeight += 1;
                endHeight += 1;
                OnEnterNextCells(treeRootUnit.MainCell);
            }, 0.2f);

        }

        private void OnMove(Direction direction)
        {
            if (direction == Direction.None) return;
            skin.DOLookAt(Tf.position + Constants.dirVector3[direction], 0.2f, AxisConstraint.Y, Vector3.up);
            if (!HasNoObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }
            if (!CanMove(nextMainCell, direction)) return;
            // 
            isInAction = true;
            OnOutCurrentCells();
            Vector3 nextPos = GetUnitNextWorldPos(nextMainCell);
            MoveAnimation(nextPos, () => { OnEnterNextCells(nextMainCell); });
        }
        
        // SUPER SPAGHETTI CODE
        private bool CanMove(GameGridCell nextMainCell, Direction direction)
        {
            GridUnit unit = mainCell.GetGridUnitAtHeight(Constants.dirFirstHeightOfSurface[GridSurfaceType.Water]);
            GridUnit nextUnit = nextMainCell.GetGridUnitAtHeight(Constants.dirFirstHeightOfSurface[GridSurfaceType.Water]);
            // Four case: both mainCell and nextMainCell is not Water, mainCell is not Water and nextMainCell is Water,
            // mainCell is Water and nextMainCell is not Water, both mainCell and nextMainCell is Water
            GridSurfaceType mainCellType = mainCell.SurfaceType;
            GridSurfaceType nextMainCellType = nextMainCell.SurfaceType;
            switch (mainCellType)
            {
                case GridSurfaceType.Ground when nextMainCellType is GridSurfaceType.Ground:
                    return true;
                case GridSurfaceType.Ground when nextMainCellType is GridSurfaceType.Water:
                    if (nextUnit is null) return false;
                    if (nextUnit is RaftUnit) return true;
                    if (nextUnit is not ChumpUnit chumpUnit) return false;
                    ChumpType type = chumpUnit.ChumpType;
                    return (type is not ChumpType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                           && (type is not ChumpType.Vertical || direction is not (Direction.Left or Direction.Right));
                case GridSurfaceType.Water when nextMainCellType is GridSurfaceType.Ground:
                    if (unit is RaftUnit) return true;
                    if (unit is not ChumpUnit chumpUnit1) return false;
                    ChumpType type1 = chumpUnit1.ChumpType;
                    return (type1 is not ChumpType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                           && (type1 is not ChumpType.Vertical || direction is not (Direction.Left or Direction.Right));
                case GridSurfaceType.Water when nextMainCellType is GridSurfaceType.Water:
                    if (unit is RaftUnit)
                    {
                        if (nextUnit is RaftUnit) return true;
                        if (nextUnit is not ChumpUnit chumpUnit2) return false;
                        ChumpType type2 = chumpUnit2.ChumpType;
                        return (type2 is not ChumpType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                               && (type2 is not ChumpType.Vertical || direction is not (Direction.Left or Direction.Right));
                    }
                    if (unit is not ChumpUnit chumpUnit3) return false;
                    ChumpType type3 = chumpUnit3.ChumpType;
                    if (nextUnit is RaftUnit) return (type3 is not ChumpType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                                                     && (type3 is not ChumpType.Vertical || direction is not (Direction.Left or Direction.Right));
                    if (nextUnit is not ChumpUnit chumpUnit4) return false;
                    ChumpType type4 = chumpUnit4.ChumpType;
                    if (type3 != type4) return false;
                    if (direction is Direction.Left or Direction.Right && type4 is ChumpType.Vertical) return false;
                    if (direction is Direction.Back or Direction.Forward && type4 is ChumpType.Horizontal) return false;
                    return true;
            }
            return true;
        }
        
        private void MoveAnimation(Vector3 newPosition, Action callback, float time = Constants.MOVING_TIME,
            string animName = Constants.WALK_ANIM)
        {
            ChangeAnim(animName);
            Tf.DOMove(newPosition, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                isInAction = false;
                ChangeAnim(Constants.IDLE_ANIM);
                callback?.Invoke();
            });
        }

        private Direction GetInputDirection()
        {
            _moveInput = new Vector2(CnInputManager.GetAxisRaw(Constants.HORIZONTAL),
                CnInputManager.GetAxisRaw(Constants.VERTICAL));
            if (_moveInput.sqrMagnitude < 0.01f) return Direction.None;
            float angle = Mathf.Atan2(_moveInput.y, -_moveInput.x);
            _moveInput = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            return Mathf.Abs(_moveInput.x) > Mathf.Abs(_moveInput.y)
                ? _moveInput.x > 0 ? Direction.Left : Direction.Right
                : _moveInput.y > 0
                    ? Direction.Forward
                    : Direction.Back;
        }

        private void ChangeAnim(string animName)
        {
            if (_currentAnim.Equals(animName)) return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.SetTrigger(_currentAnim);
        }
    }
}

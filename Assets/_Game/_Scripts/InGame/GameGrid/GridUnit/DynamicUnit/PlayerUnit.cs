using System;
using System.Collections.Generic;
using System.Linq;
using _Game.GameGrid.GridUnit.StaticUnit;
using CnControls;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class PlayerUnit : GridUnitDynamic, IInteractRootTreeUnit
    {
        [SerializeField] private Animator animator;
        private int _delayFrameCount;
        private string _currentAnim = Constants.INIT_ANIM;

        private Vector2Int _currentPosition;

        private Direction _lastDirection;
        private Vector2 _moveInput;
        private Vector2Int _nextPosition;

        private void Update()
        {
            if (isLockedAction) return;
            OnUpdate();
        }

        private void OnPushVehicle(Direction direction, GridUnit unit)
        {
            // Check if below player unit is vehicle
            GridUnit belowPlayerUnit = GetBelowUnit();
            if (belowPlayerUnit is not IVehicle vehicleUnit) return;
            if (unit.GetBelowUnit() == belowPlayerUnit) return;
            // invert direction
            direction = GridUnitFunc.InvertDirection(direction);
            vehicleUnit.OnMove(direction);
        }
        
        private void OnUpdate()
        {
            Direction direction = GetInputDirection();
            if (direction == Direction.None)
            {
                if (!isInAction) ChangeAnim(Constants.IDLE_ANIM);
                return;
            }
            if (isInAction && direction == _lastDirection) return;
            LookDirection(direction);
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                // Temporary
                if (nextUnits.Count == 1)
                {
                    GridUnit unit = nextUnits.First();
                    if (unit is not TreeRootUnit) OnPushVehicle(direction, unit);
                }
                return;
            }
            if (!CanMoveNextSurface(nextMainCell, direction)) return;
            if (isInAction)
            {
                if (direction == _lastDirection) return;
                Tf.DOKill();
                OnMovingDone(true);
            }
            OnMove(direction, nextMainCell, nextCells);
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            ChangeAnim(Constants.IDLE_ANIM);
        }
        
        public void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
        {
            // check if above treeRootUnit has unit or if above player has unit, return
            GridUnit aboveUnit = treeRootUnit.GetAboveUnit();
            if (aboveUnit is not null)
            {
                aboveUnit.OnInteract(direction);
                return;
            }
            if (mainCell.GetGridUnitAtHeight(endHeight + 1) is not null) return;
            // Kill the animation before
            Tf.DOKill();
            OnMovingDone(true);
            // Jump to treeRootUnit, Add one height 
            startHeight += 1;
            endHeight += 1;
            OnMove(direction, treeRootUnit.MainCell, null);
        }
        
        private void OnMovingDone(bool isInterrupt = false)
        {
            isInAction = false;
            if (isInterrupt) return;
            // ChangeAnim(Constants.IDLE_ANIM);
            Tf.position = GetUnitWorldPos();
        }
        
        private void OnMove(Direction direction, GameGridCell nextMainCell, HashSet<GameGridCell> nextCells)
        {
            _lastDirection = direction;
            Vector3 notFallFinalPos = GetUnitNextWorldPos(nextMainCell);
            // Out Current Cell
            OnOutCurrentCells();
            // Enter Next cell
            OnEnterNextCell2(nextMainCell);
            SetIslandId(nextMainCell);
            // Position After Fall
            Vector3 finalPos = GetUnitWorldPos();
            // If notFallFinalPos Not Same y position With finalPos, minus 1 at x or z position based on the direction
            // Can be changed if have animation instead of use Tween
            bool isFalling = false;
            if (Math.Abs(finalPos.y - notFallFinalPos.y) > 0.01)
            {
                isFalling = true;
                Vector3Int dirVector3 = Constants.dirVector3[direction];
                notFallFinalPos -= new Vector3(dirVector3.x, 0, dirVector3.z) * Constants.CELL_SIZE / 2;
            }
            // Move Animation
            isInAction = true;
            ChangeAnim(Constants.WALK_ANIM);
            Tf.DOMove(notFallFinalPos, isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    if (isFalling)
                        Tf.DOMove(finalPos, Constants.MOVING_TIME / 2).SetEase(Ease.Linear)
                            .SetUpdate(UpdateType.Fixed).OnComplete(() => { OnMovingDone(); });
                    else
                        OnMovingDone();
                });
        }

        private void SetIslandId(GameGridCell nextMainCell)
        {
            if (islandID == nextMainCell.IslandID || nextMainCell.IslandID == -1) return;
            islandID = nextMainCell.IslandID;
            LevelManager.Ins.SetFirstPlayerStepOnIsland(nextMainCell);
        }

        private void LookDirection(Direction direction)
        {
            skin.DOLookAt(Tf.position + Constants.dirVector3[direction], 0.2f, AxisConstraint.Y, Vector3.up);
        }

        private bool CanMoveNextSurface(GameGridCell nextMainCell, Direction direction)
        {
            GridUnit acceptBelowUnit = GetAcceptUnit(mainCell);
            GridUnit acceptBelowNextUnit = GetAcceptUnit(nextMainCell, true);
            // case 1: null, null -> false if current or nextCell can not move
            if (acceptBelowUnit is null && acceptBelowNextUnit is null && 
                (!mainCell.Data.canMovingDirectly || !nextMainCell.Data.canMovingDirectly)) return false;
            // case 2: null, not null -> false if nextCell can not move
            if (acceptBelowUnit is null && acceptBelowNextUnit is not null &&
                (!mainCell.Data.canMovingDirectly)) return false;
            // case 3: not null, null -> false if currentCell can not move
            if (acceptBelowUnit is not null && acceptBelowNextUnit is null &&
                (!nextMainCell.Data.canMovingDirectly)) return false;
            return true;

            GridUnit GetAcceptUnit(GameGridCell cell, bool stopWhenFirstFound = false)
            {
                for (HeightLevel height = BelowStartHeight;
                     height >= Constants.dirFirstHeightOfSurface[cell.SurfaceType];
                     height--)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(height);
                    if (unit is null) continue;
                    if (unit.UnitType is UnitType.None) continue;
                    switch (unit.UnitType)
                    {
                        case UnitType.Horizontal when direction is Direction.Left or Direction.Right:
                            return unit;
                        case UnitType.Vertical when direction is Direction.Back or Direction.Forward:
                            return unit;
                        case UnitType.Both:
                            return unit;
                        case UnitType.None:
                        default:
                            if (stopWhenFirstFound) return null;
                            break;
                    }
                }
                return null;
            }
        }
        
        private Direction GetInputDirection()
        {
            _moveInput = new Vector2(CnInputManager.GetAxisRaw(Constants.HORIZONTAL),
                CnInputManager.GetAxisRaw(Constants.VERTICAL));
            if (_moveInput.sqrMagnitude < Constants.INPUT_THRESHOLD_P2) return Direction.None;
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

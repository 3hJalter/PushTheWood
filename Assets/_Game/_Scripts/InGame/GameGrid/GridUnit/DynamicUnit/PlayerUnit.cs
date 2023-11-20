﻿using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using _Game.GameGrid.GridUnit.StaticUnit;
using DG.Tweening;
using GameGridEnum;
using HControls;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class PlayerUnit : GridUnitDynamic, IInteractRootTreeUnit, ISubject
    {
        [SerializeField] private DirectionIcon directionIcon;

        [SerializeField] private Animator animator;

        public Direction direction = Direction.None;
        private string _currentAnim = Constants.INIT_ANIM;

        private Vector2Int _currentPosition;
        private int _delayFrameCount;

        private bool _isFalling;
        private bool _isStop;

        private Direction _lastDirection;
        private Vector2 _moveInput;
        private Vector2Int _nextPosition;

        public DirectionIcon DirectionIcon => directionIcon;

        private void FixedUpdate()
        {
            if (isLockedAction) return;
            direction = HInputManager.GetDirectionInput();
            if (direction == Direction.None)
            {
                if (isInAction || _isStop) return;
                _isStop = true;
                directionIcon.OnChangeIcon(direction);
                ChangeAnim(Constants.IDLE_ANIM);
                return;
            }

            OnMoveUpdate();
        }

        public void OnInteractWithTreeRoot(Direction directionIn, TreeRootUnit treeRootUnit)
        {
            // check if above treeRootUnit has unit or if above player has unit, return
            GridUnit aboveUnit = treeRootUnit.GetAboveUnit();
            if (aboveUnit is not null)
            {
                aboveUnit.OnInteract(directionIn);
                return;
            }

            if (mainCell.GetGridUnitAtHeight(endHeight + 1) is not null) return;
            // Kill the animation before
            Tf.DOKill();
            OnMovingDone(true);
            // Jump to treeRootUnit, Add one height 
            startHeight += 1;
            endHeight += 1;
            OnMove(directionIn, treeRootUnit.MainCell);
        }

        private void OnMoveUpdate()
        {
            if (isInAction && direction == _lastDirection) return;
            _isStop = false;
            LookDirection(direction);
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> _, out HashSet<GridUnit> nextUnits))
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
            if (isInAction) return;
            OnMove(direction, nextMainCell);
        }


        private void OnPushVehicle(Direction directionIn, GridUnit unit)
        {
            // Check if below player unit is vehicle
            GridUnit belowPlayerUnit = GetBelowUnit();
            if (belowPlayerUnit is not IVehicle vehicleUnit) return;
            if (unit.GetBelowUnit() == belowPlayerUnit) return;
            // invert direction
            directionIn = GridUnitFunc.InvertDirection(directionIn);
            vehicleUnit.OnMove(directionIn);
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            ChangeAnim(Constants.IDLE_ANIM);
        }
        
        private void OnMovingDone(bool isInterrupt = false)
        {
            isInAction = false;
            if (isInterrupt) return;
            // ChangeAnim(Constants.IDLE_ANIM);
            Tf.position = GetUnitWorldPos();
        }

        private void OnMove(Direction directionIn, GameGridCell nextMainCell)
        {
            LevelManager.Ins.MoveCellViewer(nextMainCell);
            _lastDirection = directionIn;
            // Out Current Cell
            OnOutCurrentCells();
            // Enter Next cell
            OnEnterNextCell(direction, nextMainCell);
            SetIslandId(nextMainCell);
            // Can be changed if have animation instead of use Tween
            // Move Animation
            isInAction = true;
            ChangeAnim(Constants.WALK_ANIM);
            Tf.DOMove(nextPosData.initialPos, nextPosData.isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    if (nextPosData.isFalling)
                        Tf.DOMove(nextPosData.finalPos, Constants.MOVING_TIME / 2).SetEase(Ease.Linear)
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

        private void LookDirection(Direction directionIn)
        {
            skin.DOLookAt(Tf.position + Constants.dirVector3[directionIn], 0.2f, AxisConstraint.Y, Vector3.up);
            directionIcon.OnChangeIcon(directionIn);
        }

        private bool CanMoveNextSurface(GameGridCell nextMainCell, Direction directionIn)
        {
            GridUnit acceptBelowUnit = GetAcceptUnit(mainCell);
            GridUnit acceptBelowNextUnit = GetAcceptUnit(nextMainCell, true);
            // case 1: null, null -> false if current or nextCell can not move
            if (acceptBelowUnit is null && acceptBelowNextUnit is null &&
                (!mainCell.Data.canMovingDirectly || !nextMainCell.Data.canMovingDirectly)) return false;
            // case 2: null, not null -> false if nextCell can not move
            if (acceptBelowUnit is null && acceptBelowNextUnit is not null &&
                !mainCell.Data.canMovingDirectly) return false;
            // case 3: not null, null -> false if currentCell can not move
            if (acceptBelowUnit is not null && acceptBelowNextUnit is null &&
                !nextMainCell.Data.canMovingDirectly) return false;
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
                        case UnitType.Horizontal when directionIn is Direction.Left or Direction.Right:
                            return unit;
                        case UnitType.Vertical when directionIn is Direction.Back or Direction.Forward:
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

        private void ChangeAnim(string animName)
        {
            if (_currentAnim.Equals(animName)) return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.SetTrigger(_currentAnim);
        }
        
        private readonly List<IObserver> observers = new();
        
        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnNotify();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.Utilities;
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
            HUtilities.DoAfterFrames(ref _delayFrameCount, () =>
            {
                _delayFrameCount = Constants.DELAY_FRAME_TIME;
                OnUpdate();
            });
        }
        
        public void OnPushVehicle(Direction direction)
        {
            // Check if below player unit is vehicle
            GridUnit belowPlayerUnit = GetBelowUnit();
            if (belowPlayerUnit is not IVehicle vehicleUnit) return;
            // invert direction
            direction = GridUnitFunc.InvertDirection(direction);
            vehicleUnit.OnMove(direction);
        }
        
        private void OnUpdate()
        {
            Direction direction = GetInputDirection();
            if (direction == Direction.None) return;
            if (isInAction && direction == _lastDirection) return;
            LookDirection(direction);
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                // Temporary
                if (nextUnits.Count == 1 && nextUnits.First() is not TreeRootUnit)
                {
                    OnPushVehicle(direction);
                }
                return;
            }
            if (!CanMove(nextMainCell, direction)) return;
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
        
        // SPAGHETTI CODE
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
            ChangeAnim(Constants.IDLE_ANIM);
            Tf.position = GetUnitWorldPos();
        }
        
        private void OnMove(Direction direction, GameGridCell nextMainCell, HashSet<GameGridCell> nextCells)
        {
            _lastDirection = direction;
            Vector3 notFallFinalPos = GetUnitNextWorldPos(nextMainCell);
            // Out Current Cell
            OnOutCurrentCells();
            // Enter Next cell
            OnEnterNextCell2(nextMainCell, nextCells);
            SetIslandId(nextMainCell);
            // Position After Fall
            Vector3 finalPos = GetUnitWorldPos();
            // If notFallFinalPos Not Same y position With finalPos, minus 1 at x or z position based on the direction
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

        // SUPER SPAGHETTI CODE
        // TODO: Change late, each unit will be has one of three type: Horizontal, Vertical, Both, we will only handle the type instead of check class
        private bool CanMove(GameGridCell nextMainCell, Direction direction)
        {
            GridUnit unit = mainCell.GetGridUnitAtHeight(BelowStartHeight);
            GridUnit nextUnit = null;
            // Looping nextUnit from BelowStartHeight to first height of next cell until nextUnit is not null
            for (HeightLevel height = BelowStartHeight;
                 height >= Constants.dirFirstHeightOfSurface[nextMainCell.SurfaceType];
                 height--)
            {
                nextUnit = nextMainCell.GetGridUnitAtHeight(height);
                if (nextUnit is not null) break;
            }

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
                    UnitType type = chumpUnit.UnitType;
                    return (type is not UnitType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                           && (type is not UnitType.Vertical || direction is not (Direction.Left or Direction.Right));
                case GridSurfaceType.Water when nextMainCellType is GridSurfaceType.Ground:
                    if (unit is RaftUnit) return true;
                    if (unit is not ChumpUnit chumpUnit1) return false;
                    UnitType type1 = chumpUnit1.UnitType;
                    return (type1 is not UnitType.Horizontal || direction is not (Direction.Forward or Direction.Back))
                           && (type1 is not UnitType.Vertical || direction is not (Direction.Left or Direction.Right));
                case GridSurfaceType.Water when nextMainCellType is GridSurfaceType.Water:
                    if (unit is RaftUnit)
                    {
                        if (nextUnit is RaftUnit) return true;
                        if (nextUnit is not ChumpUnit chumpUnit2) return false;
                        UnitType type2 = chumpUnit2.UnitType;
                        return (type2 is not UnitType.Horizontal ||
                                direction is not (Direction.Forward or Direction.Back))
                               && (type2 is not UnitType.Vertical ||
                                   direction is not (Direction.Left or Direction.Right));
                    }

                    if (unit is not ChumpUnit chumpUnit3) return false;
                    UnitType type3 = chumpUnit3.UnitType;
                    if (nextUnit is RaftUnit)
                        return (type3 is not UnitType.Horizontal ||
                                direction is not (Direction.Forward or Direction.Back))
                               && (type3 is not UnitType.Vertical ||
                                   direction is not (Direction.Left or Direction.Right));
                    if (nextUnit is not ChumpUnit chumpUnit4) return false;
                    UnitType type4 = chumpUnit4.UnitType;
                    if (type3 != type4) return false;
                    if (direction is Direction.Left or Direction.Right && type4 is UnitType.Vertical) return false;
                    if (direction is Direction.Back or Direction.Forward && type4 is UnitType.Horizontal) return false;
                    return true;
            }

            return true;
        }

        private Direction GetInputDirection()
        {
            _moveInput = new Vector2(CnInputManager.GetAxisRaw(Constants.HORIZONTAL),
                CnInputManager.GetAxisRaw(Constants.VERTICAL));
            if (_moveInput.sqrMagnitude < Constants.INPUT_THRESHOLD_P2) return Direction.None;
            float angle = Mathf.Atan2(_moveInput.y, -_moveInput.x);
            // convert to degree angle
            Debug.Log($"Angle: {angle}");
            Debug.Log($"Angle Deg: {angle * Mathf.Rad2Deg}");
            _moveInput = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Debug.Log($"Move input: {_moveInput}");
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

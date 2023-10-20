using System;
using System.Collections.Generic;
using _Game.GameGrid.GridUnit.StaticUnit;
using CnControls;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class PlayerUnit : GridUnitDynamic
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
        public override void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
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
            if (!CanMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            // SPAGHETTI CODE
            GridUnit unit = nextMainCell.GetGridUnitAtHeight(BelowStartHeight);
            if (nextMainCell.GetSurfaceType() is GridSurfaceType.Water && unit is null) return;
            if (unit is ChumpUnit chumpUnit)
            {
                ChumpType type = chumpUnit.ChumpType;
                if ((type is ChumpType.Horizontal && direction is Direction.Forward or Direction.Back)
                    || (type is ChumpType.Vertical && direction is Direction.Left or Direction.Right))
                    return;
            }
            // 
            isInAction = true;
            OnOutCurrentCells();
            Vector3 nextPos = GetUnitNextWorldPos(nextMainCell);
            MoveAnimation(nextPos, () => { OnEnterNextCells(nextMainCell); });
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

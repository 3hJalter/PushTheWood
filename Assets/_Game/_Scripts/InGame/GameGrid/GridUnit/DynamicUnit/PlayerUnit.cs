using System.Linq;
using _Game.DesignPattern;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.GameRule.RuleEngine;
using _Game.Utilities.Grid;
using DG.Tweening;
using GameGridEnum;
using HControls;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class PlayerUnit : GridUnitDynamic, IInteractRootTreeUnit
    {
        [SerializeField] private DirectionIcon directionIcon;

        [SerializeField] private Animator animator;

        public Direction direction = Direction.None;
        private string _currentAnim = Constants.INIT_ANIM;
        
        private bool _isStop;

        private Direction _lastDirection;

        public DirectionIcon DirectionIcon => directionIcon;

        #region TEST RULE
        public RuleEngine movingRuleEngine;
        private RuleMovingData _ruleMovingData;

        private RuleMovingData RuleMovingData => _ruleMovingData ??= new RuleMovingData(this);
        
        #endregion
        
        private void FixedUpdate()
        {
            if (isLockedAction) return;
            direction = HInputManager.GetDirectionInput();
            if (direction == Direction.None)
            {
                _blockDirectionByUnit = direction;
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

        private Direction _blockDirectionByUnit = Direction.None;
        private void OnMoveUpdate()
        {
            if (isInAction && direction == _lastDirection)
            {
                Debug.Log("Lock action");
                return;
            }
            _isStop = false;
            LookDirection(direction);
            if (isInAction) return;
            if (direction == _blockDirectionByUnit) return;
            RuleMovingData.SetData(direction);  // TEST RULE
            movingRuleEngine.ApplyRules(RuleMovingData);
            // TEMPORARY FOR VEHICLE
            if (GetBelowUnit() is IVehicle vehicleUnit)
            {
                switch (MoveAccept)
                {
                    case false:
                        Direction invertDirection = GridUnitFunc.InvertDirection(direction);
                        vehicleUnit.OnMove(invertDirection);
                        return;
                    case true when RuleMovingData.blockUnits.Count > 0 &&
                                   RuleMovingData.blockUnits.First() is not TreeRootUnit:
                        invertDirection = GridUnitFunc.InvertDirection(direction);
                        vehicleUnit.OnMove(invertDirection);
                        return;
                }
            }
            //
            if (MoveAccept)
            {
                OnMove(direction, RuleMovingData.nextMainCell);
                _blockDirectionByUnit = Direction.None;
            }
            else if (RuleMovingData.blockUnits.Count > 0)
            {
                if (RuleMovingData.blockUnits.First() is not TreeRootUnit) // Temporary for TreeRootUnit
                    _blockDirectionByUnit = direction;
            }
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
            SetMove(false);
        }

        private void OnMove(Direction directionIn, GameGridCell nextMainCell)
        {
            if (isInAction) return;
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

        private void ChangeAnim(string animName)
        {
            if (_currentAnim.Equals(animName)) return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.SetTrigger(_currentAnim);
        }
    }
}

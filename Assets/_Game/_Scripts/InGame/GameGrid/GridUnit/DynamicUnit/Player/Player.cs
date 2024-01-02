using System;
using System.Collections.Generic;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using HControls;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player
{
    public class Player : GridUnitDynamic, IJumpTreeRootUnit
    {
        [SerializeField] private Animator animator;
        public bool isRideVehicle;
        public readonly Queue<Direction> InputCache = new();

        private StateMachine<Player> stateMachine;
        public StateMachine<Player> StateMachine => stateMachine;
        public override StateEnum CurrentStateId
        {
            get => StateEnum.Idle;
            set
            {
                stateMachine.ChangeState(value);
            }
        }

        private string _currentAnim = Constants.INIT_ANIM;
        private bool _isAddState;


        private bool _isWaitAFrame;
        private IVehicle _vehicle;

        public Direction Direction { get; private set; } = Direction.None;
        public InputDetection InputDetection { get; private set; } = new InputDetection();
        public float AnimSpeed => animator.speed;

        private void FixedUpdate()
        {
            if (!GameManager.Ins.IsState(GameState.InGame)) return;
            if (_isWaitAFrame)
            {
                _isWaitAFrame = false;
                Direction = InputCache.Count > 0 ? InputCache.Dequeue() : HInputManager.GetDirectionInput();
                InputDetection.GetInput(Direction);
                stateMachine?.UpdateState();
                return;
            }

            _isWaitAFrame = true;
            stateMachine?.UpdateState();
        }

        public bool CanJumpOnTreeRoot(Direction direction = Direction.None)
        {
            return true;
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (!_isAddState)
            {
                _isAddState = true;
                stateMachine = new StateMachine<Player>(this);
                AddState();
            }
            //stateMachine.Debug = true;
            stateMachine.ChangeState(StateEnum.Idle);
        }

        public override void OnDespawn()
        {
            _vehicle = null;
            base.OnDespawn();
        }

        private void AddState()
        {
            stateMachine.AddState(StateEnum.Idle, new IdlePlayerState());
            stateMachine.AddState(StateEnum.Move, new MovePlayerState());
            stateMachine.AddState(StateEnum.Interact, new InteractPlayerState());
            stateMachine.AddState(StateEnum.Push, new PushPlayerState());
            stateMachine.AddState(StateEnum.JumpUp, new JumpUpPlayerState());
            stateMachine.AddState(StateEnum.JumpDown, new JumpDownPlayerState());
            stateMachine.AddState(StateEnum.CutTree, new CutTreePlayerState());
            stateMachine.AddState(StateEnum.Die, new DiePlayerState());
            stateMachine.AddState(StateEnum.Happy, new HappyPlayerState());
        }

        public override void OnPush(Direction direction, ConditionData conditionData = null)
        {
            //NOTE: Saving when push dynamic object
            if (MovingData.blockDynamicUnits.Count > 0)
            {
                LevelManager.Ins.OnSavingState(true);
                mainCell.ValueChange();
                for (int i = 0; i < MovingData.blockDynamicUnits.Count; i++)
                {
                    MovingData.blockDynamicUnits[i].MainCell.ValueChange();
                }
                LevelManager.Ins.OnSavingState(false);
            }

            for (int i = 0; i < MovingData.blockDynamicUnits.Count; i++)
            {
                MovingData.blockDynamicUnits[i].OnBePushed(direction, this);
            }
            for (int i = 0; i < MovingData.blockStaticUnits.Count; i++)
                MovingData.blockStaticUnits[i].OnBePushed(direction, this);
        }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return stateMachine.CurrentState.Id == stateEnum;
        }

        public void ChangeAnim(string animName, bool forceAnim = false)
        {
            if (!forceAnim)
                if (_currentAnim.Equals(animName))
                    return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.SetTrigger(_currentAnim);
        }

        public void SetAnimSpeed(float speed)
        {
            animator.speed = speed;
        }

        public bool IsCurrentAnimDone()
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;
        }

        public void LookDirection(Direction directionIn)
        {
            if (directionIn is Direction.None) return;
            skin.DOLookAt(Tf.position + Constants.DirVector3[directionIn], 0.2f, AxisConstraint.Y, Vector3.up);
        }

        public bool HasVehicle()
        {
            return _vehicle is not null;
        }

        public void SetIslandId(GameGridCell nextMainCell, out bool isChangeIsland)
        {
            isChangeIsland = false;
            if (islandID == nextMainCell.IslandID || nextMainCell.IslandID == -1) return;
            islandID = nextMainCell.IslandID;
            isChangeIsland = true;
            LevelManager.Ins.CurrentLevel.SetFirstPlayerStepOnIsland(nextMainCell);
        }

        public void SetVehicle(IVehicle vehicle)
        {
            _vehicle = vehicle;
        }

        public void OnRideVehicle(Direction tDirection)
        {
            isRideVehicle = true;
            _vehicle.Ride(tDirection, this);
        }

        public bool CanRideVehicle(Direction tDirection)
        {
            // Temporary for only raft
            if (_vehicle is not Raft.Raft r) return false;
            // Check if there is a unit in the direction
            GameGridCell targetCell = mainCell.GetNeighborCell(tDirection);
            if (targetCell == null) return false;
            // Get all unit in the target cell, from Player StartHeight to Player EndHeight
            List<GridUnit> units = targetCell.GetGridUnits(StartHeight, EndHeight);
            // if no unit, mean that player can move out the raft to the target cell
            if (units == null || units.Count == 0) return false;
            // if there is a unit, check if it is a tree root
            if (units.Count == 1 && units[0] is TreeRoot) return false;
            // Set the unit to the vehicle
            r.blockDirectionUnits = units;
            return true;
        }

        #region Rule

        public ConditionMerge conditionMergeOnMoving;
        private MovingData _movingData;
        private CutTreeData _cutTreeData;

        public MovingData MovingData => _movingData ??= new MovingData(this);
        public CutTreeData CutTreeData => _cutTreeData ??= new CutTreeData(this);

        #endregion     
    }
}

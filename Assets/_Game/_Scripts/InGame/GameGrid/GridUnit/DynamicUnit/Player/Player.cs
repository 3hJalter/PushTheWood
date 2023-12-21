using System.Collections.Generic;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState;
using _Game.GameGrid.Unit.StaticUnit;
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

        private readonly Dictionary<StateEnum, IState<Player>> states = new();
        private string _currentAnim = Constants.INIT_ANIM;
        private IState<Player> _currentState;

        private bool _isAddState;


        private bool _isWaitAFrame;
        private IState<Player> _lastState;
        private IVehicle _vehicle;

        public Direction Direction { get; private set; } = Direction.None;
        public float AnimSpeed => animator.speed;

        private void FixedUpdate()
        {
            if (_isWaitAFrame)
            {
                _isWaitAFrame = false;
                Direction = InputCache.Count > 0 ? InputCache.Dequeue() : HInputManager.GetDirectionInput();
                _currentState?.OnExecute(this);
                return;
            }

            _isWaitAFrame = true;
            _currentState?.OnExecute(this);
        }

        public bool CanJumpOnTreeRoot(Direction direction = Direction.None)
        {
            return true;
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection);
            if (!_isAddState)
            {
                _isAddState = true;
                AddState();
            }

            ChangeState(StateEnum.Idle);
        }

        public override void OnDespawn()
        {
            _vehicle = null;
            base.OnDespawn();
        }

        private void AddState()
        {
            states.Add(StateEnum.Idle, new IdlePlayerState());
            states.Add(StateEnum.Move, new MovePlayerState());
            states.Add(StateEnum.Interact, new InteractPlayerState());
            states.Add(StateEnum.Push, new PushPlayerState());
            states.Add(StateEnum.JumpUp, new JumpUpPlayerState());
            states.Add(StateEnum.JumpDown, new JumpDownPlayerState());
            states.Add(StateEnum.CutTree, new CutTreePlayerState());
            states.Add(StateEnum.Die, new DiePlayerState());
            states.Add(StateEnum.Happy, new HappyPlayerState());
        }

        public override void OnPush(Direction direction, ConditionData conditionData = null)
        {
            for (int i = 0; i < MovingData.blockDynamicUnits.Count; i++)
                MovingData.blockDynamicUnits[i].OnBePushed(direction, this);
        }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return _currentState == states[stateEnum];
        }

        public void ChangeState(StateEnum stateEnum)
        {
            _currentState?.OnExit(this);
            _lastState = _currentState;
            _currentState = states[stateEnum];
            _currentState.OnEnter(this);
        }

        public void ChangeAnim(string animName, float speed = 1, bool forceAnim = false)
        {
            if (!forceAnim)
                if (_currentAnim.Equals(animName))
                    return;
            animator.ResetTrigger(_currentAnim);
            _currentAnim = animName;
            animator.speed = speed;
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
            skin.DOLookAt(Tf.position + Constants.DirVector3[directionIn], 0.2f, AxisConstraint.Y, Vector3.up);
        }

        public bool HasVehicle()
        {
            return _vehicle is not null;
        }

        public void SetIslandId(GameGridCell nextMainCell)
        {
            if (islandID == nextMainCell.IslandID || nextMainCell.IslandID == -1) return;
            islandID = nextMainCell.IslandID;
            LevelManager.Ins.SetFirstPlayerStepOnIsland(nextMainCell);
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

using System;
using System.Collections.Generic;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.GameGrid.Unit.DynamicUnit.Raft.RaftState;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Raft
{
    public class Raft : GridUnitDynamic, IVehicle
    {
        public HashSet<GridUnit> _carryUnits = new();

        private readonly Dictionary<StateEnum, IState<Raft>> states = new();
        private IState<Raft> _currentState;
        private bool _isAddState;
        private bool _isFirstSpawnDone;
        private UnitTypeXZ _lastSpawnType = UnitTypeXZ.None;

        public Player.Player player;

        public List<GridUnit> blockDirectionUnits = new();
        
        public Direction rideRaftDirection;
        
        [SerializeField] private ConditionMerge conditionMergeOnBePushed;
        public ConditionMerge ConditionMergeOnBePushed => conditionMergeOnBePushed;
        private MovingData _movingData;
        public MovingData MovingData => _movingData ??= new MovingData(this);
        
        public void Ride(Direction direction)
        {
            if (!IsCurrentStateIs(StateEnum.Idle)) return;
            // Check if there is a unit in the direction of the raft
            rideRaftDirection = direction;
            ChangeState(StateEnum.Move);
        }

        public override void OnPush(Direction direction, ConditionData conditionData = null)
        {
            List<GridUnitDynamic> blockUnits =
                conditionData is MovingData movingData ? movingData.blockDynamicUnits : new List<GridUnitDynamic>();
            for (int i = 0; i < blockUnits.Count; i++) blockUnits[i].OnBePushed(direction, this);
        }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return _currentState == states[stateEnum];
        }

        public void OnInit(GameGridCell mainCellIn, UnitTypeXZ unitTypeXZIn,
            HeightLevel startHeightIn = HeightLevel.ZeroPointFive, bool isUseInitData = false)
        {
            RotateSkin(unitTypeXZIn);
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            if (!_isAddState)
            {
                _isAddState = true;
                AddState();
            }
            ChangeState(StateEnum.Idle);
        }

        public void ChangeState(StateEnum stateEnum)
        {
            _currentState?.OnExit(this);
            _currentState = states[stateEnum];
            _currentState.OnEnter(this);
        }

        private void AddState()
        {
            states.Add(StateEnum.Idle, new IdleRaftState());
            states.Add(StateEnum.Move, new MoveRaftState());
        }

        protected override void OnEnterTriggerUpper(GridUnit triggerUnit)
        {   
            if (!IsCurrentStateIs(StateEnum.Idle)) return;
            if (triggerUnit is Player.Player playerIn)
            {
                playerIn.SetVehicle(this);
                player = playerIn;
            }
            _carryUnits.Add(triggerUnit);
        }

        protected override void OnOutTriggerUpper(GridUnit triggerUnit)
        {
            if (!IsCurrentStateIs(StateEnum.Idle)) return;
            if (triggerUnit is Player.Player playerIn)
            {
                playerIn.SetVehicle(null);
                player = null;
            }
            _carryUnits.Remove(triggerUnit);
        }

        private void RotateSkin(UnitTypeXZ type)
        {
            skin.localRotation =
                Quaternion.Euler(type is UnitTypeXZ.Horizontal
                    ? Constants.HorizontalSkinRotation
                    : Constants.VerticalSkinRotation);
            switch (_isFirstSpawnDone)
            {
                case false when type is UnitTypeXZ.Vertical:
                    size = new Vector3Int(size.z, size.y, size.x);
                    _isFirstSpawnDone = true;
                    break;
                case true when _lastSpawnType != type:
                    size = new Vector3Int(size.z, size.y, size.x);
                    break;
            }

            _lastSpawnType = type;
        }
    }
}

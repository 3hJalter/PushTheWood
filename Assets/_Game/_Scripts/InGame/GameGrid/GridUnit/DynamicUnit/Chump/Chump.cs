using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump
{
    public class Chump : GridUnitDynamic, IJumpTreeRootUnit
    {
        [SerializeField] private StateEnum onBePushedStraightState;
        [SerializeField] private StateEnum onBePushedPerpendicularState;
        [SerializeField] private Raft.Raft raftPrefab;

        private StateMachine<Chump> stateMachine;
        public StateMachine<Chump> StateMachine => stateMachine;
        public override StateEnum CurrentStateId
        {
            get => stateMachine != null ? stateMachine.CurrentStateId : StateEnum.None;
            set
            {
                stateMachine.ChangeState(value);
            }
        }

        private bool _isAddState;
        // Hand
        public Chump TriggerChump { get; private set; }

        public Raft.Raft RaftPrefab => raftPrefab;

        public bool CanJumpOnTreeRoot(Direction direction = Direction.None)
        {
            if (gridUnitDynamicType is GridUnitDynamicType.ChumpHigh) return false;
            return direction switch
            {
                Direction.Forward or Direction.Back when UnitTypeXZ is UnitTypeXZ.None
                    or UnitTypeXZ.Vertical => true,
                Direction.Left or Direction.Right when UnitTypeXZ is UnitTypeXZ.None
                    or UnitTypeXZ.Horizontal => true,
                _ => false
            };
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (!_isAddState)
            {
                _isAddState = true;
                stateMachine = new StateMachine<Chump>(this);
                //stateMachine.Debug = true;
                AddState();
            }
            stateMachine.ChangeState(StateEnum.Idle);
        }

        private void AddState()
        {
            stateMachine.AddState(StateEnum.Idle, new IdleChumpState());
            stateMachine.AddState(StateEnum.FormRaft, new FormRaftChumpState());
            stateMachine.AddState(StateEnum.Move, new MoveChumpState());
            stateMachine.AddState(StateEnum.Roll, new RollChumpState());
            stateMachine.AddState(StateEnum.TurnOver, new TurnOverChumpState());
            stateMachine.AddState(StateEnum.Fall, new FallChumpState());
            stateMachine.AddState(StateEnum.RollBlock, new RollBlockChumpState());
            stateMachine.AddState(StateEnum.Emerge, new EmergeChumpState());
        }

        public override void OnPush(Direction direction, ConditionData conditionData = null)
        {
            List<GridUnitDynamic> blockUnits =
                conditionData is MovingData movingData ? movingData.blockDynamicUnits : new List<GridUnitDynamic>();
            for (int i = 0; i < blockUnits.Count; i++) blockUnits[i].OnBePushed(direction, this);
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {

            #region Check Condition

            ChumpBeInteractedData.SetData(direction, pushUnit);
            if (!conditionMergeOnBeInteracted.IsApplicable(ChumpBeInteractedData))
                // if (_currentState != states[StateEnum.Idle])
                //     ChangeState(StateEnum.Idle); 
                return;

            #endregion

            base.OnBePushed(direction, pushUnit);
            if (unitTypeY is UnitTypeY.Up)
            {
                stateMachine.ChangeState(StateEnum.TurnOver);
            }
            else
            {
                if (unitTypeXZ is UnitTypeXZ.Horizontal)
                {
                    if (direction is Direction.Left or Direction.Right)
                        stateMachine.ChangeState(onBePushedStraightState);
                    else if (direction is Direction.Forward or Direction.Back)
                        stateMachine.ChangeState(onBePushedPerpendicularState);
                }
                else if (unitTypeXZ is UnitTypeXZ.Vertical)
                {
                    if (direction is Direction.Left or Direction.Right)
                        stateMachine.ChangeState(onBePushedPerpendicularState);
                    else if (direction is Direction.Forward or Direction.Back)
                        stateMachine.ChangeState(onBePushedStraightState);
                }
            }
        }

        protected override void OnOutTriggerBelow(GridUnit triggerUnit)
        {
            base.OnOutTriggerBelow(triggerUnit);
            if (IsCurrentStateIs(StateEnum.Idle)) stateMachine.ChangeState(StateEnum.Fall);
        }

        protected override void OnEnterTriggerUpper(GridUnit triggerUnit)
        {
            if (triggerUnit is not Chump triggerChump) return;
            // Only need one below Unit to check if the TriggerChump can form Raft
            if (triggerChump.IsCurrentStateIs(StateEnum.FormRaft)) return;
            if (!triggerChump.IsOnWater()) return;
            // Loop all below unit of TriggerChump, include this unit self
            foreach (GridUnit belowUnit in triggerChump.belowUnits)
            {
                // if one of all below unit of TriggerChump is not Chump, return
                if (belowUnit is not Chump) return;
                // if the TriggerChump is Down and one of all below unit of TriggerChump is not same UnitTypeXZ, return
                if (triggerChump.UnitTypeY is UnitTypeY.Up) continue;
                if (triggerChump.UnitTypeXZ != belowUnit.UnitTypeXZ) return;
            }

            TriggerChump = triggerChump;
            TriggerChump.StateMachine.ChangeState(StateEnum.Fall);
        }

        public bool IsOnWater()
        {
            return startHeight == Constants.DirFirstHeightOfSurface[GridSurfaceType.Ground] &&
                   cellInUnits.All(t => t.SurfaceType is GridSurfaceType.Water);
        }
        public bool IsInWater()
        {
            return startHeight == Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] &&
                   cellInUnits.All(t => t.SurfaceType is GridSurfaceType.Water);
        }
        public MovingData MainMovingData
        {
            get => TurnOverData.UsingTurnId > MovingData.UsingTurnId ? TurnOverData : MovingData;
        }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return stateMachine.CurrentState.Id == stateEnum;
        }

        public void SwitchType(Direction direction)
        {
            if (unitTypeY is UnitTypeY.Up)
            {
                unitTypeY = UnitTypeY.Down;
                unitTypeXZ = direction switch
                {
                    Direction.Back or Direction.Forward => UnitTypeXZ.Vertical,
                    Direction.Left or Direction.Right => UnitTypeXZ.Horizontal,
                    _ => unitTypeXZ
                };
            }
            else
            {
                unitTypeY = UnitTypeY.Up;
                unitTypeXZ = UnitTypeXZ.None;
            }
        }

        #region Rule

        [SerializeField] private ConditionMerge conditionMergeOnBeInteracted;
        [SerializeField] private ConditionMerge conditionMergeOnBePushed;

        public ConditionMerge ConditionMergeOnBeInteracted => conditionMergeOnBeInteracted;
        public ConditionMerge ConditionMergeOnBePushed => conditionMergeOnBePushed;

        private ChumpBeInteractedData _chumpBeInteractedData;
        private TurnOverData turnOverData;
        private MovingData _movingData;

        public ChumpBeInteractedData ChumpBeInteractedData =>
            _chumpBeInteractedData ??= new ChumpBeInteractedData(this);

        public TurnOverData TurnOverData => turnOverData ??= new TurnOverData(this);
        public MovingData MovingData => _movingData ??= new MovingData(this);
        #endregion
    }
}

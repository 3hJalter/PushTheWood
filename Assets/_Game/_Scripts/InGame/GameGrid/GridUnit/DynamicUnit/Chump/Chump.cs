using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using GameGridEnum;
using UnityEngine;
using _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump
{
    public class Chump : GridUnitDynamic, IJumpTreeRootUnit
    {
        [SerializeField] private StateEnum onBePushedStraightState;
        [SerializeField] private StateEnum onBePushedPerpendicularState;

        private readonly Dictionary<StateEnum, IState<Chump>> states = new();
        private IState<Chump> _currentState;
        private bool _isAddState;
        // Hand
        private Chump _triggerChump; // Set trigger chump to use in SpawnRaftChumpState
        [SerializeField] private Raft.Raft raftPrefab;
        public Chump TriggerChump => _triggerChump;
        public Raft.Raft RaftPrefab => raftPrefab;
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

        private void AddState()
        {
            states.Add(StateEnum.Idle, new IdleChumpState());
            states.Add(StateEnum.FormRaft, new FormRaftChumpState());
            states.Add(StateEnum.Move, new MoveChumpState());
            states.Add(StateEnum.Roll, new RollChumpState());
            states.Add(StateEnum.TurnOver, new TurnOverChumpState());
            states.Add(StateEnum.Fall, new FallChumpState());
            states.Add(StateEnum.RollBlock, new RollBlockChumpState());
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
            {
                // if (_currentState != states[StateEnum.Idle])
                //     ChangeState(StateEnum.Idle); 
                return;
            }

            #endregion

            base.OnBePushed(direction, pushUnit);
            if (unitTypeY is UnitTypeY.Up)
            {
                ChangeState(StateEnum.TurnOver);
            }
            else
            {
                if (unitTypeXZ is UnitTypeXZ.Horizontal)
                {
                    if (direction is Direction.Left or Direction.Right)
                        ChangeState(onBePushedStraightState);
                    else if (direction is Direction.Forward or Direction.Back)
                        ChangeState(onBePushedPerpendicularState);
                }
                else if (unitTypeXZ is UnitTypeXZ.Vertical)
                {
                    if (direction is Direction.Left or Direction.Right)
                        ChangeState(onBePushedPerpendicularState);
                    else if (direction is Direction.Forward or Direction.Back)
                        ChangeState(onBePushedStraightState);
                }
            }
        }

        protected override void OnOutTriggerBelow(GridUnit triggerUnit)
        {
            base.OnOutTriggerBelow(triggerUnit);
            if (IsCurrentStateIs(StateEnum.Idle))
            {
                ChangeState(StateEnum.Fall);
            }
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
            _triggerChump = triggerChump;
            _triggerChump.ChangeState(StateEnum.FormRaft);
        }

        private bool IsOnWater()
        {
            return startHeight == Constants.DirFirstHeightOfSurface[GridSurfaceType.Ground] &&
                   cellInUnits.All(t => t.SurfaceType is GridSurfaceType.Water);

        }

        // public override bool CanMoveFromUnit(Direction direction, GridUnit pushUnit = null)
        // {
        //     if (!IsCurrentStateIs(StateEnum.Idle)) return false;
        //     ChumpBeInteractedData.SetData(direction, pushUnit);
        //     if (!conditionMergeOnBeInteracted.IsApplicable(ChumpBeInteractedData)) return false;
        //     if (unitTypeY is UnitTypeY.Up)
        //     {
        //         TurnOverData.SetData(ChumpBeInteractedData.inputDirection);
        //         return ConditionMergeOnBePushed.IsApplicable(TurnOverData);
        //     }
        //
        //     if (unitTypeXZ is UnitTypeXZ.Horizontal)
        //     {
        //         if (direction is Direction.Left or Direction.Right)
        //         {
        //             if (onBePushedStraightState is StateEnum.TurnOver)
        //             {
        //                 TurnOverData.SetData(ChumpBeInteractedData.inputDirection);
        //                 return ConditionMergeOnBePushed.IsApplicable(TurnOverData);
        //             }
        //
        //             if (onBePushedStraightState is StateEnum.Move)
        //             {
        //                 MovingData.SetData(ChumpBeInteractedData.inputDirection);
        //                 return ConditionMergeOnBePushed.IsApplicable(MovingData);
        //             }
        //
        //             return false;
        //         }
        //
        //         if (direction is Direction.Forward or Direction.Back &&
        //             onBePushedPerpendicularState is StateEnum.Roll)
        //         {
        //             MovingData.SetData(ChumpBeInteractedData.inputDirection);
        //             return ConditionMergeOnBePushed.IsApplicable(MovingData);
        //         }
        //
        //         return false;
        //     }
        //
        //     if (unitTypeXZ is UnitTypeXZ.Vertical)
        //     {
        //         if (direction is Direction.Left or Direction.Right && onBePushedPerpendicularState is StateEnum.Roll)
        //         {
        //             MovingData.SetData(ChumpBeInteractedData.inputDirection);
        //             return ConditionMergeOnBePushed.IsApplicable(MovingData);
        //         }
        //
        //         if (direction is Direction.Forward or Direction.Back)
        //         {
        //             if (onBePushedStraightState is StateEnum.TurnOver)
        //             {
        //                 TurnOverData.SetData(ChumpBeInteractedData.inputDirection);
        //                 return ConditionMergeOnBePushed.IsApplicable(TurnOverData);
        //             }
        //
        //             if (onBePushedStraightState is StateEnum.Move)
        //             {
        //                 MovingData.SetData(ChumpBeInteractedData.inputDirection);
        //                 return ConditionMergeOnBePushed.IsApplicable(MovingData);
        //             }
        //
        //             return false;
        //         }
        //
        //         return false;
        //     }
        //
        //     return false;
        // }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return _currentState == states[stateEnum];
        }

        public void ChangeState(StateEnum stateEnum)
        {
            _currentState?.OnExit(this);
            _currentState = states[stateEnum];
            _currentState.OnEnter(this);
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
    }
}

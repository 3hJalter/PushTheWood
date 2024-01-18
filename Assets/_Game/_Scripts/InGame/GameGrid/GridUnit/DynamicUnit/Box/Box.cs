using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Box.BoxState;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Box
{
    public class Box : GridUnitDynamic
    {
        private StateMachine<Box> _stateMachine;

        public StateMachine<Box> StateMachine => _stateMachine;

        private bool _isAddState;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRos = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRos);
            if (!_isAddState)
            {
                _isAddState = true;
                _stateMachine = new StateMachine<Box>(this);
                AddState();
            }
            _stateMachine.ChangeState(StateEnum.Idle);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            _stateMachine.ChangeState(StateEnum.Idle);
        }

        public override void OnPush(Direction direction, ConditionData conditionData = null)
        {
            if (conditionData is not MovingData movingData) return;
            for (int i = 0; i < movingData.blockDynamicUnits.Count; i++) movingData.blockDynamicUnits[i].OnBePushed(direction, this);
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            BeInteractedData.SetData(direction, pushUnit);
            if (!ConditionMergeOnBeInteracted.IsApplicable(BeInteractedData)) return;
            
            base.OnBePushed(direction, pushUnit);
            _stateMachine.ChangeState(StateEnum.Move);
        }
        
        public bool IsInWater()
        {
            return startHeight <= Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] + FloatingHeightOffset &&
                   cellInUnits.All(t => t.SurfaceType is GridSurfaceType.Water);
        }

        public override bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return _stateMachine.CurrentState.Id == stateEnum;
        }
        
        private void AddState()
        {
            _stateMachine.AddState(StateEnum.Idle, new IdleBoxState());
            _stateMachine.AddState(StateEnum.Move, new MoveBoxState());
            _stateMachine.AddState(StateEnum.Fall, new FallBoxState());
            _stateMachine.AddState(StateEnum.Emerge, new EmergeBoxState());
        }

        public override StateEnum CurrentStateId 
        { 
            get => _stateMachine?.CurrentStateId ?? StateEnum.Idle;
            set => _stateMachine.ChangeState(value);
        }

        #region Ruling

        [SerializeField] private ConditionMerge conditionMergeOnBePushed;
        [SerializeField] private ConditionMerge conditionMergeOnBeInteracted;
        public ConditionMerge ConditionMergeOnBePushed => conditionMergeOnBePushed;
        private ConditionMerge ConditionMergeOnBeInteracted => conditionMergeOnBeInteracted;

        private MovingData _movingData;
        private BeInteractedData _beInteractedData;
        
        public MovingData MovingData => _movingData ??= new MovingData(this);
        public BeInteractedData BeInteractedData => _beInteractedData ??= new BeInteractedData(this);
        
        #endregion
    }
}

/*
using System;
using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.GameRule.RuleEngine;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class BoxUnit : GridUnitDynamic
    {
        [SerializeField] private RuleEngine ruleInteractEngine;
        [SerializeField] private RuleEngine ruleMovingEngine;
        
        private RuleInteractData _ruleInteractData;
        private RuleMovingData _ruleMovingData;
        
        
        private RuleInteractData RuleInteractData => _ruleInteractData ??= new RuleInteractData(this);
        private RuleMovingData RuleMovingData => _ruleMovingData ??= new RuleMovingData(this);
        
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            base.OnInteract(direction, interactUnit);
            if (isInAction) return;
            RuleInteractData.SetData(direction, interactUnit);
            ruleInteractEngine.ApplyRules(RuleInteractData);
            if (!RuleInteractData.isInteractAccept) return;
            OnPush(direction);
        }
        
        private void OnPush(Direction direction)
        {
            RuleMovingData.SetData(direction);
            ruleMovingEngine.ApplyRules(RuleMovingData);
            if (!MoveAccept) return;
            OnMove(direction, RuleMovingData.nextMainCell, RuleMovingData.nextCells);
        }

        private void OnMove(Direction direction, GameGridCell nextMainCell, HashSet<GameGridCell> nextCells,
            Action nextAction = null)
        {
            if (isInAction) return;

            #region Get above units

            HashSet<GridUnit> aboveUnits = new();
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                for (HeightLevel j = endHeight + 1; j <= cell.GetMaxHeight(); j++)
                {
                    GridUnit unit = cell.GetGridUnitAtHeight(j);
                    if (unit is null) continue;
                    if (unit is GridUnitDynamic) aboveUnits.Add(unit);
                }
            }

            #endregion

            SetMove(true);
            OnOutCurrentCells();
            OnEnterNextCell(direction, nextMainCell, false, nextCells);
            isInAction = true;
            Tf.DOMove(nextPosData.initialPos, nextPosData.isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    if (nextPosData.isFalling)
                        Tf.DOMove(nextPosData.finalPos, Constants.MOVING_TIME / 2).SetEase(Ease.Linear)
                            .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                            {
                                OnMovingDone(nextPosData.isFalling, aboveUnits, nextAction);
                            });
                    else OnMovingDone(nextPosData.isFalling, aboveUnits, nextAction);;
                });
        }

        private void OnMovingDone(bool isFalling, HashSet<GridUnit> aboveUnits, Action nextAction = null)
        {
            isInAction = false;
            SetMove(false);
            foreach (GridUnit unit in aboveUnits)
                if (unit is GridUnitDynamic dynamicUnit && dynamicUnit.CanFall(out int numHeightDown))
                    dynamicUnit.OnFall(numHeightDown);
            nextAction?.Invoke();
        }
    }
}
 */

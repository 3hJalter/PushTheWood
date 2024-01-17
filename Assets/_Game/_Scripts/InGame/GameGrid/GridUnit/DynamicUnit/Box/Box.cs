using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Box
{
    public class Box : GridUnitDynamic
    {
        // Check Old Commit
        public override StateEnum CurrentStateId 
        { 
            get => throw new System.NotImplementedException(); 
            set => throw new System.NotImplementedException(); 
        }
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

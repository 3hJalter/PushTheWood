using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Bomb
{
    public class Bomb : GridUnitDynamic
    {
        // Check Old Commit
        public override StateEnum CurrentStateId { 
            get => throw new System.NotImplementedException(); 
            set => throw new System.NotImplementedException(); }
    }
}

/*

using System;
using System.Collections.Generic;
using _Game._Scripts.Utilities;
using _Game.DesignPattern;
using _Game.GameRule.RuleEngine;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class BombUnit : GridUnitDynamic
    {
        [SerializeField] private RuleEngine ruleInteractEngine;
        [SerializeField] private RuleEngine ruleRollingEngine;
        
        private RuleInteractData _ruleInteractData;
        private RuleRollingData _ruleRollingData;
        
        private RuleInteractData RuleInteractData => _ruleInteractData ??= new RuleInteractData(this);
        private RuleRollingData RuleRollingData => _ruleRollingData ??= new RuleRollingData(this);
        
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
            RuleRollingData.SetData(direction);
            ruleRollingEngine.ApplyRules(RuleRollingData);
            if (!MoveAccept)
            {
                if (!_hasRoll) return;
                OnActiveBomb();
                
                return;
            }
            OnRoll(direction, RuleRollingData.nextSize, RuleRollingData.nextEndHeight, RuleRollingData.nextMainCell,
                RuleRollingData.nextCells);
        }

        private bool _hasRoll;
        
        private void OnRoll(Direction direction, Vector3Int sizeAfterRotate, HeightLevel endHeightAfterRotate,
            GameGridCell nextMainCell, HashSet<GameGridCell> nextCells)
        {
            if (isInAction) return;
            _hasRoll = true;
            SetMove(true);
            size = sizeAfterRotate;
            endHeight = endHeightAfterRotate;
            HashSet<GridUnit> aboveUnits = GetAboveUnits();
            OnOutCurrentCells();
            OnEnterNextCell(direction, nextMainCell, false, nextCells);
            isInAction = true;

            Vector3 rotate = Constants.DirVector3[direction] * 360;
            rotate = rotate.Change(x: -rotate.z, z: -rotate.x);
            
            skin.DORotate(rotate, Constants.MOVING_TIME, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    skin.localRotation = Quaternion.identity;
                });
            
            Tf.DOMove(nextPosData.initialPos, nextPosData.isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    Tf.position = nextPosData.initialPos;
                    if (!nextPosData.isFalling)
                    {
                        OnMovingDone(false, aboveUnits);
                        if (unitState == nextUnitState && gameObject.activeSelf) OnPush(direction);
                        else OnActiveBomb();
                        unitState = nextUnitState;
                        unitType = nextUnitType;
                    }
                    else
                    {
                        // Tween to final position
                        Tf.DOMove(nextPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                            .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                            {
                                OnMovingDone(true, aboveUnits);
                                Tf.position = GetUnitWorldPos(nextMainCell);
                                OnActiveBomb();
                                unitState = nextUnitState;
                                unitType = nextUnitType;
                            });
                    }
                });
        }

        private void OnActiveBomb()
        {
            // Get all unit in 4 direction
            HashSet<GridUnit> units = new();
            GridUnit unit = GetNeighborUnit(Direction.Left);
            if (unit is not null) units.Add(unit);
            unit = GetNeighborUnit(Direction.Right);
            if (unit is not null) units.Add(unit);
            unit = GetNeighborUnit(Direction.Forward);
            if (unit is not null) units.Add(unit);
            unit = GetNeighborUnit(Direction.Back);
            if (unit is not null) units.Add(unit);
            // Destroy all unit
            foreach (GridUnit gridUnit in units)
            {
                gridUnit.OnDespawn();
            }
            // Destroy this unit
            _hasRoll = false;
            OnDespawn();
        }
        
        private GridUnit GetNeighborUnit(Direction direction)
        {
            GameGridCell neighborCell = LevelManager.Ins.GetNeighbourCell(mainCell, direction);
            GridUnit unit = neighborCell?.GetGridUnitAtHeight(startHeight);
            return unit;
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
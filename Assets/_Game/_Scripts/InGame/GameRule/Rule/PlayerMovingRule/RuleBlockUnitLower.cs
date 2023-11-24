using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridUnit;
using _Game.GameRule.RuleEngine;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleBlockUnitLower", menuName = "RuleSO/Moving/RuleBlockUnitLower", order = 1)]
    public class RuleBlockUnitLower : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return false;
            if (data.runRuleUnit is not GridUnitDynamic player) return false;
            // Get Below Player Information
            GameGridCell currentCell = player.MainCell;
            bool canMoveDirectly = currentCell.Data.canMovingDirectly;
            HeightLevel belowSurfaceStartHeight = Constants.dirFirstHeightOfSurface[currentCell.SurfaceType];
            // Condition
            if (canMoveDirectly) return true;
            if (player.StartHeight <= belowSurfaceStartHeight) return false;
            GridUnit belowUnit = player.GetBelowUnit();
            if (belowUnit is null) return false;
            return belowUnit.UnitType switch
            {
                UnitType.None => false,
                UnitType.Both => true,
                UnitType.Vertical => data.runDirection is Direction.Forward or Direction.Back,
                UnitType.Horizontal => data.runDirection is Direction.Left or Direction.Right,
                _ => true
            };
        }

        public void Apply(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return;
            if (data.runRuleUnit is GridUnitDynamic dUnit) dUnit.SetMove(true);
        }

        public void Reverse(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return;
            if (data.runRuleUnit is GridUnitDynamic dUnit) dUnit.SetMove(false);
        }
    }
}

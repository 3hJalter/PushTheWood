using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameRule.RuleEngine;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleBlockUnitNextLower", menuName = "RuleSO/Moving/RuleBlockUnitNextLower", order = 1)]
    public class RuleBlockUnitNextLower : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return false;
            if (data.runRuleUnit is not GridUnitDynamic player) return false;
            bool canMoveDirectly = data.nextMainCell.Data.canMovingDirectly;
            HeightLevel belowSurfaceStartHeight = Constants.dirFirstHeightOfSurface[data.nextMainCell.SurfaceType];
            // Get all objects in next cell from below this (Player) to Surface Start Height
            List<GridUnit> unitsInNextCell = new();
            for (HeightLevel i = player.StartHeight - 1; i >= belowSurfaceStartHeight; i--)
            {
                GridUnit unit = data.nextMainCell.GetGridUnitAtHeight(i);
                if (unit is null || unitsInNextCell.Contains(unit)) continue;
                unitsInNextCell.Add(unit);
            }
            // if no objects
            if (unitsInNextCell.Count == 0)
            {
                return player.StartHeight - 2 <= belowSurfaceStartHeight && canMoveDirectly;
            }
            
            GridUnit firstUnit = unitsInNextCell[0];
            HeightLevel firstObjectEndHeight = firstUnit.EndHeight;
            // If the distance = this (Player) sH - first object of the list End Height (eH) > 2 -> Rule Reject
            int distance = player.StartHeight - firstObjectEndHeight;
            switch (distance)
            {
                case > 2:
                    return false;
                case 2:
                    return firstUnit.UnitType switch
                    {
                        UnitType.None => false,
                        UnitType.Both => true,
                        UnitType.Vertical => data.runDirection is Direction.Forward or Direction.Back,
                        UnitType.Horizontal => data.runDirection is Direction.Left or Direction.Right,
                        _ => true
                    };
                default:
                    switch (firstUnit.UnitType)
                    {
                        case UnitType.None:
                            firstUnit.OnInteract(data.runDirection, data.runRuleUnit);
                            return false;
                        case UnitType.Both:
                        case UnitType.Vertical when data.runDirection is Direction.Forward or Direction.Back:
                            return true;
                        case UnitType.Vertical:
                            firstUnit.OnInteract(data.runDirection, data.runRuleUnit);
                            return false;
                        case UnitType.Horizontal when data.runDirection is Direction.Left or Direction.Right:
                            return true;
                        case UnitType.Horizontal:
                            firstUnit.OnInteract(data.runDirection, data.runRuleUnit);
                            return false;
                        default:
                            return true;
                    }
            }

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

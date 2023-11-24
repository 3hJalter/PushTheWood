using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleBlockUnitNext", menuName = "RuleSO/Moving/RuleBlockUnitNext", order = 1)]
    public class RuleBlockUnitNext : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return false;
            if (data.runRuleUnit is not GridUnitDynamic) return false;
            if (data.blockUnits.Count == 0) return true;
            bool isBlockUnitsMove = true;
            foreach (GridUnit unit in data.blockUnits)
            {
                unit.OnInteract(data.runDirection, data.runRuleUnit);
                // TODO: Need to handle the case when the unit is not movable (Just GridUnit)
                if (unit is GridUnitDynamic dUnit)
                {
                    if (!dUnit.MoveAccept) isBlockUnitsMove = false;
                }
                else isBlockUnitsMove = false;
            }
            return isBlockUnitsMove;
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

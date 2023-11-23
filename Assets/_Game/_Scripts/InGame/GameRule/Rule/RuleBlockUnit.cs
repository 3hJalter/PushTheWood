using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleBlockUnits", menuName = "RuleSO/Moving/RuleBlockUnits", order = 1)]
    public class RuleBlockUnit : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return false;
            if (data.runRuleUnit is not GridUnitDynamic) return false;
            if (data.blockUnits.Count == 0) return true;
            bool isBlockUnitsMove = true;
            foreach (GameUnit unit in data.blockUnits)
            {
                if (unit is GridUnitDynamic dUnit)
                {
                    dUnit.OnMove(data.runDirection);
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

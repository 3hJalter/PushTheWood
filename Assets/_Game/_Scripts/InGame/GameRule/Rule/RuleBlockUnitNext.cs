using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleBlockUnitNext", menuName = "RuleSO/Moving/RuleBlockUnitNext", order = 1)]
    public class RuleBlockUnitNext : ScriptableObject, IRule
    {
        [Tooltip("True when the unit also want to move if the block units move")]
        [SerializeField] private bool canMoveWithBlockUnits = true; 
        public bool IsApplicable(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return false;
            if (data.runRuleUnit is not GridUnitDynamic) return false;
            if (data.blockUnits.Count == 0) return true;
            bool isBlockUnitsMove = canMoveWithBlockUnits;
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

        public void CancelApply(RuleEngineData dataIn)
        {
            if (dataIn is not RuleMovingData data) return;
            if (data.runRuleUnit is GridUnitDynamic dUnit) dUnit.SetMove(false);
        }
    }
}

using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit;
using _Game.GameRule.RuleEngine;
using UnityEngine;

namespace _Game.GameRule.Rule
{
    [CreateAssetMenu(fileName = "RuleCanInteract", menuName = "RuleSO/Interact/RuleCanInteract", order = 1)]
    public class RuleCanInteract : ScriptableObject, IRule
    {
        public bool IsApplicable(RuleEngineData data)
        {
            if (data is not RuleInteractData interactData) return false;
            if (interactData.interactedUnit is PlayerUnit) return true;
            UnitType type = interactData.runRuleUnit.UnitType;
            Direction direction = interactData.runDirection;
            switch (type)
            {
                case UnitType.Horizontal:
                    return direction is Direction.Back or Direction.Forward;
                case UnitType.Vertical:
                    return direction is Direction.Left or Direction.Right;
                case UnitType.Both:
                case UnitType.None:
                default:
                    return false;
            }
        }

        public void Apply(RuleEngineData dataIn)
        {
            if (dataIn is not RuleInteractData data) return;
            data.isInteractAccept = true;
        }

        public void CancelApply(RuleEngineData dataIn)
        {
            if (dataIn is not RuleInteractData data) return;
            data.isInteractAccept = false;
        }
    }
}

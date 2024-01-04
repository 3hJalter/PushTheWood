using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using UnityEngine;

namespace _Game._Scripts.InGame.GameCondition.Condition
{
    [CreateAssetMenu(fileName = "CanChumpBeInteracted", menuName = "ConditionSO/Interact/CanBeInteract", order = 1)]
    public class CanChumpBeInteracted : ScriptableObject, ICondition
    {
        public bool IsApplicable(ConditionData dataIn)
        {
            if (dataIn is not ChumpBeInteractedData data) return false;
            if (data.pushUnit is Player) return true;
            UnitTypeXZ type = data.owner.UnitTypeXZ;
            Direction direction = data.inputDirection;
            switch (type)
            {
                case UnitTypeXZ.Horizontal:
                    return direction is Direction.Back or Direction.Forward;
                case UnitTypeXZ.Vertical:
                    return direction is Direction.Left or Direction.Right;
                case UnitTypeXZ.Both:                  
                case UnitTypeXZ.None:
                    return true;
                default:
                    return false;
            }
        }
    }
}

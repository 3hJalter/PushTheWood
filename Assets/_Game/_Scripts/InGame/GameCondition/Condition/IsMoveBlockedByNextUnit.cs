using _Game._Scripts.InGame.GameCondition.Data;
using _Game.DesignPattern.ConditionRule;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.StaticUnit;
using UnityEngine;

using Tree = _Game.GameGrid.Unit.StaticUnit.Tree;

namespace _Game._Scripts.InGame.GameCondition.Condition
{
    [CreateAssetMenu(fileName = "IsMoveBlockedByNextUnit", menuName = "ConditionSO/Moving/IsMoveBlockedByNextUnit",
        order = 1)]
    public class IsMoveBlockedByNextUnit : ScriptableObject, ICondition
    {
        public bool IsApplicable(ConditionData data)
        {

            #region verify data

            if (data is not MovingData movingData) return false;

            #endregion

            return movingData.blockDynamicUnits.Count switch
            {
                0 when movingData.blockStaticUnits.Count == 0 => true,
                0 when movingData.blockStaticUnits.Count == 1 && movingData.blockStaticUnits[0] is TreeRoot =>
                    movingData.owner is IJumpTreeRootUnit jumpTreeRootUnit && jumpTreeRootUnit.CanJumpOnTreeRoot(movingData.inputDirection),               
                0 when movingData.blockStaticUnits.Count == 1 && movingData.blockStaticUnits[0] is Tree => true,
                _ => false,
            };
            // Temporary for handle TreeRoot
            // for (int i = 0; i < movingData.blockDynamicUnits.Count; i++)
            // {
            //     GridUnit unit = movingData.blockDynamicUnits[i];
            //     if (!unit.CanMoveFromUnit(movingData.inputDirection, movingData.owner)) return false;
            // }
            // Temporary for handle Tree Root

        }
    }
}

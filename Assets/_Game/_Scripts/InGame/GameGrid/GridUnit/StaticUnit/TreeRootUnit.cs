using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class TreeRootUnit : GridUnitStatic
    {
        public readonly Vector3 offsetY = new(0, 1.65f, 0);

        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            // SPAGHETTI CODE, change later
            // return if not dynamic unit
            if (interactUnit is not GridUnitDynamic dynamicUnit) return;
            dynamicUnit.OnInteractWithTreeRoot(direction, this);
        }

        public GridUnit GetAboveUnit()
        {
            return mainCell.GetGridUnitAtHeight(endHeight + 1);
        }
    }
}

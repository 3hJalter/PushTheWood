using _Game.GameGrid.GridUnit.DynamicUnit;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class TreeRootUnit : GridUnitStatic
    {
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            // SPAGHETTI CODE, change later
            if (interactUnit is not IInteractRootTreeUnit interactRootTreeUnit) return;
            interactRootTreeUnit.OnInteractWithTreeRoot(direction, this);
        }

        public GridUnit GetAboveUnit()
        {
            return mainCell.GetGridUnitAtHeight(endHeight + 1);
        }
    }
}

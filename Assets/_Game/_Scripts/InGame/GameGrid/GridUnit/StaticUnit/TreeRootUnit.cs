using _Game.GameGrid.Unit.DynamicUnit;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class TreeRootUnit : GridUnitStatic
    {
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            if (interactUnit is not IInteractRootTreeUnit interactRootTreeUnit) return;
            interactRootTreeUnit.OnInteractWithTreeRoot(direction, this);
        }

        public new GridUnit GetAboveUnit()
        {
            return mainCell.GetGridUnitAtHeight(endHeight + 1);
        }
    }
}

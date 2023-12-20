namespace _Game.GameGrid.Unit.StaticUnit
{
    public class TreeRoot : GridUnitStatic
    {
        // public bool CanMoveFromUnit(Direction direction, GridUnit pushUnit = null)
        // {
        //     if (pushUnit is null) return false;
        //     if ((direction is Direction.Forward or Direction.Back &&
        //          pushUnit.UnitTypeXZ is UnitTypeXZ.None or UnitTypeXZ.Vertical) ||
        //         (direction is Direction.Left or Direction.Right &&
        //          pushUnit.UnitTypeXZ is UnitTypeXZ.None or UnitTypeXZ.Horizontal))
        //         return true;
        //     return false;
        //
        // }

        public void UpHeight(GridUnit pushUnit, Direction direction = Direction.None)
        {
            // if (pushUnit.MainCell.GetGridUnitAtHeight(endHeight + 1) is not null) return;
            // Jump to treeRootUnit, Add one height 
            pushUnit.StartHeight += 1;
            pushUnit.EndHeight += 1;
        }
    }
}

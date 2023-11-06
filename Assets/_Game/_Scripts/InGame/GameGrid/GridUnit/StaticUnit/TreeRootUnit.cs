﻿using _Game.GameGrid.GridUnit.DynamicUnit;

namespace _Game.GameGrid.GridUnit.StaticUnit
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

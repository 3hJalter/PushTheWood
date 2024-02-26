using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.StaticUnit.Chest;
using DG.Tweening;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ChestFinalPoint : BChest
    {
        public override void OnOpenChestComplete()
        {
            base.OnOpenChestComplete();
            LevelManager.Ins.OnWin();
        }
    }
}

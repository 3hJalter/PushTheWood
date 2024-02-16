using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.StaticUnit.Chest;
using DG.Tweening;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class ChestFinalPoint : BChest
    {
        // public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        // {
        //     if (isInteracted) return;
        //     if (pushUnit is not Player) return;
        //     isInteracted = true;
        //     base.OnBePushed(direction, pushUnit);
        //     ShowAnim(true);
        //     DOVirtual.DelayedCall(1f, OnOpenChestComplete);
        // }

        public override void OnOpenChestComplete()
        {
            base.OnOpenChestComplete();
            LevelManager.Ins.OnWin();
        }
    }
}

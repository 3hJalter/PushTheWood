using _Game.GameGrid.Unit.DynamicUnit;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class FinalPointUnit : GridUnitStatic
    {
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            if (interactUnit is PlayerUnit)
            {
               LevelManager.Ins.OnWin();
            }
        }
    }
}

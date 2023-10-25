using _Game.GameGrid.GridUnit.DynamicUnit;

namespace _Game.GameGrid.GridUnit.StaticUnit
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

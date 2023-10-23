using _Game.GameGrid.GridUnit.DynamicUnit;

namespace _Game.GameGrid.GridUnit.StaticUnit.Interface
{
    public interface IPlayerInteracted
    {
        public void OnPlayerInteracted(Direction direction, PlayerUnit player);
    }
}

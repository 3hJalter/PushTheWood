using _Game.GameGrid.Unit;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public interface IChumpUnit
    {
        void OnPushChump(Direction direction);
        void OnPushChumpUp(Direction direction);
		void OnPushChumpDown(Direction direction);
		void OnGetNextStateAndType(Direction direction);
    }
}

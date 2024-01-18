using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Box.BoxState;

namespace _Game.GameGrid.Unit.DynamicUnit.Box
{
    public class ExplosiveBox : Box
    {
        protected override void AddState()
        {
            base.AddState();
            StateMachine.AddState(StateEnum.Explode, new ExplodeBoxState());
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            base.OnBePushed(direction, pushUnit);
        }
    }
}

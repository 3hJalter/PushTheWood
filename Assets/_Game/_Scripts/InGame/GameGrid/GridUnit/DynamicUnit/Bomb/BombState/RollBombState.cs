using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Bomb;

namespace a
{
    public class RollBombState : IState<Bomb>
    {
        public StateEnum Id => StateEnum.Roll;
        public void OnEnter(Bomb t)
        {
            throw new System.NotImplementedException();
        }

        public void OnExecute(Bomb t)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit(Bomb t)
        {
            throw new System.NotImplementedException();
        }
    }
}

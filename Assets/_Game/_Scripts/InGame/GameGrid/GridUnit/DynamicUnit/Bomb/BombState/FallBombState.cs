using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Bomb.BombState
{
    public class FallBombState : IState<Bomb>
    {
        public StateEnum Id => StateEnum.Fall;
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

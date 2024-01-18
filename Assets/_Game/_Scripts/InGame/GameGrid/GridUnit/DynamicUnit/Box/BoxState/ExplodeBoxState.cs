using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Box.BoxState
{
    public class ExplodeBoxState : IState<Box>
    {
        public StateEnum Id => StateEnum.Explode;
        public void OnEnter(Box t)
        {
            throw new System.NotImplementedException();
        }

        public void OnExecute(Box t)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit(Box t)
        {
            throw new System.NotImplementedException();
        }
    }
}

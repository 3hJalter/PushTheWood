using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.PlayerState
{
    public class InteractState : IState<NPlayerUnit>
    {
        public void OnEnter(NPlayerUnit t)
        {
            t.ChangeAnim(Constants.INTERACT_ANIM);
        }

        public void OnExecute(NPlayerUnit t)
        {
            // Wait for animation done
            if (t.IsAnimDone()) return;
            t.OnInteract(t.RuleMovingData.blockUnits[^1]);
            t.ChangeState(StateEnum.Idle);
        }

        public void OnExit(NPlayerUnit t)
        {
           
        }
    }
}

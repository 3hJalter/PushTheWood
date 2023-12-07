using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.PlayerState
{
    public class CutTreeState : IState<NPlayerUnit>
    {
        private bool _isInteract;

        public void OnEnter(NPlayerUnit t)
        {
            _isInteract = false;
            t.ChangeAnim(Constants.CUT_TREE_ANIM);
        }

        public void OnExecute(NPlayerUnit t)
        {
            if (!t.IsAnimDone(0.5f)) return;
            if (!_isInteract)
            {
                t.OnInteract(t.RuleMovingData.blockUnits[^1]);
                _isInteract = true;
            }

            if (!t.IsAnimDone()) return;
            t.ChangeState(StateEnum.Idle);
        }

        public void OnExit(NPlayerUnit t)
        {
        }
    }
}

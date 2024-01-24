using _Game._Scripts.Managers;
using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.ButtonUnitState
{
    public class EnterButtonState : IState<ButtonUnit>
    {
        public StateEnum Id => StateEnum.Enter;
        public void OnEnter(ButtonUnit t)
        {
            t.ChangeAnim(Constants.ENTER_ANIM);
            // DUST Effect
            EventGlobalManager.Ins.OnButtonUnitEnter?.Dispatch(true);
        }

        public void OnExecute(ButtonUnit t)
        {
        }

        public void OnExit(ButtonUnit t)
        {
            t.ChangeAnim(Constants.EXIT_ANIM);
            // DUST Effect
            EventGlobalManager.Ins.OnButtonUnitEnter?.Dispatch(false);
        }
    }
}

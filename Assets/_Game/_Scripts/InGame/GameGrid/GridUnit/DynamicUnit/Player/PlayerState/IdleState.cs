using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using HControls;

namespace _Game.GameGrid.Unit.DynamicUnit.PlayerState
{
    public class IdleState : IState<NPlayerUnit>
    {
        public void OnEnter(NPlayerUnit t)
        {
            t.ChangeAnim(Constants.IDLE_ANIM);
        }

        public void OnExecute(NPlayerUnit t)
        {
            t.direction = HInputManager.GetDirectionInput();
            if (t.direction == Direction.None) return;
            if (t.vehicle is not null)
            {
                t.vehicle.OnMove(t.direction);
                return;
            }
            t.movingRuleEngine.ApplyRules(t.RuleMovingData);
            if (t.RuleMovingData.blockUnits.Count > 0)
            {
                OnHasBlockUnit(t);
                return;
            } OnHasNoBlockUnit(t);
        }

        public void OnExit(NPlayerUnit t)
        {
            
        }
        
        private static void OnHasBlockUnit(NPlayerUnit t)
        {
            ChangeStateByUnit(t.RuleMovingData.blockUnits.Count > 1
                ? t.RuleMovingData.blockUnits[^1]
                : t.RuleMovingData.blockUnits[0]);
            return;

            void ChangeStateByUnit(GridUnit unit)
            {
                switch (unit)
                {
                    // TODO: Add case InteractUnit (Cage, Door, ...)
                    case TreeUnit:
                        t.ChangeState(StateEnum.CutTree);
                        break;
                    default:
                        t.ChangeState(StateEnum.Push);
                        break;
                }
            }
        }
        
        private void OnHasNoBlockUnit(NPlayerUnit t)
        {
            // TODO: bool CanMove -> If can move, check if Falling, else return
            // TODO: bool IsFalling -> If falling, change state to JumpDown, else change state to Walk
        }
    }
}

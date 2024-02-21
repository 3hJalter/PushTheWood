using System;
using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class HappyPlayerState : IState<Player>
    {
        public StateEnum Id => StateEnum.Happy;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.HAPPY_ANIM);
        }

        public void OnExecute(Player t)
        {
            
        }

        public void OnExit(Player t)
        {
            
        }
    }
}

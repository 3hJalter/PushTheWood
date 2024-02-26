using System;
using _Game.DesignPattern.StateMachine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class HappyPlayerState : AbstractPlayerState
    {
        public override StateEnum Id => StateEnum.Happy;

        public override void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.HAPPY_ANIM);
        }

        public override void OnExecute(Player t)
        {
            
        }

        public override void OnExit(Player t)
        {
            
        }
    }
}

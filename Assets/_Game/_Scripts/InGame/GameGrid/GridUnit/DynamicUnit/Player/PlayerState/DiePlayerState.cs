using System;
using _Game.DesignPattern.StateMachine;
using _Game.Managers;
using _Game.Utilities.Timer;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class DiePlayerState : AbstractPlayerState
    {
        private const float DIE_TIME = 1.08f;
        public override StateEnum Id => StateEnum.Die;

        public override void OnEnter(Player t)
        {
            GameplayManager.Ins.IsCanUndo = false;
            GameplayManager.Ins.IsCanResetIsland = false;
            t.ChangeAnim(Constants.DIE_ANIM);
            TimerManager.Ins.WaitForTime(DIE_TIME, OnLoseGame);
            void OnLoseGame()
            {
                GameManager.Ins.PostEvent(DesignPattern.EventID.LoseGame);
            }
        }

        public override void OnExecute(Player t)
        {
            
        }

        public override void OnExit(Player t)
        {
            
        }
    }
}

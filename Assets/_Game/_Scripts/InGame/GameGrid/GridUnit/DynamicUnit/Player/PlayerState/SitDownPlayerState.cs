using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class SitDownPlayerState : IState<Player>
    {
        public StateEnum Id => StateEnum.SitDown;
        private STimer timer;
        private bool isSitDown = true;
        public void OnEnter(Player t)
        {
            if (timer == null)
            {
                timer = TimerManager.Inst.PopSTimer();
            }
            t.ChangeAnim(Constants.SIT_DOWN_ANIM);
            
        }

        public void OnExecute(Player t)
        {
            if (!isSitDown) return;
        }

        public void OnExit(Player t)
        {
            
        }
    }
}
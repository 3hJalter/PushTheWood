using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    using _Game.DesignPattern.StateMachine;
    using _Game.Utilities.Timer;

    public class SleepPlayerState : IState<Player>
    {
        public StateEnum Id => StateEnum.Sleep;
        private bool isSleeping;
        private STimer timer;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.SLEEP_ANIM);
            if(timer == null)
            {
                timer = TimerManager.Inst.PopSTimer();
            }
            isSleeping = true;
        }

        public void OnExecute(Player t)
        {
            if (!isSleeping) return;

            if (t.Direction != Direction.None)
            {
                t.ChangeAnim(Constants.SLEEP_UP_ANIM);
                isSleeping = false;
                timer.Start(Constants.SLEEP_UP_TIME, ChangeIdleState);
            }

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExit(Player t)
        {
            timer.Stop();
        }
    }
}
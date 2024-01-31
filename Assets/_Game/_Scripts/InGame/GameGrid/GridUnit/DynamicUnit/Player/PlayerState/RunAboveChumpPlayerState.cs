using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Timer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class RunAboveChumpPlayerState : IState<Player>
    {
        private const float ANIM_TIME = 2f;
        public StateEnum Id => StateEnum.RunAboveChump;
        STimer timer;
        Direction oldDirection;
        bool isRunAboveChump;

        public void OnEnter(Player t)
        {
            if (timer == null)
                timer = TimerManager.Inst.PopSTimer();
            t.ChangeAnim(Constants.RUN_ABOVE_CHUMP);           
            timer.Start(ANIM_TIME, ChangeIdleState);
            oldDirection = t.Direction;
            isRunAboveChump = true;

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExecute(Player t)
        {
            if (!isRunAboveChump) return;
            if (t.Direction != Direction.None && t.Direction != oldDirection)
            {
                isRunAboveChump = false;
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExit(Player t)
        {
            timer.Stop();
        }
    }
}
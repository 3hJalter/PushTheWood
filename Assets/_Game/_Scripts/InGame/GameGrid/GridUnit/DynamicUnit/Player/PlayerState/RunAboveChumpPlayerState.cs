using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Timer;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class RunAboveChumpPlayerState : IState<Player>
    {
        private const float ANIM_TIME = 2f;
        private readonly Vector3 UNIT_VECTOR = new Vector3(1, 0, 1);
        public StateEnum Id => StateEnum.RunAboveChump;
        STimer timer;
        Direction oldDirection;
        //bool isRunAboveChump;
        GridUnit chump;

        public void OnEnter(Player t)
        {
            if (timer == null)
                timer = TimerManager.Inst.PopSTimer();
            chump = t.MainCell.GetGridUnitAtHeight(Constants.DirFirstHeightOfSurface[GameGridEnum.GridSurfaceType.Water] + 1);
            oldDirection = t.Direction;
            if (chump is Chump.Chump)
                chump.skin.DOLocalRotate(Vector3.Cross(Constants.DirVector3F[oldDirection], UNIT_VECTOR) * 720f, ANIM_TIME, RotateMode.LocalAxisAdd); 
            else 
                ChangeIdleState();

            t.ChangeAnim(Constants.RUN_ABOVE_CHUMP);           
            timer?.Start(ANIM_TIME, ChangeIdleState);
            //isRunAboveChump = true;

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExecute(Player t)
        {
            //if (!isRunAboveChump) return;
            //if (t.Direction != Direction.None && t.Direction != oldDirection)
            //{
            //    isRunAboveChump = false;
            //    t.StateMachine.ChangeState(StateEnum.Idle);
            //}
        }

        public void OnExit(Player t)
        {
            timer?.Stop();
            chump?.skin.DOKill();
        }
    }
}
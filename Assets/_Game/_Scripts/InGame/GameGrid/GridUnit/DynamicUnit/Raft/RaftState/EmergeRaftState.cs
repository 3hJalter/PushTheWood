using _Game.DesignPattern.StateMachine;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Raft.RaftState
{
    public class EmergeRaftState : IState<Raft>
    {
        Tween moveTween;
        public StateEnum Id => StateEnum.Emerge;

        public void OnEnter(Raft t)
        {
            //DEV: Refactor
            t.Tf.position = new Vector3(t.Tf.position.x, Constants.POS_Y_BOTTOM, t.Tf.position.z);
            moveTween = t.Tf.DOMoveY((float)Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] / 2 * Constants.CELL_SIZE - t.yOffsetOnDown, Constants.MOVING_TIME * 1.5f).OnComplete(() =>
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            });

        }

        public void OnExecute(Raft t)
        {

        }

        public void OnExit(Raft t)
        {
            moveTween.Kill();
        }
    }
}
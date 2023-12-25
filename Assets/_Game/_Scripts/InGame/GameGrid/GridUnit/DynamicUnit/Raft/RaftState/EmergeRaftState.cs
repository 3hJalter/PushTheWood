using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using GameGridEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Raft.RaftState
{
    public class EmergeRaftState : IState<Raft>
    {
        public void OnEnter(Raft t)
        {
            //DEV: Refactor
            t.Tf.position.Set(t.Tf.position.x, Constants.POS_Y_BOTTOM, t.Tf.position.z);
            t.Tf.DOMoveY((float)Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] / 2 * Constants.CELL_SIZE, Constants.MOVING_TIME * 2).OnComplete(() =>
            {
                t.ChangeState(StateEnum.Idle);
            });

        }

        public void OnExecute(Raft t)
        {

        }

        public void OnExit(Raft t)
        {

        }
    }
}
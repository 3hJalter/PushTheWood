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
        public void OnEnter(Raft t)
        {
            //DEV: Refactor
            DevLog.Log(DevId.Hung, "STATE: Emerge Raft");
            t.Tf.position = new Vector3(t.Tf.position.x, Constants.POS_Y_BOTTOM, t.Tf.position.z);
            t.Tf.DOMoveY((float)Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] / 2 * Constants.CELL_SIZE - t.yOffsetOnDown, Constants.MOVING_TIME * 2).OnComplete(() =>
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
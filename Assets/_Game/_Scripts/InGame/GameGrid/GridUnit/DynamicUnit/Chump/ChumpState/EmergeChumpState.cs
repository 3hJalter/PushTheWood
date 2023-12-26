using _Game.DesignPattern.StateMachine;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class EmergeChumpState : IState<Chump>
    {
        public StateEnum Id => StateEnum.Emerge;

        public void OnEnter(Chump t)
        {
            //DEV: Refactor 
            t.Tf.position = new Vector3(t.Tf.position.x, Constants.POS_Y_BOTTOM, t.Tf.position.z);
            t.Tf.DOMoveY((float)Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] / 2 * Constants.CELL_SIZE - t.yOffsetOnDown, Constants.MOVING_TIME * 1.5f).OnComplete(() =>
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            });
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {
            
        }

    }
}
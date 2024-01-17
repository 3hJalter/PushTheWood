using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class EmergeChumpState : IState<Chump>
    {
        public StateEnum Id => StateEnum.Emerge;
        Tween moveTween;
        public void OnEnter(Chump t)
        {
            //DEV: Refactor 
            Vector3 position = t.Tf.position;
            position = new Vector3(position.x, Constants.POS_Y_BOTTOM, position.z);
            t.Tf.position = position;
            moveTween = t.Tf.DOMoveY((float)(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] + t.FloatingHeightOffset) / 2 * Constants.CELL_SIZE - t.yOffsetOnDown, Constants.MOVING_TIME * 1.5f).OnComplete(() =>
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            });
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {
            moveTween.Kill();
        }

    }
}
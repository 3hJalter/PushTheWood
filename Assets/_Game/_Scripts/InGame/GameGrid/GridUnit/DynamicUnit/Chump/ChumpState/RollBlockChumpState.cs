using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class RollBlockChumpState : IState<Chump>
    {
        Vector3 axis;
        float lastAngle = 0;
        public void OnEnter(Chump t)
        {         

            switch (t.UnitTypeY)
            {
                case UnitTypeY.Up:
                    //NOTE: Blocking when chump is up 
                    axis = Vector3.Cross(Vector3.up, Constants.DirVector3[t.TurnOverData.inputDirection]);
                    lastAngle = 0;
                    DOVirtual.Float(0, 45, Constants.MOVING_TIME * 0.45f, i =>
                    {
                        t.skin.RotateAround(t.anchor.Tf.position, axis, i - lastAngle);
                        lastAngle = i;
                    }).SetUpdate(UpdateType.Fixed)
                    .SetEase(Ease.Linear)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() => t.ChangeState(StateEnum.Idle));          
                    break;
                case UnitTypeY.Down:
                    //NOTE: Blocking when chump is down
                    Vector3 originPos = t.Tf.position;
                    t.Tf.DOMove(originPos + Constants.DirVector3[t.MovingData.inputDirection] * Constants.CELL_SIZE / 2, Constants.MOVING_TIME * 0.45f)
                        .SetEase(Ease.Linear)
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => t.ChangeState(StateEnum.Idle));
                    break;
            }
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {
            
        }

    }
}
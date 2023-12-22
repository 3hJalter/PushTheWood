using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Timer;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class RollBlockChumpState : IState<Chump>
    {
        private int DEGREE = 60;
        Vector3 axis;
        float lastAngle = 0;
        GridUnitStatic blockObject;
        Direction blockDirection;

        public void OnEnter(Chump t)
        {

            switch (t.UnitTypeY)
            {
                case UnitTypeY.Up:
                    //NOTE: Blocking when chump is up
                    blockObject = t.TurnOverData.blockStaticUnits[0];
                    blockDirection = t.TurnOverData.inputDirection;
                    axis = Vector3.Cross(Vector3.up, Constants.DirVector3[blockDirection]);
                    lastAngle = 0;
                    DEGREE = 90 - (blockObject.Size.y + 1) * 15;

                    DOVirtual.Float(0, DEGREE * 2, Constants.MOVING_TIME * 1f, i =>
                    {
                        i = i <= DEGREE ? i : 2 * DEGREE - i;
                        t.skin.RotateAround(t.anchor.Tf.position, axis, i - lastAngle);
                        lastAngle = i;
                    }).SetUpdate(UpdateType.Fixed)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => t.ChangeState(StateEnum.Idle));

                    
                    if(blockObject != null)
                        TimerManager.Inst.WaitForTime(Constants.MOVING_TIME * 0.5f, ObjectBlocking);
                    break;
                case UnitTypeY.Down:
                    //NOTE: Blocking when chump is down
                    if (!IsSamePushDirection())
                    {
                        blockObject = t.MovingData.blockStaticUnits[0];
                        blockDirection = t.MovingData.inputDirection;

                        Vector3 originPos = t.Tf.position;

                        t.Tf.DOMove(originPos + Constants.DirVector3F[blockDirection] * Constants.CELL_SIZE / 2f, Constants.MOVING_TIME * 0.5f).SetEase(Ease.InQuad)
                            .OnComplete(() => t.Tf.DOMove(originPos, Constants.MOVING_TIME * 0.5f).SetEase(Ease.OutQuad).OnComplete(ChangeToIdle));
                    }
                    else
                    {
                        blockObject = t.TurnOverData.blockStaticUnits[0];
                        blockDirection = t.TurnOverData.inputDirection;

                        Vector3 originPos = t.Tf.position;
                        t.Tf.DOMove(originPos + Constants.DirVector3F[blockDirection] * Constants.CELL_SIZE / 4f, Constants.MOVING_TIME * 0.4f).SetEase(Ease.InQuad)
                            .OnComplete(() => t.Tf.DOMove(originPos, Constants.MOVING_TIME * 0.4f).SetEase(Ease.OutQuad).OnComplete(ChangeToIdle));
                    }
                    if (blockObject != null)
                        TimerManager.Inst.WaitForTime(Constants.MOVING_TIME * 0.5f, ObjectBlocking);

                    break;
            }

            void ObjectBlocking()
            {
                blockObject.OnBlock(blockDirection);
            }

            bool IsSamePushDirection()
            {
                if((t.UnitTypeXZ == UnitTypeXZ.Horizontal && (t.MovingData.inputDirection == Direction.Left || t.MovingData.inputDirection == Direction.Right)) 
                    || (t.UnitTypeXZ == UnitTypeXZ.Vertical && (t.MovingData.inputDirection == Direction.Forward || t.MovingData.inputDirection == Direction.Back)))
                {
                    return true;
                }
                return false;
            }

            void ChangeToIdle()
            {
                t.ChangeState(StateEnum.Idle);
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
using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class RollChumpState : IState<Chump>
    {
        private bool _isRoll;

        public void OnEnter(Chump t)
        {
            t.MovingData.SetData(t.ChumpBeInteractedData.inputDirection);
            _isRoll = t.ConditionMergeOnBePushed.IsApplicable(t.MovingData);
            OnExecute(t);
        }

        public void OnExecute(Chump t)
        {
            if (!_isRoll)
            {
                if (t.MovingData.blockDynamicUnits.Count > 0) t.OnPush(t.MovingData.inputDirection, t.MovingData);
                t.ChangeState(StateEnum.Idle);
            }
            else
            {

                t.SetEnterCellData(t.MovingData.inputDirection, t.MovingData.enterMainCell, t.UnitTypeY, false,
                    t.MovingData.enterCells);
                t.OnOutCells();
                t.OnEnterCells(t.MovingData.enterMainCell, t.MovingData.enterCells);
                // Animation and complete
                // TODO: rotate in place animation for skin
                t.Tf.DOMove(t.EnterPosData.initialPos,
                        t.EnterPosData.isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                    .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                    {
                        if (t.EnterPosData.isFalling)
                        {
                            t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                                .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                                {
                                    // Handle Cell Data
                                    t.OnEnterTrigger(t);
                                    t.ChangeState(StateEnum.Idle);
                                });
                        }
                        else
                        {
                            t.OnEnterTrigger(t);
                            t.ChangeState(StateEnum.Idle);
                            if (t.gameObject.activeSelf)
                                t.OnBePushed(t.ChumpBeInteractedData.inputDirection, t.ChumpBeInteractedData.pushUnit);
                        }
                    });
            }
        }

        public void OnExit(Chump t)
        {

        }

        private Vector3 GetRotateAxis(GridUnit t, Direction direction)
        {
            Vector3 axis = t.skin.localRotation.eulerAngles;
            if (direction is Direction.Back or Direction.Forward) axis.x += 359;
            else if (direction is Direction.Left or Direction.Right) axis.z += 359;
            return axis;
        }
    }
}

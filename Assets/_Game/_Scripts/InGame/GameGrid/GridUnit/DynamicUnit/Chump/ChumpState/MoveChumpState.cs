using _Game.DesignPattern.StateMachine;
using _Game.Utilities;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class MoveChumpState : IState<Chump>
    {
        private bool _isMove;

        public StateEnum Id => StateEnum.Move;

        public void OnEnter(Chump t)
        {
            t.MovingData.SetData(t.ChumpBeInteractedData.inputDirection);
            _isMove = t.ConditionMergeOnBePushed.IsApplicable(t.MovingData);
            OnExecute(t);
        }

        public void OnExecute(Chump t)
        {
            if (!_isMove)
            {
                if (t.MovingData.blockDynamicUnits.Count > 0) t.OnPush(t.MovingData.inputDirection, t.MovingData);
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
            else
            {
                t.SetEnterCellData(t.MovingData.inputDirection, t.MovingData.enterMainCell, t.UnitTypeY, false,
                    t.MovingData.enterCells);
                t.OnOutCells();
                t.OnEnterCells(t.MovingData.enterMainCell, t.MovingData.enterCells);
                // Animation and complete
                t.Tf.DOMove(t.EnterPosData.initialPos,
                        t.EnterPosData.isFalling ? Constants.MOVING_TIME / 2 : Constants.MOVING_TIME)
                    .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                    {
                        if (t.EnterPosData.isFalling)
                        {
                            t.StateMachine.ChangeState(StateEnum.Fall);
                        }
                        else
                        {
                            t.OnEnterTrigger(t);
                            t.StateMachine.ChangeState(StateEnum.Idle);
                        }
                    });
            }
        }

        public void OnExit(Chump t)
        {
        }
    }
}

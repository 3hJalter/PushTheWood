using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class MovePlayerState : IState<Player>
    {
        private bool _isExecuted;
        private Direction direction;
        private float initAnimSpeed;
        private Tween moveTween;
        private float OriginMovingDistance;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.MOVE_ANIM);
            initAnimSpeed = t.AnimSpeed;
            direction = t.Direction;
        }

        public void OnExecute(Player t)
        {
            if (t.Direction is not Direction.None && t.Direction != direction)
            {
                t.SetAnimSpeed(initAnimSpeed / Constants.MOVING_TIME_FAST_RATE);
                moveTween.Kill();
                direction = t.Direction;


                moveTween = t.Tf.DOMove(t.EnterPosData.finalPos, CalculateMoveRemainingTime())
                    .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                    {
                        t.InputCache.Enqueue(direction);
                        t.LookDirection(t.Direction);
                        t.SetAnimSpeed(initAnimSpeed);

                        t.OnEnterTrigger(t);
                        t.ChangeState(StateEnum.Idle);
                    });
            }

            if (moveTween != null) return;
            OriginMovingDistance = (t.EnterPosData.finalPos - t.Tf.position).sqrMagnitude;
            moveTween = t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.OnEnterTrigger(t);
                    t.ChangeState(StateEnum.Idle);
                });


            float CalculateMoveRemainingTime()
            {
                float remainingDistance = (t.EnterPosData.finalPos - t.Tf.position).sqrMagnitude;
                return Constants.MOVING_TIME_FAST_RATE * Constants.MOVING_TIME *
                       Mathf.Sqrt(remainingDistance / OriginMovingDistance);
            }
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
            moveTween = null;
        }
    }
}

using _Game.DesignPattern.StateMachine;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class MovePlayerState : IState<Player>
    {
        private bool _isExecuted;
        private Direction direction;
        private Tween moveTween;
        private float animSpeed;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.MOVE_ANIM);
            direction = t.Direction;
            animSpeed = t.AnimSpeed;
        }

        public void OnExecute(Player t)
        {
            if (moveTween == null)
            {
                moveTween = t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                    .SetEase(Ease.Linear).SetUpdate(UpdateType.Normal).OnComplete(() =>
                    {
                        t.OnEnterTrigger(t);
                        t.ChangeState(StateEnum.Idle);
                    });
            }
            if (direction != t.Direction && t.Direction != Direction.None)
            {
                moveTween.Kill();
                t.SetAnimSpeed(animSpeed * Constants.MOVING_TIME / 0.1f);
                moveTween = t.Tf.DOMove(t.EnterPosData.finalPos, 0.1f)
                    .SetEase(Ease.Linear).SetUpdate(UpdateType.Normal).OnComplete(() =>
                    {
                        t.OnEnterTrigger(t);
                        t.SetAnimSpeed(animSpeed);
                        t.ChangeState(StateEnum.Move);
                    });
            }
        }

        public void OnExit(Player t)
        {
            moveTween.Kill();
            moveTween = null;
        }
    }
}

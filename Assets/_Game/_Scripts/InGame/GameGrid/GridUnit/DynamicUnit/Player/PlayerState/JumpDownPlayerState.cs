using _Game.DesignPattern.StateMachine;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class JumpDownPlayerState : IState<Player>
    {
        private bool _isExecuted;
        private bool _firstTime;
        private Direction direction;
        public StateEnum Id => StateEnum.JumpDown;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.MOVE_ANIM);
            direction = Direction.None;
            _firstTime = true;
        }

        public void OnExecute(Player t)
        {
            if (!_firstTime && t.InputDetection.InputAction == InputAction.ButtonDown)
            {
                direction = t.Direction;
            }
            if (_isExecuted) return;
            _isExecuted = true;
            _firstTime = false;
            t.Tf.DOMove(t.EnterPosData.initialPos, Constants.MOVING_TIME / 2)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                        .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                        {
                            t.InputCache.Enqueue(direction);
                            t.OnEnterTrigger(t);
                            t.StateMachine.ChangeState(StateEnum.Idle);
                        });
                });
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
        }
    }
}

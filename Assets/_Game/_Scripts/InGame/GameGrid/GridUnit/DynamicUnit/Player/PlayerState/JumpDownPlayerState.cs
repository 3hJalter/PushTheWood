using _Game.DesignPattern.StateMachine;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class JumpDownPlayerState : IState<Player>
    {
        private bool _isExecuted;

        public StateEnum Id => StateEnum.JumpDown;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.MOVE_ANIM);
        }

        public void OnExecute(Player t)
        {
            if (_isExecuted) return;
            _isExecuted = true;
            t.Tf.DOMove(t.EnterPosData.initialPos, Constants.MOVING_TIME / 2)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                        .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                        {
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

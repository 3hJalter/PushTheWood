using _Game.DesignPattern.StateMachine;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class JumpUpPlayerState : IState<Player>
    {
        private bool _isExecuted;

        public StateEnum Id => StateEnum.JumpUp;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.JUMP_UP_ANIM);
        }

        public void OnExecute(Player t)
        {
            if (_isExecuted) return;
            _isExecuted = true;
            t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.OnEnterTrigger(t);
                    t.StateMachine.ChangeState(StateEnum.Idle);
                });
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
        }
    }
}

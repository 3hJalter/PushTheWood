using _Game.DesignPattern.StateMachine;
using DG.Tweening;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class MovePlayerState : IState<Player>
    {
        private bool _isExecuted;

        public void OnEnter(Player t)
        {
            t.ChangeAnim(Constants.MOVE_ANIM);
        }

        public void OnExecute(Player t)
        {
            if (t.Direction is not Direction.None) t.LookDirection(t.Direction);
            if (_isExecuted) return;
            _isExecuted = true;
            t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.OnEnterTrigger(t);
                    t.ChangeState(StateEnum.Idle);
                });
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
        }
    }
}

using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class PushPlayerState : IState<Player>
    {
        private bool _isExecuted;
        public void OnEnter(Player t)
        {
            _counterTime = 0.5f;
            t.ChangeAnim(Constants.PUSH_ANIM);
        }

        private float _counterTime;
        
        public void OnExecute(Player t)
        {
            // BUG: The time checks Idle anim instead of Push anim
            if (_counterTime > 0)
            {   _counterTime -= Time.fixedDeltaTime;
                if (_isExecuted || t.Direction == t.MovingData.inputDirection || t.Direction == Direction.None) return;
                t.ChangeState(StateEnum.Idle);
                return;
            }
            if (!_isExecuted)
            {
                _isExecuted = true;
                // Push the block Unit
                t.OnPush(t.MovingData.inputDirection);
                DOVirtual.DelayedCall(0.5f, () => t.ChangeState(StateEnum.Idle));
            }
        }

        public void OnExit(Player t)
        {
           _isExecuted = false;
        }
    }
}

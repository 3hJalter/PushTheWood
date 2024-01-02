using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class PushPlayerState : IState<Player>
    {
        float originAnimSpeed;
        float _counterTime;
        private bool _isExecuted;
        public StateEnum Id => StateEnum.Push;

        public void OnEnter(Player t)
        {
            originAnimSpeed = t.AnimSpeed;
            t.ChangeAnim(Constants.PUSH_ANIM, true);
            t.SetAnimSpeed(originAnimSpeed * Constants.PUSH_ANIM_TIME / Constants.PUSH_TIME);
            _counterTime = Constants.PUSH_TIME;
            _isExecuted = false;
        }

        public void OnExecute(Player t)
        {
            // BUG: The time checks Idle anim instead of Push anim
            if (_counterTime > 0)
            {
                _counterTime -= Time.fixedDeltaTime;
                if (_isExecuted || t.Direction == t.MovingData.inputDirection || t.Direction == Direction.None) return;
                t.StateMachine.ChangeState(StateEnum.Idle);
                return;
            }

            if (!_isExecuted)
            {
                _isExecuted = true;
                // Push the block Unit
                t.OnPush(t.MovingData.inputDirection);
                ParticlePool.Play(PoolController.Ins.Particles[VFX.DUST], t.transform.position + t.skin.transform.forward * Constants.CELL_SIZE * 0.5f);
                DOVirtual.DelayedCall(Constants.PUSH_TIME, OnCompletePush);
                void OnCompletePush()
                {
                    t.StateMachine.ChangeState(StateEnum.Idle);
                }
            }
        }

        public void OnExit(Player t)
        {
            t.SetAnimSpeed(originAnimSpeed);
        }
    }
}

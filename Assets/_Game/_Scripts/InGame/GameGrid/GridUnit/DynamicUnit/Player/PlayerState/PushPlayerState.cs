﻿using _Game.DesignPattern.StateMachine;
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
            t.ChangeAnim(Constants.PUSH_ANIM);
            t.SetAnimSpeed(originAnimSpeed * Constants.PUSH_ANIM_TIME / Constants.PUSH_TIME);
            _counterTime = Constants.PUSH_TIME;
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
                DOVirtual.DelayedCall(Constants.PUSH_TIME, () => t.StateMachine.ChangeState(StateEnum.Idle));
            }
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
            t.SetAnimSpeed(originAnimSpeed);
        }
    }
}

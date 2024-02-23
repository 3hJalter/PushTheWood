using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    using _Game.DesignPattern;
    using _Game.DesignPattern.StateMachine;
    using _Game.Managers;
    using _Game.Utilities.Timer;
    using GameGridEnum;

    public class SleepPlayerState : IState<Player>
    {
        private const float SLEEP_UP_TIME = 0.6f;
        private const float SLEEP_SFX_INTERVAL = 8f;

        public StateEnum Id => StateEnum.Sleep;
        private bool isSleeping;
        private STimer timer;
        private STimer sfxTimer;
        private float initAnimSpeed;
        ParticleSystem sleepingParticle;

        public void OnEnter(Player t)
        {
            if (timer == null)
            {
                timer = TimerManager.Ins.PopSTimer();
                sfxTimer = TimerManager.Ins.PopSTimer();
            }
            t.ChangeAnim(Constants.SLEEP_ANIM);           
            isSleeping = true;
            initAnimSpeed = t.AnimSpeed;
            sleepingParticle = ParticlePool.Play(DataManager.Ins.VFXData.GetParticleSystem(VFXType.SleepingzZz), t.VFXPositions[0].position);
            AudioManager.Ins.PlaySfx(AudioEnum.SfxType.Sleep);
            sfxTimer.Start(SLEEP_SFX_INTERVAL, () => AudioManager.Ins.PlaySfx(AudioEnum.SfxType.Sleep), true);
        }

        public void OnExecute(Player t)
        {
            if (!isSleeping) return;

            if (t.Direction != Direction.None)
            {
                t.ChangeAnim(Constants.SLEEP_UP_ANIM);
                t.SetAnimSpeed(initAnimSpeed * Constants.SLEEP_UP_ANIM_TIME / SLEEP_UP_TIME);
                isSleeping = false;
                sleepingParticle?.Stop();
                timer.Start(SLEEP_UP_TIME, ChangeIdleState);
            }

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExit(Player t)
        {
            timer.Stop();
            sfxTimer.Stop();
            sleepingParticle?.Stop();
            t.SetAnimSpeed(initAnimSpeed);
        }
    }
}
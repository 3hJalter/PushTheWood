using _Game.Data;
using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.Managers;
using _Game.Utilities.Timer;
using AudioEnum;
using DG.Tweening;
using GameGridEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class SitDownPlayerState : AbstractPlayerState
    {
        public const float SIT_UP_TIME = 0.4f;
        public const float SIT_DOWN_TIME = 0.4f;
        public const float SIT_DISTANCE = 0.4f;
        public override StateEnum Id => StateEnum.SitDown;
        private STimer timer;
        private STimer whistlingTimer;
        private bool isWhistling;
        private bool isSitDown = true;
        Direction oldDirection;
        SfxType oldWhistling = SfxType.None;

        float initAnimSpeed;
        Vector3 sitDistance;
        Vector3 oldSkinPos;
        ParticleSystem musicalNotes;
        Player player;
        public override void OnEnter(Player t)
        {
            if (timer == null)
            {
                timer = TimerManager.Ins.PopSTimer();
                whistlingTimer = TimerManager.Ins.PopSTimer();
                isWhistling = false;
            }
            isWhistling = true;
            player = t;
            initAnimSpeed = t.AnimSpeed;
            t.ChangeAnim(Constants.SIT_DOWN_ANIM, true);
            t.SetAnimSpeed(initAnimSpeed * Constants.SIT_DOWN_ANIM_TIME / SIT_DOWN_TIME);
            oldDirection = t.Direction;
            isSitDown = false;
            sitDistance = Constants.DirVector3F[oldDirection] * SIT_DISTANCE;

            oldSkinPos = t.skin.localPosition;
            t.skin.DOLocalMove(oldSkinPos + sitDistance, SIT_DOWN_TIME).OnComplete(Singing);
            t.OnCharacterChangePosition();
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, OnStandUp);

            void Singing()
            {
                musicalNotes = ParticlePool.Play(DataManager.Ins.VFXData.GetParticleSystem(VFXType.MusicalNotes), t.VFXPositions[1].position);
                if(GameManager.Ins.IsState(GameState.InGame))
                    Whistling();
                t.SetAnimSpeed(initAnimSpeed);
                isSitDown = true;
            }
        }

        public override void OnExecute(Player t)
        {
            if (!isWhistling)
            {
                Whistling();
            }
            if (!isSitDown) return;
            if (t.InputDirection != Direction.None && t.InputDirection != oldDirection)
            {
                OnStandUp();
            }
        }

        public override void OnExit(Player t)
        {
            t.SetAnimSpeed(initAnimSpeed);
            t.skin.DOKill();
            timer?.Stop();
            whistlingTimer?.Stop();
            musicalNotes?.Stop();
            t.skin.transform.localPosition = oldSkinPos;
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, OnStandUp);
        }

        private void Whistling()
        {
            isWhistling = true;
            float time = AudioManager.Ins.PlaySfx(GetWhistlingAudio()).clip.length + Random.Range(1f, 3f);
            whistlingTimer.Start(time, () => isWhistling = false);
        }
        private void OnStandUp()
        {
            player.ChangeAnim(Constants.SIT_UP_ANIM);
            player.SetAnimSpeed(initAnimSpeed * Constants.SIT_UP_ANIM_TIME / SIT_UP_TIME);

            timer.Start(SIT_UP_TIME, ChangeIdleState);
            player.skin.transform.DOLocalMove(oldSkinPos, SIT_UP_TIME);
            musicalNotes?.Stop();
            isSitDown = false;

            void ChangeIdleState()
            {
                player.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        private SfxType GetWhistlingAudio()
        {
            int value = Random.Range(0, 3);
            switch (value)
            {
                case 0:
                    if (oldWhistling == SfxType.Whistling1)
                        oldWhistling = SfxType.Whistling2;
                    else
                        oldWhistling = SfxType.Whistling1;
                    return oldWhistling;
                case 1:
                    if (oldWhistling == SfxType.Whistling2)
                        oldWhistling = SfxType.Whistling3;
                    else
                        oldWhistling = SfxType.Whistling2;
                    return oldWhistling;
                case 2:
                    if (oldWhistling == SfxType.Whistling3)
                        oldWhistling = SfxType.Whistling1;
                    else
                        oldWhistling = SfxType.Whistling3;
                    return oldWhistling;
            }
            return SfxType.None;
        }
    }
}
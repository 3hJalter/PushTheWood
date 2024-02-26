using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.Managers;
using _Game.Utilities.Timer;
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
        private bool isSitDown = true;
        Direction oldDirection;

        float initAnimSpeed;
        Vector3 sitDistance;
        Vector3 oldSkinPos;
        ParticleSystem musicalNotes;
        public override void OnEnter(Player t)
        {
            if (timer == null)
            {
                timer = TimerManager.Ins.PopSTimer();
            }

            initAnimSpeed = t.AnimSpeed;
            t.ChangeAnim(Constants.SIT_DOWN_ANIM, true);
            t.SetAnimSpeed(initAnimSpeed * Constants.SIT_DOWN_ANIM_TIME / SIT_DOWN_TIME);
            oldDirection = t.Direction;
            isSitDown = false;
            sitDistance = Constants.DirVector3F[oldDirection] * SIT_DISTANCE;

            oldSkinPos = t.skin.transform.localPosition;
            t.skin.transform.DOLocalMove(oldSkinPos + sitDistance, SIT_DOWN_TIME).OnComplete(PlayVFXSinging);

            void PlayVFXSinging()
            {
                musicalNotes = ParticlePool.Play(DataManager.Ins.VFXData.GetParticleSystem(VFXType.MusicalNotes), t.VFXPositions[1].position);
                t.SetAnimSpeed(initAnimSpeed);
                isSitDown = true;
            }
        }

        public override void OnExecute(Player t)
        {
            if (!isSitDown) return;
            if (t.InputDirection != Direction.None && t.InputDirection != oldDirection)
            {
                t.ChangeAnim(Constants.SIT_UP_ANIM);
                t.SetAnimSpeed(initAnimSpeed * Constants.SIT_UP_ANIM_TIME / SIT_UP_TIME);

                SaveCommand(t);
                timer.Start(SIT_UP_TIME, ChangeIdleState);
                t.skin.transform.DOLocalMove(oldSkinPos, SIT_UP_TIME);
                musicalNotes?.Stop();
                isSitDown = false;
            }

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public override void OnExit(Player t)
        {
            t.SetAnimSpeed(initAnimSpeed);
            t.skin.DOKill();
            timer?.Stop();
            musicalNotes?.Stop();
            t.skin.transform.localPosition = oldSkinPos;
        }
    }
}
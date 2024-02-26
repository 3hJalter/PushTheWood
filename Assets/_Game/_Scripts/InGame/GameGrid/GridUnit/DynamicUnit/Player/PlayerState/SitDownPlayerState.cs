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
    public class SitDownPlayerState : IState<Player>
    {
        public const float SIT_UP_TIME = 0.4f;
        public const float SIT_DOWN_TIME = 0.4f;
        public const float SIT_DISTANCE = 0.4f;
        public StateEnum Id => StateEnum.SitDown;
        private STimer timer;
        private bool isSitDown = true;
        Direction oldDirection;
        Direction cacheDirection;

        float initAnimSpeed;
        Vector3 sitDistance;
        Vector3 oldSkinPos;
        ParticleSystem musicalNotes;
        public void OnEnter(Player t)
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

        public void OnExecute(Player t)
        {
            if (!isSitDown) return;
            if (t.Direction != Direction.None && t.Direction != oldDirection)
            {
                t.ChangeAnim(Constants.SIT_UP_ANIM);
                t.SetAnimSpeed(initAnimSpeed * Constants.SIT_UP_ANIM_TIME / SIT_UP_TIME);
                timer.Start(SIT_UP_TIME, ChangeIdleState);
                t.skin.transform.DOLocalMove(oldSkinPos, SIT_UP_TIME);
                cacheDirection = t.Direction;
                musicalNotes?.Stop();
                isSitDown = false;
            }

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
                t.InputCache.Enqueue(cacheDirection);
            }
        }

        public void OnExit(Player t)
        {
            t.SetAnimSpeed(initAnimSpeed);
            t.skin.DOKill();
            timer?.Stop();
            musicalNotes?.Stop();
            t.skin.transform.localPosition = oldSkinPos;
        }
    }
}
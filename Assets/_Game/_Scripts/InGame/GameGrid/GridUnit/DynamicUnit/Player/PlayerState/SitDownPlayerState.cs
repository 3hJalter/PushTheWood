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
        public const float SIT_UP_TIME = 0.7f;
        public const float SIT_DISTANCE = 0.4f;
        public StateEnum Id => StateEnum.SitDown;
        private STimer timer;
        private bool isSitDown = true;
        Direction oldDirection;
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
            t.ChangeAnim(Constants.SIT_DOWN_ANIM, true);
            oldDirection = t.Direction;
            initAnimSpeed = t.AnimSpeed;
            isSitDown = false;
            sitDistance = Constants.DirVector3F[oldDirection] * SIT_DISTANCE;

            oldSkinPos = t.skin.transform.localPosition;
            t.skin.transform.DOLocalMove(oldSkinPos + sitDistance, Constants.SIT_UP_ANIM_TIME).OnComplete(PlayVFXSinging);

            void PlayVFXSinging()
            {
                musicalNotes = ParticlePool.Play(DataManager.Ins.VFXData.GetParticleSystem(VFXType.MusicalNotes), t.VFXPositions[1].position);
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
                musicalNotes?.Stop();
                isSitDown = false;
            }

            void ChangeIdleState()
            {
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }

        public void OnExit(Player t)
        {
            t.SetAnimSpeed(initAnimSpeed);
            timer?.Stop();
            musicalNotes?.Stop();
            t.skin.transform.localPosition = oldSkinPos;
        }
    }
}
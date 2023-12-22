﻿using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Timer;
using DG.Tweening;
using UnityEngine;
namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class IdleChumpState : IState<Chump>
    {
        private float MOVE_Y_VALUE = 0.1f;
        private float MOVE_Y_TIME = 2f;

        Vector3 originTransform;
        public void OnEnter(Chump t)
        {
            #region ANIM
            //DEV: Refactor
            if (t.IsInWater())
            {
                originTransform = t.Tf.transform.position;
                DOVirtual.Float(0 ,MOVE_Y_TIME, MOVE_Y_TIME, i =>
                {
                    SetSinWavePosition(i);
                }).SetLoops(-1).SetEase(Ease.Linear); 
            }


            void SetSinWavePosition(float time)
            {
                float value = Mathf.Sin(2 * time * Mathf.PI / MOVE_Y_TIME) * MOVE_Y_VALUE;
                t.Tf.transform.position = originTransform + Vector3.up * value;
            }
            #endregion
        }

        public void OnExecute(Chump t)
        {

        }

        public void OnExit(Chump t)
        {

        }
    }
}

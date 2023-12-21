using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class RollBlockChumpState : IState<Chump>
    {
        public void OnEnter(Chump t)
        {           
            switch (t.UnitTypeY)
            {
                case UnitTypeY.Up:
                    //NOTE: Blocking when chump is up
                    Vector3 axis = Vector3.Cross(Vector3.up, Constants.DirVector3[t.TurnOverData.inputDirection]);
                    float lastAngle = 0;
                    DOVirtual.Float(0, 90, Constants.MOVING_TIME / 1.5f, i =>
                    {
                        //NOTE: Rotate Back
                        if(i >= 45)
                        {
                            i = 90 - i;
                        }
                        t.skin.RotateAround(t.anchor.Tf.position, axis, i - lastAngle);
                        lastAngle = i;
                    }).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear)
                    .OnComplete(() => t.ChangeState(StateEnum.Idle));          
                    break;
                case UnitTypeY.Down:
                    t.Tf.DOShakePosition(0.15f, 0.1f, 100, 50, false, true, ShakeRandomnessMode.Harmonic)
                        .OnComplete(() => t.ChangeState(StateEnum.Idle));
                    //NOTE: Blocking when chump is down
                    break;
            }
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {
          
        }

    }
}
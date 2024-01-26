using _Game.DesignPattern.StateMachine;
using _Game.Utilities;
using _Game.Utilities.Timer;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class IdleChumpState : IState<Chump>
    {
        private const float MOVE_Y_VALUE = 0.06f;
        private const float MOVE_Y_TIME = 2f;

        private Tween floatingTween;
        
        Vector3 originTransform;

        public StateEnum Id => StateEnum.Idle;

        public void OnEnter(Chump t)
        {
            #region ANIM
            //DEV: Refactor anim system
            if (t.IsInWater())
            {
                originTransform = new Vector3(t.Tf.position.x, 
                    (float)(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water] + t.FloatingHeightOffset) / 2 * Constants.CELL_SIZE - t.yOffsetOnDown, 
                    t.Tf.position.z);
                floatingTween = DOVirtual.Float(0 ,MOVE_Y_TIME, MOVE_Y_TIME, SetSinWavePosition).SetLoops(-1).SetEase(Ease.Linear);
            }
            else
            {
                if (floatingTween is null) return;
                floatingTween.Kill();
            }

            return;
            
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
            floatingTween.Kill();
        }
    }
}

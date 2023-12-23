using System.Collections.Generic;
using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class FallChumpState : IState<Chump>
    {
        public void OnEnter(Chump t)
        {
            Debug.Log("Fall");
            // if (t.belowUnits.Count != 0)
            // {
            //     t.ChangeState(StateEnum.Idle);
            //     return;
            // }
            GameGridCell mainCell = t.MainCell;
            List<GameGridCell> cellInUnits = new(t.cellInUnits); //DEV: Why new List here?
            t.SetEnterCellData(Direction.None, t.MainCell, t.UnitTypeY, false, t.cellInUnits);
            t.OnOutCells();
            t.OnEnterCells(mainCell, cellInUnits);

            //NOTE: Fall into water that do not have anything
            if (t.TurnOverData.enterMainCell.SurfaceType is GridSurfaceType.Water
                & t.TurnOverData.enterMainCell.GetGridUnitAtHeight(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water]) == t)
            {
                // Tween to final position
                t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME).SetEase(Ease.Linear)
                    .SetUpdate(UpdateType.Fixed).OnComplete(() =>
                    {
                        t.OnEnterTrigger(t);
                        // Can be change to animation later
                        t.skin.localRotation =
                            Quaternion.Euler(t.UnitTypeXZ is UnitTypeXZ.Horizontal
                                ? Constants.HorizontalSkinRotation
                                : Constants.VerticalSkinRotation);
                        // minus position offsetY
                        t.Tf.position -= Vector3.up * t.yOffsetOnDown;
                        t.ChangeState(StateEnum.Idle);
                    });
            }
            else
            {
                t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.OnEnterTrigger(t);
                    t.ChangeState(StateEnum.Idle);
                });
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

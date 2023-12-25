using System.Collections.Generic;
using _Game.DesignPattern.StateMachine;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class FallChumpState : IState<Chump>
    {
        public void OnEnter(Chump t)
        {
            DevLog.Log(DevId.Hung, "STATE: Fall");
            // We know that when OnOutCell, the mainCell and cellInUnits will be cleared
            // That why we need to save it to use for OnEnterCell because all the mainCell and cellInUnits still the same as before
            GameGridCell mainCell = t.MainCell;
            //DEV: Can be optimize
            List<GameGridCell> cellInUnits = new(t.cellInUnits);
            t.SetEnterCellData(Direction.None, t.MainCell, t.UnitTypeY, false, t.cellInUnits);
            t.OnOutCells();
            t.OnEnterCells(mainCell, cellInUnits);

            //NOTE: Fall into water that do not have anything
            GridUnit unitInCells = t.TurnOverData.enterMainCell.GetGridUnitAtHeight(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water]);
            if (t.IsNextCellSurfaceIs(GridSurfaceType.Water) && (unitInCells == t || unitInCells == null))
            {
                // Tween to final position
                DevLog.Log(DevId.Hung, "Fall into water not have anything");
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
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {

        }
    }
}

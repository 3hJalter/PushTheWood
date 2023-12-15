using System.Collections.Generic;
using _Game.DesignPattern.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class FallChumpState : IState<Chump>
    {
        public void OnEnter(Chump t)
        {
            Debug.Log("Fall");
            if (t.belowUnits.Count != 0)
            {
                t.ChangeState(StateEnum.Idle);
                return;
            }
            OnExecute(t);
        }

        public void OnExecute(Chump t)
        {
            GameGridCell mainCell = t.MainCell;
            List<GameGridCell> cellInUnits = new(t.cellInUnits);
            t.SetEnterCellData(Direction.None, t.MainCell, t.UnitTypeY, false, t.cellInUnits);
            t.OnOutCells();
            t.OnEnterCells(mainCell, cellInUnits);
            t.Tf.DOMove(t.EnterPosData.finalPos, Constants.MOVING_TIME)
                .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                {
                    t.OnEnterTrigger(t);
                    t.ChangeState(StateEnum.Idle);
                });
        }

        public void OnExit(Chump t)
        {

        }
    }
}

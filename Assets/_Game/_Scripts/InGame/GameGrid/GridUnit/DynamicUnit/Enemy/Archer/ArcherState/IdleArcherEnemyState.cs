using _Game._Scripts.InGame;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates
{
    public class IdleArcherEnemyState : IState<ArcherEnemy>
    {
        private const int MAX_RANGE = 20;
        List<GameGridCell> attackRange = new List<GameGridCell>();
        Direction attackDirection = Direction.None;
        public StateEnum Id => StateEnum.Idle;

        public void OnEnter(ArcherEnemy t)
        {
            attackDirection = t.SkinRotationDirection;
            GameGridCell cell = t.MainCell;
            attackRange.Add(cell);
            for (int i = 0; i < MAX_RANGE; i++)
            {
                cell = t.MainCell.GetNeighborCell(attackDirection);
                if (cell == null || cell.Data.gridSurfaceType == GameGridEnum.GridSurfaceType.Water) continue;
                attackRange.Add(cell);
            }
        }

        public void OnExecute(ArcherEnemy t)
        {
            
        }

        public void OnExit(ArcherEnemy t)
        {
            
        }
    }
}

using _Game._Scripts.InGame;
using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Grid;
using GameGridEnum;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates
{
    public class IdleArcherEnemyState : IState<ArcherEnemy>
    {
        private const int MAX_RANGE = 20;

        private bool _isChangeAnim;
        List<GameGridCell> attackRange = new List<GameGridCell>();
        List<DangerIndicator> dangerIndicators = new List<DangerIndicator>();
        Direction attackDirection = Direction.None;
        public StateEnum Id => StateEnum.Idle;

        public void OnEnter(ArcherEnemy t)
        {
            attackDirection = t.Direction;
            GameGridCell cell = t.MainCell;
            for (int i = 0; i < MAX_RANGE; i++)
            {
                cell = cell.GetNeighborCell(attackDirection);
                if (cell == null || cell.Data.gridSurfaceType == GridSurfaceType.Water) break;
                
                attackRange.Add(cell);
                dangerIndicators.Add(SimplePool.Spawn<DangerIndicator>(PoolType.DangerIndicator, cell.WorldPos + Vector3.up * 1.25f, Quaternion.identity));

            }
        }

        public void OnExecute(ArcherEnemy t)
        {
            if (t.Direction == Direction.None)
            {
                if (!_isChangeAnim)
                {
                    _isChangeAnim = true;
                    t.ChangeAnim(Constants.IDLE_ANIM);
                }
                return;
            }
        }

        public void OnExit(ArcherEnemy t)
        {
            attackRange.Clear();
            foreach(DangerIndicator cell in dangerIndicators)
            {
                SimplePool.Despawn(cell);
            }
        }
    }
}

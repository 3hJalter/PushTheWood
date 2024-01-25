using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.Managers;
using GameGridEnum;

using System.Collections.Generic;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates
{
    public class IdleMageEnemyState : IState<MageEnemy>
    {
        private const int MAX_RANGE = 2;

        private bool _isChangeAnim;
        List<GameGridCell> attackRange = new List<GameGridCell>();
        List<DangerIndicator> dangerIndicators = new List<DangerIndicator>();
        Direction attackDirection = Direction.None;
        private bool isAttack = false;
        private MageEnemy main;
        public StateEnum Id => StateEnum.Idle;

        public IdleMageEnemyState()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.ObjectInOutDangerCell, IsResetAttackRange);
        }
        public void OnEnter(MageEnemy t)
        {
            isAttack = false;
            attackDirection = t.SkinRotationDirection;
            GameGridCell cell = t.MainCell;
            main = t;
            for (int i = 0; i < 2; i++)
            {
                cell = cell.GetNeighborCell(attackDirection);
                CheckingAttackCell(cell);
                if (Constants.DirVector[attackDirection].x != 0)
                {
                    CheckingAttackCell(cell.GetNeighborCell(Direction.Forward));
                    CheckingAttackCell(cell.GetNeighborCell(Direction.Back));
                }
                else if (Constants.DirVector[attackDirection].y != 0)
                {
                    CheckingAttackCell(cell.GetNeighborCell(Direction.Left));
                    CheckingAttackCell(cell.GetNeighborCell(Direction.Right));
                }

            }

            void CheckingAttackCell(GameGridCell cell)
            {
                if (cell == null || cell.Data.gridSurfaceType == GridSurfaceType.Water) return;
                if (IsPreventAttack(cell))
                {
                    cell.Data.IsBlockDanger = true;
                }
                else
                {
                    cell.Data.IsBlockDanger = false;
                }
                cell.Data.IsDanger = true;
                isAttack = isAttack || IsHavePlayer(cell);
                attackRange.Add(cell);
                dangerIndicators.Add(SimplePool.Spawn<DangerIndicator>(PoolType.DangerIndicator, cell.WorldPos + Vector3.up * 1.25f, Quaternion.identity));
            }
            bool IsPreventAttack(GameGridCell cell)
            {
                for (int i = (int)t.StartHeight; i <= (int)t.EndHeight; i++)
                {
                    if (cell.Data.gridUnits[i] && cell.Data.gridUnits[i] is not Player.Player)
                    {
                        return true;
                    }
                }
                return false;
            }
            bool IsHavePlayer(GameGridCell cell)
            {
                for (int i = (int)t.StartHeight; i <= (int)t.EndHeight; i++)
                {
                    if (cell.Data.gridUnits[i] && cell.Data.gridUnits[i] is Player.Player player)
                    {
                        if (!player.IsDead)
                            return true;
                        return false;
                    }
                }
                return false;
            }
        }
        public void OnExecute(MageEnemy t)
        {
            if (t.IsDead)
            {
                t.StateMachine.ChangeState(StateEnum.Die);
                return;
            }
            if (isAttack)
            {
                t.StateMachine.ChangeState(StateEnum.Attack);
                isAttack = false;
                return;
            }

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
        public void OnExit(MageEnemy t)
        {
            foreach (GameGridCell cell in attackRange)
            {
                cell.Data.IsDanger = false;
                cell.Data.IsBlockDanger = false;
            }
            attackRange.Clear();
            foreach (DangerIndicator cell in dangerIndicators)
            {
                SimplePool.Despawn(cell);
            }
            dangerIndicators.Clear();
            _isChangeAnim = false;
        }
        private void IsResetAttackRange(object value)
        {
            if (!GameManager.Ins.IsState(GameState.InGame)) return;
            if (attackRange.Contains((GameGridCell)value))
                main.StateMachine.ChangeState(StateEnum.Idle);
        }
        ~IdleMageEnemyState()
        {
            if (GameManager.Ins)
                GameManager.Ins.UnregisterListenerEvent(EventID.ObjectInOutDangerCell, IsResetAttackRange);
        }
    }
}
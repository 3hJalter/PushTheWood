using _Game._Scripts.InGame;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates
{
    public class DieArcherEnemyState : IState<ArcherEnemy>
    {
        public StateEnum Id => StateEnum.Die;

        public void OnEnter(ArcherEnemy t)
        {

        }

        public void OnExecute(ArcherEnemy t)
        {

        }

        public void OnExit(ArcherEnemy t)
        {

        }
    }
}

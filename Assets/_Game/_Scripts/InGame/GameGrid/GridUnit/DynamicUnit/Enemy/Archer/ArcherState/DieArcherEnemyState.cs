using _Game._Scripts.InGame;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Timer;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Enemy.EnemyStates
{
    public class DieArcherEnemyState : IState<ArcherEnemy>
    {
        public StateEnum Id => StateEnum.Die;

        public void OnEnter(ArcherEnemy t)
        {
            t.ChangeAnim(Constants.DIE_ANIM);
        }

        public void OnExecute(ArcherEnemy t)
        {

        }

        public void OnExit(ArcherEnemy t)
        {

        }
    }
}

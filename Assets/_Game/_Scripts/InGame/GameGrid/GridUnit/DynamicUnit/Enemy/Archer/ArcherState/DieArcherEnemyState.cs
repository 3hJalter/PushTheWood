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
        private const float DIE_TIME = 1.08f;
        private STimer timer;
        public StateEnum Id => StateEnum.Die;

        public void OnEnter(ArcherEnemy t)
        {
            timer = TimerManager.Inst.PopSTimer();
            t.ChangeAnim(Constants.DIE_ANIM);
            t.OnOutCells();
            timer.Start(DIE_TIME, t.OnDespawn);
        }

        public void OnExecute(ArcherEnemy t)
        {

        }

        public void OnExit(ArcherEnemy t)
        {
            timer.Stop();
            TimerManager.Inst.PushSTimer(timer);
        }
    }
}

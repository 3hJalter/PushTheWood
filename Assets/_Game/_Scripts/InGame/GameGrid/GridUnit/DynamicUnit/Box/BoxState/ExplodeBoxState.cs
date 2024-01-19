using System.Collections.Generic;
using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.Managers;
using _Game.Utilities;
using GameGridEnum;

namespace _Game.GameGrid.Unit.DynamicUnit.Box.BoxState
{
    public class ExplodeBoxState : IState<Box>
    {
        public StateEnum Id => StateEnum.Explode;
        public void OnEnter(Box t)
        {
            DevLog.Log(DevId.Hoang, "ExplodeBoxState OnEnter");
            // Cast to ExplosiveBox
            ParticlePool.Play(DataManager.Ins.VFXData.GetParticleSystem(VFXType.BombExplosion),
                t.Tf.position);
            Explode(t);
            
        }

        public void OnExecute(Box t)
        {
            
        }

        public void OnExit(Box t)
        {
            
        }

        private static void Explode(GridUnit t)
        {
            // Merge all neighbor, below, upper units
            List<GridUnit> units = new();
            units.AddRange(t.neighborUnits);
            units.AddRange(t.belowUnits);
            units.AddRange(t.upperUnits);
            units.Add(t);
            // Despawn all units, include this unit
            // TODO: Handle with Player and Enemy -> Die State instead of Despawn
            units.ForEach(unit => unit.OnDespawn());
        }
    }
}

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
            // TODO: BOMB LOGIC
            t.OnDespawn();
        }

        public void OnExecute(Box t)
        {
            
        }

        public void OnExit(Box t)
        {
            
        }
    }
}

using System.Collections.Generic;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Box.BoxState;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Box
{
    public class ExplosiveBox : Box
    {
        [Title("Explosive Box")]
        [SerializeField] private float waitTimeToExplode = 5f;
        [SerializeField] private GameObject waitExplosionObjectEffect;

        [SerializeField] private bool _isWaitForExplode;
        
        private const string EXPLODE_TAG = "Explosion";
        
        protected override void AddState()
        {
            base.AddState();
            StateMachine.AddState(StateEnum.Explode, new ExplodeBoxState());
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            base.OnBePushed(direction, pushUnit);
            if (_isWaitForExplode) return;
            if (pushUnit is not Player.Player) return;
            // LevelManager.Ins.SaveGameState(true);
            // player.MainCell.ValueChange();
            // MainCell.ValueChange();
            // LevelManager.Ins.SaveGameState(false);
            _isWaitForExplode = true;
            Timing.RunCoroutine(WaitToExplode(), EXPLODE_TAG);
        }

        public override void OnDespawn()
        {
            _isWaitForExplode = false;
            StateMachine.OverrideState = StateEnum.None;
            waitExplosionObjectEffect.SetActive(false);
            base.OnDespawn();
        }
        
        private IEnumerator<float> WaitToExplode()
        {
            // Wait each 0.5s, to set the waitExplosionObjectEffect active or not
            float waitTime = 0f;
            while (waitTime < waitTimeToExplode)
            {
                // if _isWaitForExplode is false, stop the coroutine
                waitTime += 0.5f;
                waitExplosionObjectEffect.SetActive(!waitExplosionObjectEffect.activeSelf);
                yield return Timing.WaitForSeconds(0.5f);
            }
            // if _isWaitForExplode is false, stop the coroutine
            // Change state to explode
            StateMachine.OverrideState = StateEnum.Explode;
            StateMachine.ChangeState(StateEnum.Explode);
        }
        
        private void StopExplode()
        {
            _isWaitForExplode = false;
            Timing.KillCoroutines(EXPLODE_TAG);
            waitExplosionObjectEffect.SetActive(false);
            StateMachine.OverrideState = StateEnum.None;
        }
        
        public override IMemento Save()
        {
            IMemento save;
            if (overrideSpawnSave != null)
            {
                save = overrideSpawnSave;
                overrideSpawnSave = null;
            }
            else
            {
                save = new ExplosiveBoxMemento(this, CurrentStateId, isSpawn, Tf.position, skin.rotation, startHeight, endHeight
                    , unitTypeY, unitTypeXZ, belowUnits, neighborUnits, upperUnits, mainCell, cellInUnits, islandID, lastPushedDirection);
            }
            return save;
        }

        private class ExplosiveBoxMemento : DynamicUnitMemento<ExplosiveBox>
        {
            public ExplosiveBoxMemento(GridUnitDynamic main, StateEnum currentState, params object[] data) : base(main, currentState, data)
            {
            }
            
            public override void Restore()
            {
                base.Restore();
                if (main._isWaitForExplode) main.StopExplode();
            }

            
        }
    }
}

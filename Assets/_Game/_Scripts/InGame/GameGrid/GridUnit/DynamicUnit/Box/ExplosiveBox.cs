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

        private bool _isWaitForExplode;
        
        protected override void AddState()
        {
            base.AddState();
            StateMachine.AddState(StateEnum.Explode, new ExplodeBoxState());
        }

        public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            base.OnBePushed(direction, pushUnit);
            if (_isWaitForExplode) return;
            if (pushUnit is not Player.Player player) return;
            LevelManager.Ins.SaveGameState(true);
            player.MainCell.ValueChange();
            MainCell.ValueChange();
            LevelManager.Ins.SaveGameState(false);
            _isWaitForExplode = true;
            Timing.RunCoroutine(WaitToExplode());
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
                waitTime += 0.5f;
                waitExplosionObjectEffect.SetActive(!waitExplosionObjectEffect.activeSelf);
                yield return Timing.WaitForSeconds(0.5f);
            }
            // Change state to explode
            StateMachine.OverrideState = StateEnum.Explode;
            StateMachine.ChangeState(StateEnum.Explode);
        }
    }
}

using _Game.GameGrid.Unit.DynamicUnit.Player;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class FinalPoint : GridUnitStatic
    {
        [SerializeField] protected Target indicatorTarget;
        [SerializeField] protected bool isInteracted;
        [SerializeField] protected ParticleSystem winParticle;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (!isInteracted) indicatorTarget.enabled = true;
        }
        
        public override void OnDespawn()
        {
            isInteracted = false;
            winParticle.Stop();
            base.OnDespawn();
        }
        
        private void OnReachPoint()
        {
            indicatorTarget.enabled = false;
            winParticle.Play();
            LevelManager.Ins.OnWin();
        }
        
        protected override void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {
            if (isInteracted) return;
            if (triggerUnit is not Player) return;
            isInteracted = true;
            OnReachPoint();
        }
    }
}

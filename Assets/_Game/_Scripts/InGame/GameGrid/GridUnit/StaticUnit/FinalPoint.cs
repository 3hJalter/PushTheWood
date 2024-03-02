using _Game._Scripts.Managers;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.Interface;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class FinalPoint : GridUnitStatic, IFinalPoint
    {
        [SerializeField] protected Target indicatorTarget;
        [SerializeField] protected bool isInteracted;
        [SerializeField] protected ParticleSystem winParticle;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (!isInteracted) indicatorTarget.enabled = true;
            OnAddToLevelManager();
        }
        
        public override void OnDespawn()
        {
            isInteracted = false;
            winParticle.Stop();
            OnRemoveFromLevelManager(false);
            base.OnDespawn();
        }
        
        private void OnReachPoint()
        {
            indicatorTarget.enabled = false;
            winParticle.Play();
            OnRemoveFromLevelManager();
        }
        
        protected override void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {
            if (isInteracted) return;
            if (triggerUnit is not Player) return;
            isInteracted = true;
            OnReachPoint();
        }

        private void OnAddToLevelManager()
        {
            LevelManager.Ins.OnAddFinalPoint(this);
        }
        
        private void OnRemoveFromLevelManager(bool checkWin = true)
        {
            LevelManager.Ins.OnRemoveFinalPoint(this, checkWin);
        }
    }
}

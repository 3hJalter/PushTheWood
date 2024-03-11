using _Game._Scripts.Managers;
using _Game.Data;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.Interface;
using _Game.Managers;
using _Game.Resource;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using VinhLB;

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
            OnGetResource(); // TEMPORARY
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

        private void OnGetResource()
        {
            #region Get Resource
            
            LevelManager.Ins.goldCount += 20;
            if (LevelManager.Ins.CurrentLevel.LevelType == LevelType.Normal)
            {
                int levelIndex = LevelManager.Ins.CurrentLevel.Index;

                if (levelIndex == 2)
                {
                    LevelManager.Ins.boosterRewardCount.Add(BoosterType.Undo, 2);
                    // LevelManager.Ins.boosterRewardCount.Add(BoosterType.ResetIsland, 2);
                } else if (levelIndex == 4)
                {
                    LevelManager.Ins.boosterRewardCount.Add(BoosterType.GrowTree, 1);
                    LevelManager.Ins.boosterRewardCount.Add(BoosterType.PushHint, 1);
                } else if (levelIndex == 5)
                {
                    CollectingResourceManager.Ins.SpawnCollectingRewardKey(3, LevelManager.Ins.player.transform);
                    LevelManager.Ins.KeyRewardCount += 3;
                }
                else if (levelIndex >= DataManager.Ins.ConfigData.unlockSecretLevelAtLevelIndex)
                {
                    int getCompass = levelIndex -
                                     DataManager.Ins.ConfigData.unlockSecretLevelAtLevelIndex;
                    if (getCompass > 0 && getCompass % 4 == 0)
                    {
                        CollectingResourceManager.Ins.SpawnCollectingRewardKey(1, LevelManager.Ins.player.transform);
                        LevelManager.Ins.SecretMapPieceCount += 1;
                    }
                }
            }
            #endregion
        }
    }
}

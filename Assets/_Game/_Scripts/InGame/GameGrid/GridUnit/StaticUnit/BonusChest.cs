using _Game._Scripts.Managers;
using _Game.Data;
using _Game.GameGrid.Unit.StaticUnit.Chest;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using VinhLB;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class BonusChest : BChest
    {
        private Vector3 originTransform;
        private Tween floatingTween;
        private const float MOVE_Y_VALUE = 0.06f;
        private const float MOVE_Y_TIME = 2f;
        
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            if (IsOnWater() || IsInWater()) SetFloatingTween();
            if (LevelManager.Ins.CurrentLevel.LevelType == LevelType.Secret)
            {
                OnAddToLevelManager();
            }
        }

        private void SetFloatingTween()
        {
            #region ANIM
            //DEV: Refactor anim system
            if (IsInWater())
            {
                originTransform = Tf.transform.position;
                floatingTween = DOVirtual.Float(0 ,MOVE_Y_TIME, MOVE_Y_TIME, SetSinWavePosition).SetLoops(-1).SetEase(Ease.Linear);
            }
            else
            {
                if (floatingTween is null) return;
                floatingTween.Kill();
            }

            return;
            
            void SetSinWavePosition(float time)
            {
                float value = Mathf.Sin(2 * time * Mathf.PI / MOVE_Y_TIME) * MOVE_Y_VALUE;
                Tf.transform.position = originTransform + Vector3.up * value;
            }
            #endregion
        }

        // public override void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        // {
        //     if (isInteracted) return;
        //     if (pushUnit is not Player) return;
        //     isInteracted = true;
        //     base.OnBePushed(direction, pushUnit);
        //     ShowAnim(true);
        //     DOVirtual.DelayedCall(Constants.CHEST_OPEN_TIME, OnOpenChestComplete);
        // }

        public override void OnDespawn()
        {
            floatingTween?.Kill();
            base.OnDespawn();
        }

        public override void OnOpenChestComplete()
        {
            base.OnOpenChestComplete();
            CollectingResourceManager.Ins.SpawnCollectingRewardKey(1, LevelManager.Ins.player.transform);
            LevelManager.Ins.ReceivingKeyReward = true;
            DevLog.Log(DevId.Hoang, "Loot something");
            if (LevelManager.Ins.CurrentLevel.LevelType == LevelType.Secret)
            {
                OnRemoveFromLevelManager();
            }
        }
        
        private static void OnAddToLevelManager()
        {
            LevelManager.Ins.numsOfCollectingObjectInLevel++;
            LevelManager.Ins.objectiveCounter++;
        }
        
        private static void OnRemoveFromLevelManager()
        {
            LevelManager.Ins.numsOfCollectingObjectInLevel--;
            EventGlobalManager.Ins.OnChangeLevelCollectingObjectNumber.Dispatch();
        }
    }
}

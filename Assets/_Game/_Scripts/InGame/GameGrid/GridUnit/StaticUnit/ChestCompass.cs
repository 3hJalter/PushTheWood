using _Game.GameGrid.Unit.StaticUnit.Chest;
using _Game.Utilities;
using UnityEngine;
using VinhLB;

namespace _Game.GameGrid.Unit.StaticUnit
{ 
    public class ChestCompass : BChest
    {
        private Vector3 originTransform;
        
        public override void OnOpenChestComplete()
        {
            base.OnOpenChestComplete();
            // TODO: Change to collect compass
            CollectingResourceManager.Ins.SpawnCollectingRewardKey(1, LevelManager.Ins.player.transform);
            LevelManager.Ins.SecretMapPieceCount += 1;
            // 
            DevLog.Log(DevId.Hoang, "Loot something");
        }
    }
}

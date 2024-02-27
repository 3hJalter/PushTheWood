using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Managers
{
    using _Game._Scripts.Utilities;
    using _Game.Data;
    using _Game.DesignPattern;
    using _Game.Resource;
    using System;
    using VinhLB;

    public class RewardManager : Singleton<RewardManager>
    {
        [SerializeField]
        GameData _gameData;
        [SerializeField]
        private List<Sprite> resourceIconList;
        [SerializeField]
        private List<Sprite> boosterIconList;
        public readonly Dictionary<ResourceType, Sprite> ResourceIcons;
        [SerializeField]
        public readonly Dictionary<BoosterType, Sprite> BoosterIcons;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        [ContextMenu("Convert From List To Dict")]
        public void ConvertListToDict()
        {
            for (int i = 0; i < resourceIconList.Count; i++)
            {
                if (ResourceIcons.ContainsKey((ResourceType)i))
                    ResourceIcons[((ResourceType)i)] = resourceIconList[i];
                else
                    ResourceIcons.Add((ResourceType)i, resourceIconList[i]);
            }

            for (int i = 0; i < boosterIconList.Count; i++)
            {
                if (BoosterIcons.ContainsKey((BoosterType)i))
                    BoosterIcons[((BoosterType)i)] = boosterIconList[i];
                else
                    BoosterIcons.Add((BoosterType)i, resourceIconList[i]);
            }
        }
    }

    [Serializable]
    public class HomeReward
    {
        GameData gameData;
        RewardManager rewardManager;
        public HomeReward(GameData gameData, RewardManager rewardManager)
        {
            this.gameData = gameData;
            this.rewardManager = rewardManager;
        }
        public bool IsCanClaimRC => gameData.user.currentRewardChestIndex < gameData.user.rewardChestUnlock;
        public bool IsCanClaimLC => gameData.user.currentLevelChestIndex < gameData.user.levelChestUnlock;
        public void ClaimRewardChest()
        {
            if (IsCanClaimRC)
            {
                gameData.user.currentRewardChestIndex += 1;
                GameManager.Ins.PostEvent(EventID.OnClaimRewardChest, gameData.user.currentRewardChestIndex - 1);
                UIManager.Ins.OpenUI<RewardPopup>().Open(GetRCReward());
            }
        }

        public void ClaimLevelChest(int index)
        {
            if (IsCanClaimLC)
            {
                gameData.user.currentRewardChestIndex += 1;
                GameManager.Ins.PostEvent(EventID.OnClaimRewardChest, gameData.user.currentLevelChestIndex - 1);
            }
        }

        private Reward[] GetRCReward()
        {
            Reward[] rewards = new Reward[] { new Reward() };
            int index = HUtilities.WheelRandom(DataManager.Ins.ConfigData.RCRates);
            ResourceType type = DataManager.Ins.ConfigData.RCRewards[index];
            rewards[0].Name = type.ToString();
            rewards[0].ResourceType = type;
            rewards[0].RewardType = RewardType.Resource;
            rewards[0].IconSprite = rewardManager.ResourceIcons[type];
            switch (type)
            {
                case ResourceType.Gold:
                    rewards[0].Amount = UnityEngine.Random.Range(100, 201);
                    break;
                case ResourceType.AdTicket:
                    rewards[0].Amount = DataManager.Ins.ConfigData.RCQuantitys[index];
                    break;
            }
            return rewards;
        }
    }
}
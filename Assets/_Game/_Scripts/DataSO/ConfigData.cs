using System;
using System.Collections.Generic;
using _Game.Managers;
using _Game.Resource;
using Sirenix.OdinInspector;
using UnityEngine;
using VinhLB;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "ScriptableObjects/ConfigData", order = 1)]
    public class ConfigData : SerializedScriptableObject
    {
        #region Feature Unlock
        [FoldoutGroup("Feature Unlock")]
        public readonly int unlockGoMainMenuOnLoseAtLevelIndex = 5;
        [FoldoutGroup("Feature Unlock")]
        public readonly int unlockBonusChestAtLevelIndex = 5;
        [FoldoutGroup("Feature Unlock")]
        public readonly int unlockDailyChallengeAtLevelIndex = 9;
        [FoldoutGroup("Feature Unlock")]
        public readonly int unlockSecretLevelAtLevelIndex = 19;
        #endregion

        #region In Game
        [FoldoutGroup("In Game")]
        // Time per level
        [FoldoutGroup("In Game/Time Per Level/Normal")]
        [InfoBox("Do not change the order of the list: Easy -> Medium -> Hard")]
        public List<TimePerNormalLevel> timePerNormalLevel = new()
        {
            new TimePerNormalLevel() { levelNormalType = LevelNormalType.Easy, time = 300 },
            new TimePerNormalLevel() { levelNormalType = LevelNormalType.Medium, time = 600 },
            new TimePerNormalLevel() { levelNormalType = LevelNormalType.Hard, time = 900 },
        };
        [FoldoutGroup("In Game/Time Per Level")]
        public readonly int timePerDailyChallengeLevel = 900;
        [FoldoutGroup("In Game/Time Per Level")]
        public readonly int timePerSecretLevel = 900;
        [FoldoutGroup("In Game/Reward Chest")]
        public readonly int requireRewardKey = 3;
        [FoldoutGroup("In Game/Reward Chest")]
        public readonly CurrencyType[] RCRewards;
        [FoldoutGroup("In Game/Reward Chest")]
        public readonly int[] RCQuantitys;
        [FoldoutGroup("In Game/Reward Chest")]
        public readonly float[] RCRates;

        [FoldoutGroup("In Game/Level Chest")]
        public readonly int requireLevelProgress = 3;
        [FoldoutGroup("In Game/Level Chest")]
        public readonly CurrencyType[] LCRewards;
        [FoldoutGroup("In Game/Level Chest")]
        public readonly int[] LCQuantitys;
        [FoldoutGroup("In Game/Level Chest")]
        public readonly float[] LCRates;
        [FoldoutGroup("In Game/Secret Level")]
        public readonly int requireSecretMapPiece = 8;
        [FoldoutGroup("In Game/Characters")]
        public readonly int[] CharacterCosts;
        [FoldoutGroup("In Game/Time")]
        public readonly int DangerTime = 30;
        [FoldoutGroup("In Game/MoreTimeAdded")]
        public readonly int MoreTimeAdded = 60;
        #endregion

        #region Daily Challenge Reward
        [FoldoutGroup("Daily Challenge Reward")]
        // Note: Add a theme resource when have, and make player take it the first time player clear full 7 days
        public List<DailyChallengeRewardMilestone> dailyChallengeRewardMilestones = new()
        {
            new DailyChallengeRewardMilestone()
            {
                clearLevelNeed = 1,
                rewards = new List<DailyChallengeReward>()
                {
                    new()
                    {
                        currencyType = CurrencyType.AdTicket,
                        quantity = 1
                    }
                }
            },
            new DailyChallengeRewardMilestone()
            {
                clearLevelNeed = 2,
                rewards = new List<DailyChallengeReward>()
                {
                    new()
                    {
                        currencyType = CurrencyType.Gold,
                        quantity = 60
                    }
                }
            },
            new DailyChallengeRewardMilestone()
            {
                clearLevelNeed = 4,
                rewards = new List<DailyChallengeReward>()
                {
                    new()
                    {
                        currencyType = CurrencyType.AdTicket,
                        quantity = 1
                    }
                }
            },
            new DailyChallengeRewardMilestone()
            {
                clearLevelNeed = 7,
                rewards = new List<DailyChallengeReward>()
                {
                    new()
                    {
                        currencyType = CurrencyType.Gold,
                        quantity = 120
                    },
                    new()
                    {
                        currencyType = CurrencyType.AdTicket,
                        quantity = 1
                    },
                    new()
                    {
                        currencyType = CurrencyType.RandomBooster,
                        quantity = 1,
                    }
                }
            },
        };
        #endregion

        #region Booster Purchase
        // Ticket purchase
        [FoldoutGroup("Booster Purchase")]
        [InfoBox("Do not change the order of the list, check order in BoosterType enum")]
        public List<BoosterConfig> boosterConfigList = new();
        #endregion

        #region Ads
        [FoldoutGroup("Ads")]
        public readonly int stepInterAdsCountMax = 3;
        [FoldoutGroup("Ads")]
        public readonly int interAdsCooldownTime = 45;
        [FoldoutGroup("Ads")]
        public readonly int startInterAdsLevel = 8;
        [FoldoutGroup("Ads")]
        public readonly int startBannerAds = 8;
        [FoldoutGroup("Ads")]
        public readonly int winLevelCountInterAds = 3;
        [FoldoutGroup("Ads")]
        public readonly int bannerHeight = 100;
        #endregion

        // [BoxGroup("Monetize")]

        public BoosterConfig GetBoosterConfig(BoosterType boosterType)
        {
            return boosterConfigList.Find(x => x.Type == boosterType);
        }
    }

    [Serializable]
    public struct DailyChallengeRewardMilestone
    {
        public int clearLevelNeed;
        public List<DailyChallengeReward> rewards;
    }

    [Serializable]
    public struct DailyChallengeReward
    {
        public CurrencyType currencyType;
        public int quantity;

        public void GetReward(object param)
        {
            switch (currencyType)
            {
                case CurrencyType.Gold:
                    GameManager.Ins.GainGold(quantity, param);
                    break;
                case CurrencyType.AdTicket:
                    GameManager.Ins.GainAdTickets(quantity, param);
                    break;
                case CurrencyType.RandomBooster:
                    break;
                case CurrencyType.None:
                case CurrencyType.SecretMapPiece:
                default:
                    break;
            }
        }
    }

    [Serializable]
    public struct BoosterConfig
    {
        [SerializeField]
        private BoosterType type;
        [SerializeField]
        private int unlockAtLevel;
        [SerializeField]
        private int goldPerBuyTen;
        [SerializeField]
        private TicketPerBuyRatio ticketPerBuyRatio;

        [HideInInspector]
        public UIResourceConfig UIResourceConfig;

        public BoosterType Type => type;
        public int UnlockAtLevel => unlockAtLevel;
        public string Name => UIResourceConfig.Name;
        public Sprite MainIcon => UIResourceConfig.MainIconSprite;
        public int GoldPerBuyTen => goldPerBuyTen;
        public TicketPerBuyRatio TicketPerBuyRatio => ticketPerBuyRatio;
        // Do with goldPerBuyMore
    }

    [Serializable]
    public record TimePerNormalLevel
    {
        public LevelNormalType levelNormalType;
        public int time;
    }

    [Serializable]
    public struct TicketPerBuyRatio
    {
        public TicketPerBuyRatio(int first, int second)
        {
            ticketNeed = first;
            itemsPerBuy = second;
        }

        public int ticketNeed;
        public int itemsPerBuy;
    }
}
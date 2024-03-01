﻿using System;
using System.Collections.Generic;
using _Game.Managers;
using _Game.Resource;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VinhLB;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "ScriptableObjects/ConfigData", order = 1)]
    public class ConfigData : SerializedScriptableObject
    {
        #region In Game

        [FoldoutGroup("In Game")]
        // Time per level
        [FoldoutGroup("In Game/Time Per Level/Normal")]
        [InfoBox("Do not change the order of the list: Easy -> Medium -> Hard")]
        public List<TimePerNormalLevel> timePerNormalLevel = new()
        {
            new TimePerNormalLevel() {levelNormalType = LevelNormalType.Easy, time = 300},
            new TimePerNormalLevel() {levelNormalType = LevelNormalType.Medium, time = 600},
            new TimePerNormalLevel() {levelNormalType = LevelNormalType.Hard, time = 900},
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
        public readonly int winLevelCountInterAds = 3;
        [FoldoutGroup("Ads/Booster")]
        public readonly int requireAdsForHintBooster = 2;
        public int[] boosterAmountReceiveFromAds; //Along With Booster Type value
        #endregion

        public int GetBoosterAmountFromAds(BoosterType boosterType) 
        {
            return boosterAmountReceiveFromAds[(int)boosterType];
        }
        // [BoxGroup("Monetize")]
    }

    [Serializable]
    public struct BoosterConfig
    {
        [SerializeField] private BoosterType type;
        [SerializeField] private int unlockAtLevel;
        [SerializeField] private int goldPerBuyTen;
        [SerializeField] private TicketPerBuyRatio ticketPerBuyRatio;

        [FormerlySerializedAs("UIResourceData")]
        [FormerlySerializedAs("ResourceData")]
        public UIResourceConfig UIResourceConfig;
        
        public BoosterType Type => type;
        public int UnlockAtLevel => unlockAtLevel;
        public string Name => UIResourceConfig.Name;
        public Sprite Icon => UIResourceConfig.IconSprite;
        public int GoldPerBuyTen => goldPerBuyTen;

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
        public TicketPerBuyRatio(int first, int second) {
            ticketNeed = first;
            itemsPerBuy = second;
        }
        
        public int ticketNeed;
        public int itemsPerBuy;
    }
}

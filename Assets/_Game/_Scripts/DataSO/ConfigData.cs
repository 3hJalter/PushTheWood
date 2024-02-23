using System;
using System.Collections.Generic;
using _Game.Managers;
using _Game.Resource;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "ScriptableObjects/ConfigData", order = 1)]
    public class ConfigData : SerializedScriptableObject
    {
        #region In Game

        [FoldoutGroup("In Game")]
        // Time per level
        [FoldoutGroup("In Game/Time Per Level")]
        public readonly int timePerNormalLevel = 600;
        [FoldoutGroup("In Game/Time Per Level")]
        public readonly int timePerDailyChallengeLevel = 900;
        [FoldoutGroup("In Game/Time Per Level")]
        public readonly int timePerSecretLevel = 900;

        #endregion
        
        #region Booster Purchase

        // Ticket purchase
        [FoldoutGroup("Booster Purchase")]
        public readonly Dictionary<BoosterType, BoosterConfig> boosterConfigs = new();

        #endregion

        #region Ads
        [FoldoutGroup("Ads")]
        public readonly int stepInterAdsCountMax = 3;
        public readonly int interAdsCooldownTime = 45;
        #endregion
        // [BoxGroup("Monetize")]
    }

    [Serializable]
    public struct BoosterConfig
    {
        [SerializeField] private BoosterType type;
        [SerializeField] private string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private int goldPerBuyTen;
        [SerializeField] private TicketPerBuyRatio ticketPerBuyRatio;
        
        public BoosterType Type => type;

        public string Name => name;

        public Sprite Icon => icon;

        public int GoldPerBuyTen => goldPerBuyTen;

        // Do with goldPerBuyMore
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

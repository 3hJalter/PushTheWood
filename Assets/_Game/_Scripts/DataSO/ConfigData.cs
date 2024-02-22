using System;
using System.Collections.Generic;
using _Game.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

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

        #endregion
        
        #region Booster Purchase

        // Ticket purchase
        [FoldoutGroup("Booster Purchase")]
        public readonly Dictionary<BoosterType, BoosterConfig> boosterConfigs = new();

        #endregion

        #region Gems Purchase

        [FoldoutGroup("Gems Purchase")]
        public readonly int gemToGold = 10;

        #endregion

        #region Ads
        [FoldoutGroup("Ads")]
        public readonly int stepInterAdsCountMax = 3;
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
        
        public BoosterType Type => type;

        public string Name => name;

        public Sprite Icon => icon;

        public int GoldPerBuyTen => goldPerBuyTen;

        // Do with goldPerBuyMore
    }
}

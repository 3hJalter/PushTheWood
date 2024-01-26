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
        public readonly int goldPerUndo = 10;
        [FoldoutGroup("Booster Purchase")]
        public readonly int goldPerReset = 100;
        [FoldoutGroup("Booster Purchase")]
        public readonly int goldPerHint = 200;

        #endregion

        #region Gems Purchase

        [FoldoutGroup("Gems Purchase")]
        public readonly int gemToGold = 10;

        #endregion
        
        // [BoxGroup("Monetize")]
    }
}

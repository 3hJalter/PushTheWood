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
        
        #region Ticket Purchase

        [FoldoutGroup("Ticket Purchase")]
        // Ticket purchase
        [FoldoutGroup("Ticket Purchase/Booster Purchase Ticket")]
        public readonly int undoTicketNeed = 5;
        [FoldoutGroup("Ticket Purchase/Booster Purchase Ticket")]
        public readonly int resetIslandTicketNeed = 5;
        [FoldoutGroup("Ticket Purchase/Booster Purchase Ticket")]
        public readonly int hintTicketNeed = 5;
        // Cost to buy ticket
        [FoldoutGroup("Ticket Purchase/Cost to buy ticket")]
        public readonly int goldBuyPerTicket = 100;
        [FoldoutGroup("Ticket Purchase/Cost to buy ticket")]
        public readonly int gemBuyPerTicket = 1;

        #endregion
        
        // [BoxGroup("Monetize")]
    }
}

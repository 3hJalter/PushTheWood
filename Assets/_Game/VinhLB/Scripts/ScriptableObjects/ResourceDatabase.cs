using System.Collections.Generic;
using _Game.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace VinhLB
{
    using _Game.Resource;
    [CreateAssetMenu(fileName = "ResourceDatabase", menuName = "ScriptableObjects/ResourceDatabase")]
    public class ResourceDatabase : SerializedScriptableObject
    {
        [Title("Booster Resource Data Dictionary")]
        public Dictionary<BoosterType, ResourceData> BoosterResourceDataDict = new();
        
        [Title("Currency Resource Data Dictionary")]
        public Dictionary<CurrencyType, ResourceData> CurrencyResourceDataDict = new();
    }

    [System.Serializable]
    public struct ResourceData
    {
        public string Name;
        public Sprite IconSprite;
    }   
}
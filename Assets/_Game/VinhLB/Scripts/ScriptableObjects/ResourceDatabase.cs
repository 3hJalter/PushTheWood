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
        [SerializeField]
        private List<Sprite> boosterIconList;
        public Dictionary<BoosterType, ResourceData> BoosterResourceDataDict = new();
        
        [Title("Currency Resource Data Dictionary")]
        [SerializeField]
        private List<Sprite> currencyIconList;
        public Dictionary<CurrencyType, ResourceData> CurrencyResourceDataDict = new();

        [ContextMenu("Convert From List To Dict")]
        public void ConvertListToDict()
        {
            for (int i = 0; i < currencyIconList.Count; i++)
            {
                CurrencyType type = (CurrencyType)i;
                if (CurrencyResourceDataDict.ContainsKey(type))
                    CurrencyResourceDataDict[type] = new ResourceData(type.ToString(), currencyIconList[i]);
                else
                    CurrencyResourceDataDict.Add(type, new ResourceData(type.ToString(), currencyIconList[i]));
            }

            for (int i = 0; i < boosterIconList.Count; i++)
            {
                BoosterType type = (BoosterType)i;
                if (BoosterResourceDataDict.ContainsKey(type))
                    BoosterResourceDataDict[type] = new ResourceData(type.ToString(), boosterIconList[i]);
                else
                    BoosterResourceDataDict.Add(type, new ResourceData(type.ToString(), boosterIconList[i]));
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
            }
#endif
        }
    }

    [System.Serializable]
    public struct ResourceData
    {
        public string Name;
        public Sprite IconSprite;

        public ResourceData(string Name, Sprite IconSprite)
        {
            this.Name = Name;
            this.IconSprite = IconSprite;
        }
    }   
}
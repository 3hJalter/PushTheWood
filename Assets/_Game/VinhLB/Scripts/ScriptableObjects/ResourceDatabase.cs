using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VinhLB
{
    using _Game.Resource;
    [CreateAssetMenu(fileName = "ResourceDatabase", menuName = "ScriptableObjects/ResourceDatabase")]
    public class ResourceDatabase : SerializedScriptableObject
    {
        [Title("Booster Resource Data")]
        [SerializeField]
        private List<Sprite> _boosterIconList;
        public Dictionary<BoosterType, ResourceData> BoosterResourceDataDict = new();
        
        [Title("Currency Resource Data")]
        [SerializeField]
        private List<Sprite> _currencyIconList;
        public Dictionary<CurrencyType, ResourceData> CurrencyResourceDataDict = new();

        [ContextMenu("Convert From List To Dict")]
        public void ConvertListToDict()
        {
            for (int i = 0; i < _currencyIconList.Count; i++)
            {
                CurrencyType type = (CurrencyType)i;
                if (CurrencyResourceDataDict.ContainsKey(type))
                    CurrencyResourceDataDict[type] = new ResourceData(type.ToString(), _currencyIconList[i]);
                else
                    CurrencyResourceDataDict.Add(type, new ResourceData(type.ToString(), _currencyIconList[i]));
            }

            for (int i = 0; i < _boosterIconList.Count; i++)
            {
                BoosterType type = (BoosterType)i;
                if (BoosterResourceDataDict.ContainsKey(type))
                    BoosterResourceDataDict[type] = new ResourceData(type.ToString(), _boosterIconList[i]);
                else
                    BoosterResourceDataDict.Add(type, new ResourceData(type.ToString(), _boosterIconList[i]));
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

        public ResourceData(string name, Sprite iconSprite)
        {
            this.Name = name;
            this.IconSprite = iconSprite;
        }
    }   
}
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace VinhLB
{
    [CreateAssetMenu(fileName = "DailyLoginSettings", menuName = "ScriptableObjects/DailyLoginSettings")]
    public class DailyRewardSettingsSO : SerializedScriptableObject
    {
        public int CycleDays = 7;
        public bool OneCycleOnly = false;
        public bool DifferentFirstCycle = true;
        public bool MissRewardIfNotLogin = false;

        [ShowIf(nameof(DifferentFirstCycle), false)]
        public Reward[] FirstCycleRewardArray;
        public Reward[] RewardArray;
    }

    [System.Serializable]
    public class Reward
    {
        public string Name;
        public Sprite IconSprite;
        public int Amount;

        public void Obtain()
        {
            
        }
    }
}
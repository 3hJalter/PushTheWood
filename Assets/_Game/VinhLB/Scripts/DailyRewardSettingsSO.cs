using _Game.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VinhLB
{
    using _Game.Resource;
    [CreateAssetMenu(fileName = "DailyLoginSettings", menuName = "ScriptableObjects/DailyLoginSettings")]
    public class DailyRewardSettingsSO : SerializedScriptableObject
    {
        public int CycleDays = 7;
        public bool OneCycleOnly = false;
        public bool DifferentFirstCycle = true;
        public bool MissRewardIfNotLogin = false;
        [ShowIf(nameof(DifferentFirstCycle), false)]
        public Reward[] FirstCycleRewards;
        public Reward[] Rewards;
    }

    [System.Serializable]
    public class Reward
    {
        public RESOURCE_TYPE RewardType;
        public string Name;
        public Sprite IconSprite;
        public int Amount;

        public void Obtain(Vector3 fromPosition = default)
        {
            switch (RewardType)
            {
                case RESOURCE_TYPE.GOLD:
                    GameManager.Ins.GainGold(Amount, fromPosition);
                    break;
                case RESOURCE_TYPE.GEM:
                    GameManager.Ins.GainGems(Amount, fromPosition);
                    break;
                case RESOURCE_TYPE.SECRET_MAP_PIECE:
                    GameManager.Ins.GainSecretMapPiece(Amount);
                    break;
            }
        }
    }   
}
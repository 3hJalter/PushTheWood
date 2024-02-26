using System.Collections.Generic;
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
        public List<Reward[]> FirstCycleRewardList = new List<Reward[]>(7);
        public List<Reward[]> RewardsList = new List<Reward[]>(7);
    }

    [System.Serializable]
    public class Reward
    {
        public RewardType RewardType;
        [ShowIf(nameof(RewardType), _Game.Resource.RewardType.Booster)]
        public BoosterType BoosterType;
        [ShowIf(nameof(RewardType), _Game.Resource.RewardType.Resource)]
        public ResourceType ResourceType;
        public string Name;
        public Sprite IconSprite;
        public int Amount;

        public void Obtain(Vector3 fromPosition = default)
        {
            if (RewardType == RewardType.Booster)
            {
                //TODO: Implement logic to receive booster
            }
            else if (RewardType == RewardType.Resource)
            {
                switch (ResourceType)
                {
                    case ResourceType.Gold:
                        GameManager.Ins.GainGold(Amount, fromPosition);
                        break;
                    case ResourceType.AdTicket:
                        GameManager.Ins.GainAdTickets(Amount, fromPosition);
                        break;
                    case ResourceType.SecretMapPiece:
                        GameManager.Ins.GainSecretMapPiece(Amount);
                        break;
                }
            }
        }
    }   
}
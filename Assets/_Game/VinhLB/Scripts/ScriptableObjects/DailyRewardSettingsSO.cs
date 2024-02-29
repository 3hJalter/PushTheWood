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
        [ShowIf(nameof(RewardType), RewardType.Booster)]
        public BoosterType BoosterType;
        [ShowIf(nameof(RewardType), RewardType.Currency)]
        public CurrencyType CurrencyType;
        public int Amount;

        private UIResourceConfig _uiResourceConfig;

        public UIResourceConfig UIResourceConfig
        {
            get
            {
                if (_uiResourceConfig.Equals(default(UIResourceConfig)))
                {
                    switch (RewardType)
                    {
                        case RewardType.Booster:
                            _uiResourceConfig = DataManager.Ins.GetBoosterUIResourceConfig(BoosterType);
                            break;
                        case RewardType.Currency:
                            _uiResourceConfig = DataManager.Ins.GetCurrencyUIResourceConfig(CurrencyType);
                            break;
                    }
                }

                return _uiResourceConfig;
            }
        }

        public void Obtain(Vector3 fromPosition = default)
        {
            if (RewardType == RewardType.Booster)
            {
                //TODO: Implement logic to receive booster
                switch (BoosterType)
                {
                    case BoosterType.Undo:
                        break;
                    case BoosterType.PushHint:
                        break;
                    case BoosterType.GrowTree:
                        break;
                }
            }
            else if (RewardType == RewardType.Currency)
            {
                switch (CurrencyType)
                {
                    case CurrencyType.Gold:
                        GameManager.Ins.GainGold(Amount, fromPosition);
                        break;
                    case CurrencyType.AdTicket:
                        GameManager.Ins.GainAdTickets(Amount, fromPosition);
                        break;
                    case CurrencyType.SecretMapPiece:
                        GameManager.Ins.GainSecretMapPiece(Amount);
                        break;
                }
            }
        }
    }   
}
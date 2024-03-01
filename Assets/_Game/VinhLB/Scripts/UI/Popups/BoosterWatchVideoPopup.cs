using _Game._Scripts.Managers;
using _Game.Data;
using _Game.Managers;
using _Game.Utilities;
using System.Collections.Generic;
using _Game.Resource;
using _Game.UIs.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VinhLB
{
    public class BoosterWatchVideoPopup : UICanvas
    {
        private const int BOOSTER_AMOUNT_ON_BUY = 5;

        [SerializeField]
        private Image _boosterIcon;
        [SerializeField]
        private TMP_Text _boosterText;
        [SerializeField]
        private Image _currencyIcon;
        [SerializeField]
        private TMP_Text _currencyAmountText;
        [SerializeField]
        private TMP_Text _videoAmountText;

        private BoosterConfig _boosterConfig;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            // Cast param to BoosterConfig
            if (param != null)
            {
                _boosterConfig = (BoosterConfig)param;
                // Change the _boosterIcon to boosterConfig.icon & _boosterText to boosterConfig.name
                _boosterIcon.sprite = _boosterConfig.Icon;
                _boosterText.text = _boosterConfig.Name;
                _currencyAmountText.text = _boosterConfig.GoldPerBuyTen.ToString(("#,#"));
                switch (_boosterConfig.Type)
                {
                    case BoosterType.PushHint:
                        _videoAmountText.text = $"CLAIM({DataManager.Ins.HintAdsCount}/{DataManager.Ins.ConfigData.requireAdsForHintBooster})";
                        break;
                }
                // _currencyAmountTextContentSizeFitter.enabled = false;
            }
            GameManager.Ins.ChangeState(GameState.Pause);
        }

        public void OnClickBuyButton()
        {
            if (GameManager.Ins.TrySpendGold(_boosterConfig.GoldPerBuyTen))
            {
                EventGlobalManager.Ins.OnChangeBoosterAmount.Dispatch(_boosterConfig.Type, BOOSTER_AMOUNT_ON_BUY);
                Close();
            }
            else
            {
                DevLog.Log(DevId.Hoang, "Show popup not enough gold");
                UIManager.Ins.OpenUI<NotificationPopup>("You do not have enough gold!");
            }
        }

        public void OnClickClaimButton()
        {
            // Double reward
            AdsManager.Ins.RewardedAds.Show(null, null, new List<BoosterType> { _boosterConfig.Type },
                new List<int> { BOOSTER_AMOUNT_ON_BUY * 2 });
            Close();
        }

        public override void Close()
        {
            GameManager.Ins.ChangeState(GameState.InGame);
            base.Close();
        }
    }
}
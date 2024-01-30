using _Game._Scripts.Managers;
using _Game.Data;
using _Game.Managers;
using _Game.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VinhLB
{
    public class BoosterWatchVideoPopup : UICanvas
    {
        private const int BOOSTER_AMOUNT_ON_BUY = 10;
        [SerializeField]
        private Image _boosterIcon;
        [SerializeField]
        private TMP_Text _boosterText;
        [SerializeField]
        private Image _currencyIcon;
        [SerializeField]
        private TMP_Text _currencyAmountText;

        private BoosterConfig _boosterConfig;
        
        public override void Setup(object o = null)
        {
            base.Setup(o);
            // Cast o to BoosterConfig
            if (o != null)
            {
                _boosterConfig = (BoosterConfig) o;
                // Change the _boosterIcon to boosterConfig.icon & _boosterText to boosterConfig.name
                _boosterIcon.sprite = _boosterConfig.Icon;
                _boosterText.text = _boosterConfig.Name;
                _currencyAmountText.text = _boosterConfig.GoldPerBuyTen.ToString(("#,#"));
            }
            GameManager.Ins.ChangeState(GameState.Pause);
        }

        public void OnClickBuyButton()
        {
            if (GameManager.Ins.SpendGold(_boosterConfig.GoldPerBuyTen))
            {
                EventGlobalManager.Ins.OnChangeBoosterAmount.Dispatch(_boosterConfig.Type, BOOSTER_AMOUNT_ON_BUY);
                Close();   
            }
            else
            {
                DevLog.Log(DevId.Hoang, "Show popup not enough gold");
            }
        }

        public void OnClickClaimButton()
        {
            // Double reward
            EventGlobalManager.Ins.OnChangeBoosterAmount.Dispatch(_boosterConfig.Type, BOOSTER_AMOUNT_ON_BUY * 2);
            Close();
        }

        public override void Close()
        {
            GameManager.Ins.ChangeState(GameState.InGame);
            base.Close();
        }
    }
}
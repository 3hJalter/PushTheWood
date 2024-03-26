﻿using System;
using _Game._Scripts.Managers;
using _Game.Data;
using _Game.Managers;
using _Game.Utilities;
using System.Collections.Generic;
using _Game.Resource;
using _Game.UIs.Popup;
using TMPro;
using UnityEngine;
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
        private TMP_Text _boosterAmountText;
        [SerializeField]
        private Image _currencyIcon;
        [SerializeField]
        private TMP_Text _currencyAmountText;
        [SerializeField]
        private TMP_Text _videoAmountText;
        [SerializeField]
        private HButton _buyButton;

        private BoosterConfig _boosterConfig;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            // Cast param to BoosterConfig
            if (param != null)
            {
                _boosterConfig = (BoosterConfig)param;
                
                // _currencyAmountTextContentSizeFitter.enabled = false;
            }
            GameManager.Ins.ChangeState(GameState.Pause);
            
            _buyButton.gameObject.SetActive(GameManager.Ins.AdTickets >= _boosterConfig.TicketPerBuyRatio.ticketNeed);
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            UIManager.Ins.OpenUI<StatusBarScreen>(true);
        }
        
        public override void Close()
        {
            UIManager.Ins.CloseUI<StatusBarScreen>();
            GameManager.Ins.ChangeState(GameState.InGame);
            base.Close();
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            // Change the _boosterIcon to boosterConfig.icon & _boosterText to boosterConfig.name
            _boosterIcon.sprite = _boosterConfig.MainIcon;
            _boosterText.text = _boosterConfig.Name;
            _boosterAmountText.text = $"x{_boosterConfig.TicketPerBuyRatio.itemsPerBuy}";
            _currencyAmountText.text = _boosterConfig.TicketPerBuyRatio.ticketNeed.ToString(("#,#"));

            switch (_boosterConfig.Type)
            {
                case BoosterType.PushHint:
                    _videoAmountText.text = $"CLAIM({DataManager.Ins.HintAdsCount}/{_boosterConfig.TicketPerBuyRatio.ticketNeed})";
                    break;
                default:
                    _videoAmountText.text = "CLAIM";
                    break;
            }
        }
        
        public void OnClickBuyButton()
        {
            if (GameManager.Ins.TrySpendAdTickets(_boosterConfig.TicketPerBuyRatio.ticketNeed))
            {
                EventGlobalManager.Ins.OnChangeBoosterAmount.Dispatch(_boosterConfig.Type, _boosterConfig.TicketPerBuyRatio.itemsPerBuy);
                Close();
            }
            else
            {
                UIManager.Ins.OpenUI<NotificationPopup>("You do not have enough Ticket Ads!");
            }
        }

        public void OnClickClaimButton()
        {
            AdsManager.Ins.RewardedAds.Show(null, null, new List<BoosterType> { _boosterConfig.Type },
                new List<int> { _boosterConfig.TicketPerBuyRatio.itemsPerBuy});
            if(_boosterConfig.Type == BoosterType.PushHint)
            {
                //NOTE: If enough ads for hint 
                if (DataManager.Ins.HintAdsCount >= (_boosterConfig.TicketPerBuyRatio.ticketNeed - 1))
                {
                    //NOTE: Run Push Hint
                    Close();
                } 
            }
            else
            {
                Close();
            }
        }
    }
}
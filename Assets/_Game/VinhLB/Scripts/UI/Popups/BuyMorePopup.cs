using System;
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
    public class BuyMorePopup : UICanvas
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private TMP_Text _amountText;
        [SerializeField]
        private HButton _buyButton;
        [SerializeField]
        private HButton _claimButton;
        [SerializeField]
        private Image _currencyIcon;
        [SerializeField]
        private TMP_Text _currencyAmountText;
        [SerializeField]
        private TMP_Text _videoAmountText;

        private void Awake()
        {
            _buyButton.onClick.AddListener(OnBuyClick);
            _claimButton.onClick.AddListener(OnClaimClick);
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            // TODO: Retrieve param to detect resource to buy such as Gold, Heart, etc...
        }

        private void OnBuyClick()
        {
            UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
        }

        private void OnClaimClick()
        {
            UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
        }
    }
}
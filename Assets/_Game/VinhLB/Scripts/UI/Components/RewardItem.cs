﻿using _Game.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class RewardItem : HMonoBehaviour
    {
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private Image _iconImage;
        [SerializeField]
        private TMP_Text _amountText;

        private Reward _reward = null;

        public Vector3 IconImagePosition => _iconImage.transform.position;
        public Reward Reward => _reward;

        public void Initialize(Reward reward)
        {
            _reward = reward;

            if (_reward == null)
            {
                return;
            }

            if (_nameText != null)
            {
                _nameText.text = _reward.UIResourceConfig.Name;
            }

            _iconImage.sprite = _reward.UIResourceConfig.IconSprite;

            string amountText;
            if (_reward.RewardType == RewardType.Currency && _reward.CurrencyType == CurrencyType.Gold)
            {
                amountText = $"{_reward.Amount}";
            }
            else
            {
                amountText = $"x{_reward.Amount}";
            }
            _amountText.text = amountText;
        }
    }
}
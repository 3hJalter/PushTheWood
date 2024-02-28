﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VinhLB
{
    public class DailyRewardItem : HMonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TMP_Text _dayText;
        [SerializeField]
        private RewardItem _rewardItemPrefab;
        [SerializeField]
        private Transform _rewardItemParentTF;
        [FormerlySerializedAs("_activeRectTf")]
        [SerializeField]
        private RectTransform _activeRectTF;
        [FormerlySerializedAs("_inactiveRectTf")]
        [SerializeField]
        private RectTransform _inactiveRectTF;
        [FormerlySerializedAs("_checkedRectTf")]
        [SerializeField]
        private RectTransform _checkedRectTF;

        private int _day = -1;
        private Reward[] _rewards = null;
        private List<RewardItem> _rewardItemList = new List<RewardItem>();

        public Reward[] Rewards => _rewards;

        public void Initialize(int day, Reward[] rewards)
        {
            _day = day;
            _rewards = rewards;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                DailyRewardManager.Ins.ObtainTodayReward();
            });
        }

        public void UpdateVisual()
        {
            if (_day < 0 || _rewards is null)
            {
                return;
            }

            _dayText.text = $"Day {_day + 1}";

            for (int i = 0; i < _rewards.Length; i++)
            {
                RewardItem rewardItem;
                if (i < _rewardItemList.Count)
                {
                    rewardItem = _rewardItemList[i];
                }
                else
                {
                    rewardItem = Instantiate(_rewardItemPrefab, _rewardItemParentTF);
                    _rewardItemList.Add(rewardItem);
                }

                rewardItem.Initialize(_rewards[i]);
            }

            if (_day <= DailyRewardManager.Ins.CycleDay)
            {
                if (_day == DailyRewardManager.Ins.CycleDay && !DailyRewardManager.Ins.IsTodayRewardObtained)
                {
                    _button.interactable = true;
                    _activeRectTF.gameObject.SetActive(true);
                    _inactiveRectTF.gameObject.SetActive(false);
                    _checkedRectTF.gameObject.SetActive(false);
                }
                else
                {
                    _button.interactable = false;
                    _activeRectTF.gameObject.SetActive(false);
                    _inactiveRectTF.gameObject.SetActive(false);
                    _checkedRectTF.gameObject.SetActive(true);
                }
            }
            else
            {
                _button.interactable = false;
                _activeRectTF.gameObject.SetActive(false);
                _inactiveRectTF.gameObject.SetActive(true);
                _checkedRectTF.gameObject.SetActive(false);
            }
        }
    }
}
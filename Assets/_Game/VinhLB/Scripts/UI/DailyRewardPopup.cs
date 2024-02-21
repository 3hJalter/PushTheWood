using System.Collections;
using System.Collections.Generic;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class DailyRewardPopup : UICanvas
    {
        [SerializeField]
        private Button _claimButton;
        [SerializeField]
        private List<DailyRewardItem> _dailyRewardItemList;

        private void Awake()
        {
            _claimButton.onClick.AddListener(() =>
            {
                if (DailyRewardManager.Ins.IsTodayRewardObtained)
                {
                    DevLog.Log(DevId.Vinh, "You have claimed today reward");
                    UIManager.Ins.OpenUI<NotificationPopup>("You have claimed today reward");
                }
                else
                {
                    DailyRewardManager.Ins.ObtainTodayReward();
                }
            });

            SetupDailyRewards();

            DailyRewardManager.Ins.OnDailyRewardParamsChanged += DailyRewardManager_OnOnDailyRewardParamsChanged;
        }

        private void SetupDailyRewards()
        {
            for (int i = 0; i < DailyRewardManager.Ins.DailyRewardSettingsSO.CycleDays; i++)
            {
                if (_dailyRewardItemList[i].Reward != DailyRewardManager.Ins.Rewards[i])
                {
                    _dailyRewardItemList[i].Initialize(i, DailyRewardManager.Ins.Rewards[i]);
                }

                _dailyRewardItemList[i].UpdateVisual();
            }
        }

        private void DailyRewardManager_OnOnDailyRewardParamsChanged()
        {
            SetupDailyRewards();
        }
    }
}
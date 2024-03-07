using System;
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
                    Close();
                }
            });

            SetupDailyRewards();

            DailyRewardManager.Ins.OnDailyRewardParamsChanged += DailyRewardManager_OnOnDailyRewardParamsChanged;
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);

            _claimButton.gameObject.SetActive(!DailyRewardManager.Ins.IsTodayRewardObtained);
        }

        private void SetupDailyRewards()
        {
            for (int i = 0; i < DailyRewardManager.Ins.DailyRewardSettings.CycleDays; i++)
            {
                if (_dailyRewardItemList[i].Rewards != DailyRewardManager.Ins.Rewards[i])
                {
                    _dailyRewardItemList[i].Initialize(i, DailyRewardManager.Ins.Rewards[i]);
                }

                _dailyRewardItemList[i].UpdateVisual();
            }
        }

        private void DailyRewardManager_OnOnDailyRewardParamsChanged()
        {
            SetupDailyRewards();
            
            _claimButton.gameObject.SetActive(!DailyRewardManager.Ins.IsTodayRewardObtained);
        }
    }
}
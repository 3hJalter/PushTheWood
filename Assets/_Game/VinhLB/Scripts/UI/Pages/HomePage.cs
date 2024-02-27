﻿using System;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.UIs.Screen;
using _Game.Utilities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VinhLB
{
    public class HomePage : TabPage
    {
        [SerializeField] 
        private Button _playButton;
        [SerializeField] 
        private Button _dailyChallengeButton;
        [SerializeField]
        private Button _dailyRewardButton;
        [SerializeField]
        private Button _dailyMissionButton;
        [SerializeField]
        private Button _secretMapButton;
        [SerializeField]
        private Button _rewardChestButton;
        [SerializeField]
        private TMP_Text _rewardKeyTxt;
        [SerializeField]
        private TMP_Text _levelProgressTxt;
        [SerializeField]
        private TMP_Text _levelText;

        private void Awake()
        {
            _playButton.onClick.AddListener(() =>
            {
                UIManager.Ins.CloseAll();
                UIManager.Ins.OpenUI<InGameScreen>();
                LevelManager.Ins.InitLevel();
            });
            _dailyChallengeButton.onClick.AddListener( () =>
            {
                DevLog.Log(DevId.Hoang, "Click daily challenge button");
                UIManager.Ins.OpenUI<DailyChallengePopup>();
            });
            _dailyRewardButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily reward button");
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            });
            _dailyMissionButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily mission button");
                UIManager.Ins.OpenUI<DailyMissionPopup>();
            });
            _secretMapButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click secret map button");
                UIManager.Ins.OpenUI<SecretMapPopup>();
            });
            _rewardChestButton.onClick.AddListener(() =>
            {
                RewardManager.Ins.HomeReward.ClaimRewardChest();
            });
        }

        private void Start()
        {
            if (!DailyRewardManager.Ins.IsTodayRewardObtained)
            {
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            }
        }

        public override void UpdateUI()
        {
            _levelText.text = $"Level {LevelManager.Ins.NormalLevelIndex + 1}";
            _rewardKeyTxt.text = $"{GameManager.Ins.RewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
            _levelProgressTxt.text = $"{GameManager.Ins.LevelProgress}/{DataManager.Ins.ConfigData.requireLevelProgress}";

            // UIManager.Ins.OpenUI<MaskScreen>(new MaskData()
            // {
            //     Position = _playButton.transform.position,
            //     Size = _playButton.GetComponent<RectTransform>().sizeDelta + Vector2.one * 20f,
            //     MaskType = MaskType.Rectangle
            // });
        }
    }
}
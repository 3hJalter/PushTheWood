using System;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        private RectTransform _rewardChestIconRectTF;
        [SerializeField]
        private GameObject _rewardChestCurrencyIconGO;
        [SerializeField]
        private TMP_Text _rewardKeyTxt;
        [SerializeField]
        private Button _levelChestButton;
        [SerializeField]
        private RectTransform _levelChestIconRectTF;
        [SerializeField]
        private GameObject _levelChestCurrencyIconGO;
        [SerializeField]
        private TMP_Text _levelProgressTxt;
        [SerializeField]
        private TMP_Text _levelText;

        private STimer shakeRewardTimer;
        private STimer shakeLevelTimer;

        private void Awake()
        {
            _playButton.onClick.AddListener(() =>
            {
                UIManager.Ins.CloseAll();
                UIManager.Ins.OpenUI<InGameScreen>();
                LevelManager.Ins.InitLevel();
            });
            _dailyChallengeButton.onClick.AddListener(() =>
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
            _rewardChestButton.onClick.AddListener(() => { RewardManager.Ins.HomeReward.ClaimRewardChest(); });
            _levelChestButton.onClick.AddListener(() => { RewardManager.Ins.HomeReward.ClaimLevelChest(); });
            shakeRewardTimer = TimerManager.Ins.PopSTimer();
            shakeLevelTimer = TimerManager.Ins.PopSTimer();
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
            if (RewardManager.Ins.HomeReward.IsCanClaimRC)
            {
                if (!shakeRewardTimer.IsStart)
                {
                    shakeRewardTimer.Start(1f,
                        () => _rewardChestIconRectTF.DOShakeRotation(0.5f, 40, 10, 0, true,
                            ShakeRandomnessMode.Harmonic), true);
                    _rewardKeyTxt.text = $"FULL";
                    _rewardKeyTxt.color = Color.green;
                    _rewardChestCurrencyIconGO.SetActive(false);
                }
            }
            else
            {
                shakeRewardTimer.Stop();
                _rewardKeyTxt.text = $"{GameManager.Ins.RewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
                _rewardKeyTxt.color = Color.white;
                _rewardChestCurrencyIconGO.SetActive(true);
                _rewardChestIconRectTF.localRotation = Quaternion.identity;
            }

            if (RewardManager.Ins.HomeReward.IsCanClaimLC)
            {
                if (!shakeLevelTimer.IsStart)
                {
                    shakeLevelTimer.Start(1f,
                        () => _levelChestIconRectTF.DOShakeRotation(0.5f, 40, 10, 0, true,
                            ShakeRandomnessMode.Harmonic), true);
                    _levelProgressTxt.text = $"FULL";
                    _levelProgressTxt.color = Color.green;
                    _levelChestCurrencyIconGO.SetActive(false);
                }
            }
            else
            {
                shakeLevelTimer.Stop();
                _levelProgressTxt.text =
                    $"{GameManager.Ins.LevelProgress}/{DataManager.Ins.ConfigData.requireLevelProgress}";
                _levelProgressTxt.color = Color.white;
                _levelChestCurrencyIconGO.SetActive(true);
                _levelChestIconRectTF.localRotation = Quaternion.identity;
            }

            // UIManager.Ins.OpenUI<MaskScreen>(new MaskData()
            // {
            //     Position = _playButton.transform.position,
            //     Size = _playButton.GetComponent<RectTransform>().sizeDelta + Vector2.one * 20f,
            //     MaskType = MaskType.Rectangle
            // });
        }

        private void OnDestroy()
        {
            TimerManager.Ins.PushSTimer(shakeRewardTimer);
        }
    }
}
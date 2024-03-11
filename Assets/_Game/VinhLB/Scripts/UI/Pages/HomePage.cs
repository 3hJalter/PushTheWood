using System;
using _Game._Scripts.UIs.Component;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class HomePage : TabPage
    {
        [SerializeField]
        private HButton _playButton;
        [SerializeField]
        // private HButton _dailyChallengeButton;
        private FeatureButton dailyChallengeButton;
        [SerializeField]
        // private HButton _secretMapButton;
        private FeatureButton secretMapButton;
        [SerializeField]
        private HButton _dailyRewardButton;
        [SerializeField]
        private HButton _dailyMissionButton;
        
        [SerializeField]
        private FeatureButton rewardChestButton;
        [SerializeField]
        private RectTransform _rewardChestIconRectTF;
        [SerializeField]
        private GameObject _rewardChestCurrencyIconGO;
        [SerializeField]
        private TMP_Text _rewardKeyTxt;
        [SerializeField]
        private FeatureButton levelChestButton;
        [SerializeField]
        private RectTransform _levelChestIconRectTF;
        [SerializeField]
        private GameObject _levelChestCurrencyIconGO;
        [SerializeField]
        private TMP_Text _levelProgressTxt;
        [SerializeField]
        private TMP_Text _levelText;

        private STimer _shakeRewardTimer;
        private STimer _shakeLevelTimer;
        private Quaternion _startRewardChestIconQuaternion;
        private Quaternion _startLevelChestIconQuaternion;

        private bool _isTutorialRunning;
        
        private void Awake()
        {
            _playButton.onClick.AddListener(() =>
            {
                UIManager.Ins.CloseAll();
                LevelManager.Ins.InitLevel();
                UIManager.Ins.OpenUI<InGameScreen>();
            });
            dailyChallengeButton.AddListener(() =>
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
            secretMapButton.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click secret map button");
                UIManager.Ins.OpenUI<SecretMapPopup>();
            });
            rewardChestButton.AddListener(() =>
            {
                RewardManager.Ins.HomeReward.ClaimRewardChest();
            });
            levelChestButton.AddListener(() =>
            {
                RewardManager.Ins.HomeReward.ClaimLevelChest();
            });

            _shakeRewardTimer = TimerManager.Ins.PopSTimer();
            _shakeLevelTimer = TimerManager.Ins.PopSTimer();
            _startRewardChestIconQuaternion = _rewardChestIconRectTF.localRotation;
            _startLevelChestIconQuaternion = _levelChestIconRectTF.localRotation;
        }

        private void Start()
        {
            if (!DailyRewardManager.Ins.IsTodayRewardObtained)
            {
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            }

            // Invoke(nameof(OpenMask), 1f);
        }

        private void OnDestroy()
        {
            TimerManager.Ins.PushSTimer(_shakeRewardTimer);
        }
        public override void Open(object param = null)
        {
            base.Open(param);
            SetupHomeCamera();
            OnShowFeature();
        }
        public override void UpdateUI()
        {
            _levelText.text = $"Level {LevelManager.Ins.NormalLevelIndex + 1}";
            if (RewardManager.Ins.HomeReward.IsCanClaimRC)
            {
                if (!_shakeRewardTimer.IsStart)
                {
                    _shakeRewardTimer.Start(1f, () =>
                    {
                        _rewardChestIconRectTF.DOShakeRotation(0.5f, Vector3.forward * 10f, 10, 45f, true,
                            ShakeRandomnessMode.Harmonic);
                    }, true);
                    _rewardKeyTxt.text = $"FULL";
                    _rewardKeyTxt.color = Color.green;
                    _rewardChestCurrencyIconGO.SetActive(false);
                }
            }
            else
            {
                _shakeRewardTimer.Stop();
                _rewardKeyTxt.text = $"{GameManager.Ins.RewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
                _rewardKeyTxt.color = Color.white;
                _rewardChestCurrencyIconGO.SetActive(true);
                _rewardChestIconRectTF.localRotation = _startRewardChestIconQuaternion;
            }

            if (RewardManager.Ins.HomeReward.IsCanClaimLC)
            {
                if (!_shakeLevelTimer.IsStart)
                {
                    _shakeLevelTimer.Start(1f, () =>
                    {
                        _levelChestIconRectTF.DOShakeRotation(0.5f, Vector3.forward * 10f, 10, 45f, true,
                            ShakeRandomnessMode.Harmonic);
                    }, true);
                    _levelProgressTxt.text = $"FULL";
                    _levelProgressTxt.color = Color.green;
                    _levelChestCurrencyIconGO.SetActive(false);
                }
            }
            else
            {
                _shakeLevelTimer.Stop();
                _levelProgressTxt.text =
                    $"{GameManager.Ins.LevelProgress}/{DataManager.Ins.ConfigData.requireLevelProgress}";
                _levelProgressTxt.color = Color.white;
                _levelChestCurrencyIconGO.SetActive(true);
                _levelChestIconRectTF.localRotation = _startLevelChestIconQuaternion;
            }
        }
        private void SetupHomeCamera()
        {
            LevelManager.Ins.ConstructingLevel();
            GameGridCell waterCell, playerCell;
            (waterCell, playerCell) = LevelManager.Ins.player.SetActiveAgent(true);
            Vector3 offset = waterCell.WorldPos - playerCell.WorldPos;
            offset.Set(Mathf.Abs(offset.x) < 0.001f ? Constants.CELL_SIZE : offset.x * 12, 3.5f, Mathf.Abs(offset.z) < 0.001f ? Constants.CELL_SIZE : offset.z * 12);

            CinemachineFramingTransposer transposerCam = CameraManager.Ins.GetCameraCinemachineComponent<CinemachineFramingTransposer>(_Game.Camera.ECameraType.PerspectiveCamera);
            transposerCam.m_TrackedObjectOffset = offset;
            CameraManager.Ins.ChangeCameraTargetPosition(playerCell.WorldPos + Vector3.up * 2, 1f, Ease.OutQuad);
            CameraManager.Ins.ChangeCameraPosition(playerCell.WorldPos + offset);
        }
        private void OpenMask()
        {
            UIManager.Ins.OpenUI<MaskScreen>(new MaskData()
            {
                Position = dailyChallengeButton.transform.position,
                Size = dailyChallengeButton.GetComponent<RectTransform>().sizeDelta + Vector2.one * 20f,
                MaskType = MaskType.Rectangle,
                ClickableItem = dailyChallengeButton
            });
        }

        private void OnShowFeature()
        {
            // Get normal level index
            int normalLevelIndex = LevelManager.Ins.NormalLevelIndex;
            dailyChallengeButton.IsLock = normalLevelIndex >= DataManager.Ins.ConfigData.unlockDailyChallengeAtLevelIndex;
            secretMapButton.IsLock = normalLevelIndex >= DataManager.Ins.ConfigData.unlockSecretLevelAtLevelIndex;
            levelChestButton.IsLock = normalLevelIndex >= DataManager.Ins.ConfigData.unlockBonusChestAtLevelIndex;
            rewardChestButton.IsLock = normalLevelIndex >= DataManager.Ins.ConfigData.unlockBonusChestAtLevelIndex;
            
            GameManager.Ins.PostEvent(EventID.OnShowTutorialInMenu, normalLevelIndex);
        }
    }
}
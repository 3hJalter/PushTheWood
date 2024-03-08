using System;
using System.Globalization;
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
        private HButton _dailyChallengeButton;
        [SerializeField]
        private HButton _dailyRewardButton;
        [SerializeField]
        private HButton _dailyMissionButton;
        [SerializeField]
        private HButton _secretMapButton;
        [SerializeField]
        private HButton _rewardChestButton;
        [SerializeField]
        private RectTransform _rewardChestIconRectTF;
        [SerializeField]
        private RectTransform _rewardChestCurrencyIconRectTF;
        [SerializeField]
        private TMP_Text _rewardKeyTxt;
        [SerializeField]
        private HButton _levelChestButton;
        [SerializeField]
        private RectTransform _levelChestIconRectTF;
        [SerializeField]
        private RectTransform _levelChestCurrencyIconRectTF;
        [SerializeField]
        private TMP_Text _levelProgressTxt;
        [SerializeField]
        private TMP_Text _levelText;

        private STimer _shakeRewardTimer;
        private STimer _shakeLevelTimer;
        private Quaternion _startRewardChestIconQuaternion;
        private Quaternion _startLevelChestIconQuaternion;

        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeRewardKeys,
                data => ChangeRewardKeyValue((ResourceChangeData)data));
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeLevelProgress,
                data => ChangeLevelProgressValue((ResourceChangeData)data));

            _playButton.onClick.AddListener(() =>
            {
                UIManager.Ins.CloseAll();
                LevelManager.Ins.InitLevel();
                UIManager.Ins.OpenUI<InGameScreen>();
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
            _rewardChestButton.onClick.AddListener(() =>
            {
                if (!CanClaimRewardChest())
                {
                    return;
                }
                
                if (RewardManager.Ins.HomeReward.TryClaimRewardChest())
                {
                    GameManager.Ins.SmoothRewardKeys = GameManager.Ins.RewardKeys;

                    UpdateRewardChestUI();
                }
            });
            _levelChestButton.onClick.AddListener(() =>
            {
                if (RewardManager.Ins.HomeReward.TryClaimLevelChest())
                {
                }
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
            GameManager.Ins.UnregisterListenerEvent(EventID.OnChangeRewardKeys,
                data => ChangeRewardKeyValue((ResourceChangeData)data));
            GameManager.Ins.UnregisterListenerEvent(EventID.OnChangeLevelProgress,
                data => ChangeLevelProgressValue((ResourceChangeData)data));

            TimerManager.Ins.PushSTimer(_shakeRewardTimer);
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            SetupHomeCamera();

            UpdateRewardChestUI();
        }

        public override void UpdateUI()
        {
            _levelText.text = $"Level {LevelManager.Ins.NormalLevelIndex + 1}";
            
            // if (RewardManager.Ins.HomeReward.IsCanClaimRC)
            // {
            //     if (!_shakeRewardTimer.IsStart)
            //     {
            //         _shakeRewardTimer.Start(1f, () =>
            //         {
            //             _rewardChestIconRectTF.DOShakeRotation(0.5f, Vector3.forward * 10f, 10, 45f, true,
            //                 ShakeRandomnessMode.Harmonic);
            //         }, true);
            //         _rewardKeyTxt.text = $"FULL";
            //         _rewardKeyTxt.color = Color.green;
            //         _rewardChestCurrencyIconRectTF.gameObject.SetActive(false);
            //     }
            // }
            // else
            // {
            //     _shakeRewardTimer.Stop();
            //     _rewardKeyTxt.text = $"{GameManager.Ins.RewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
            //     _rewardKeyTxt.color = Color.white;
            //     _rewardChestCurrencyIconRectTF.gameObject.SetActive(true);
            //     _rewardChestIconRectTF.localRotation = _startRewardChestIconQuaternion;
            // }

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
                    _levelChestCurrencyIconRectTF.gameObject.SetActive(false);
                }
            }
            else
            {
                _shakeLevelTimer.Stop();
                _levelProgressTxt.text =
                    $"{GameManager.Ins.LevelProgress}/{DataManager.Ins.ConfigData.requireLevelProgress}";
                _levelProgressTxt.color = Color.white;
                _levelChestCurrencyIconRectTF.gameObject.SetActive(true);
                _levelChestIconRectTF.localRotation = _startLevelChestIconQuaternion;
            }
        }

        private void SetupHomeCamera()
        {
            LevelManager.Ins.ConstructingLevel();
            GameGridCell waterCell, playerCell;
            (waterCell, playerCell) = LevelManager.Ins.player.SetActiveAgent(true);
            Vector3 offset = waterCell.WorldPos - playerCell.WorldPos;
            offset.Set(Mathf.Abs(offset.x) < 0.001f ? Constants.CELL_SIZE : offset.x * 12, 3.5f,
                Mathf.Abs(offset.z) < 0.001f ? Constants.CELL_SIZE : offset.z * 12);

            CinemachineFramingTransposer transposerCam =
                CameraManager.Ins.GetCameraCinemachineComponent<CinemachineFramingTransposer>(_Game.Camera.ECameraType
                    .PerspectiveCamera);
            transposerCam.m_TrackedObjectOffset = offset;
            CameraManager.Ins.ChangeCameraTargetPosition(playerCell.WorldPos + Vector3.up * 2, 1f, Ease.OutQuad);
            CameraManager.Ins.ChangeCameraPosition(playerCell.WorldPos + offset);
        }

        private void ChangeRewardKeyValue(ResourceChangeData data)
        {
            // If screen not open yet, just set value
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (data.ChangedAmount > 0 && data.Source is Vector3 spawnPosition)
            {
                int collectingRewardKeys = Mathf.Min((int)data.ChangedAmount, Constants.MAX_UI_UNIT);
                CollectingResourceManager.Ins.SpawnCollectingUIRewardKeys(collectingRewardKeys, spawnPosition,
                    _rewardChestIconRectTF,
                    (progress) =>
                    {
                        GameManager.Ins.SmoothRewardKeys += data.ChangedAmount / collectingRewardKeys;

                        UpdateRewardChestUI();
                    });
            }
            else
            {
                _rewardKeyTxt.text = $"{GameManager.Ins.RewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
            }
        }

        private void ChangeLevelProgressValue(ResourceChangeData data)
        {
        }

        private void UpdateRewardChestUI()
        {
            if (CanClaimRewardChest())
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
                    _rewardChestCurrencyIconRectTF.gameObject.SetActive(false);
                }
            }
            else
            {
                if (_shakeRewardTimer.IsStart)
                {
                    _shakeRewardTimer.Stop();
                    _rewardKeyTxt.color = Color.white;
                    _rewardChestCurrencyIconRectTF.gameObject.SetActive(true);
                    _rewardChestIconRectTF.localRotation = _startRewardChestIconQuaternion;
                }

                _rewardKeyTxt.text = $"{GameManager.Ins.SmoothRewardKeys}/{DataManager.Ins.ConfigData.requireRewardKey}";
            }
        }

        private bool CanClaimRewardChest()
        {
            return GameManager.Ins.SmoothRewardKeys >= DataManager.Ins.ConfigData.requireRewardKey;
        }

        private void OpenMask()
        {
            UIManager.Ins.OpenUI<MaskScreen>(new MaskData()
            {
                Position = _dailyChallengeButton.transform.position,
                Size = _dailyChallengeButton.GetComponent<RectTransform>().sizeDelta + Vector2.one * 20f,
                MaskType = MaskType.Rectangle,
                ClickableItem = _dailyChallengeButton
            });
        }
    }
}
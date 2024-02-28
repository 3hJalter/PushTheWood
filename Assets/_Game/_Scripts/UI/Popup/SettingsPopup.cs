using System;
using _Game._Scripts.Managers;
using _Game._Scripts.Utilities;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Popup
{
    public class SettingsPopup : UICanvas
    {
        [SerializeField]
        private TMP_Text _headerText;
        [SerializeField]
        private GameObject _navigationGroupGO;

        [SerializeField]
        private GameObject[] _activeMoveChoices;
        
        [SerializeField]
        private Slider _bgmVolumeSlider;
        [SerializeField]
        private Slider _sfxVolumeSlider;
        [SerializeField]
        private ToggleSwitch _hapticToggleSwitch;
        [SerializeField]
        private ToggleSwitch _gridToggleSwitch;

        private void Awake()
        {
            _bgmVolumeSlider.onValueChanged.AddListener(OnChangeBgmVolume);
            _sfxVolumeSlider.onValueChanged.AddListener(OnChangeSfxVolume);
        }

        private void OnDestroy()
        {
            _bgmVolumeSlider.onValueChanged.RemoveAllListeners();
            _sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        }


        public override void Setup(object param = null)
        {
            base.Setup(param);

            _bgmVolumeSlider.value = AudioManager.Ins.BgmVolume;
            _sfxVolumeSlider.value = AudioManager.Ins.SfxVolume;
            _hapticToggleSwitch.SetState(HVibrate.IsHapticOn, false);
            _gridToggleSwitch.SetState(FXManager.Ins.IsGridOn, false);

            if (GameManager.Ins.IsState(GameState.InGame))
            {
                GameManager.Ins.ChangeState(GameState.Pause);

                _headerText.text = "Pause";
                _navigationGroupGO.SetActive(true);
            }
            else
            {
                _headerText.text = "Settings";
                _navigationGroupGO.SetActive(false);
            }

            UpdateCurrentMoveChoice();
        }

        public override void Open(object param = null)
        {
            base.Open(param);

            MoveInputManager.Ins.ShowContainer(false);
        }

        public override void Close()
        {
            base.Close();
            if (UIManager.Ins.IsOpened<InGameScreen>())
            {
                GameManager.Ins.ChangeState(GameState.InGame);
                MoveInputManager.Ins.ShowContainer(true);
            }
        }

        public void OnChangeBgmVolume(float value)
        {
            AudioManager.Ins.ToggleBgmVolume(value);
        }

        public void OnChangeSfxVolume(float value)
        {
            AudioManager.Ins.ToggleSfxVolume(value);
            AudioManager.Ins.ToggleEnvironmentVolume(value);
        }

        public void OnToggleHapticOff()
        {
            HVibrate.OnToggleHaptic(false);
            DevLog.Log(DevId.Hoang, "Haptic off");
        }

        public void OnToggleHapticOn()
        {
            HVibrate.OnToggleHaptic(true);
            DevLog.Log(DevId.Hoang, "Haptic on");
        }

        public void OnToggleGridOff()
        {
            DevLog.Log(DevId.Vinh, "Grid off");
            FXManager.Ins.SwitchGridActive(true, false);
        }
        
        public void OnToggleGridOn()
        {
            DevLog.Log(DevId.Vinh, "Grid on");
            FXManager.Ins.SwitchGridActive(true, true);
        }

        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }

        public void OnClickUseDPadButton()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.DPad);

            UpdateCurrentMoveChoice();
        }

        public void OnClickUseSwitchButton()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.Switch);

            UpdateCurrentMoveChoice();
        }

        public void OnClickUseSwipeButton()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.Swipe);

            UpdateCurrentMoveChoice();
        }

        public void OnClickUseSwipeContinuousButton()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.SwipeContinuous);

            UpdateCurrentMoveChoice();
        }

        public void OnClickToggleGridButton()
        {
            FXManager.Ins.SwitchGridActive();
        }

        public void OnClickTutorialButton()
        {
            UIManager.Ins.OpenUI<TutorialPopup>();
        }

        public void OnClickGoMenuButton()
        {
            if (LevelManager.Ins.CurrentLevel.LevelType != LevelType.Normal)
            {
                LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex, false);
            }
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnInterAdsStepCount, 1);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
        }

        private void UpdateCurrentMoveChoice()
        {
            for (int i = 0; i < _activeMoveChoices.Length; i++)
            {
                if (i == (int)MoveInputManager.Ins.CurrentChoice)
                {
                    _activeMoveChoices[i].SetActive(true);
                }
                else
                {
                    _activeMoveChoices[i].SetActive(false);
                }
            }
        }

        #region Old functions
        public void OnClickSelectLevelButton()
        {
            // UIManager.Ins.CloseAll();
            // UIManager.Ins.OpenUI<WorldLevelScreen>();
        }

        public void OnClickHowToPlayButton()
        {
            Debug.Log("Click how to play button");
        }

        public void OnClickTogglePostProcessing()
        {
            Debug.Log("Click toggle post-processing button");
            FXManager.Ins.TogglePostProcessing();
        }

        public void OnClickToggleWater()
        {
            Debug.Log("Click toggle water button");
            FXManager.Ins.ToggleWater();
        }

        public void OnClickToggleGrasses()
        {
            Debug.Log("Click toggle grass button");
            FXManager.Ins.ToggleGrasses();
        }

        public void OnClickTestBoosterWatchVideo()
        {
            UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
        }
        #endregion
    }
}
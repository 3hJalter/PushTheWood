using _Game._Scripts.Managers;
using _Game._Scripts.Utilities;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
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
        private GameObject _group2GO;
        [SerializeField]
        private GameObject _group3GO;
        
        [SerializeField]
        private Toggle _musicToggle;
        [SerializeField]
        private Toggle _soundToggle;
        [SerializeField]
        private Toggle _hapticToggle;

        private void Awake()
        {
            _musicToggle.onValueChanged.AddListener(value => OnChangeMusicVolume(!value));
            _soundToggle.onValueChanged.AddListener(value => OnChangeSoundVolume(!value));
            _hapticToggle.onValueChanged.AddListener(value => OnChangeHapticValue(value));
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);

            // bgmToggleSwitch.SetState(!AudioManager.Ins.IsBgmMute(), false);
            // sfxToggleSwitch.SetState(!AudioManager.Ins.IsSfxMute(), false);
            // environmentToggleSwitch.SetState(!AudioManager.Ins.IsEnvironmentMute(), false);
            // _hapticToggleSwitch.SetState(HVibrate.IsHapticOn, false);
            _musicToggle.isOn = !AudioManager.Ins.IsBgmMute();
            _soundToggle.isOn = !AudioManager.Ins.IsSfxMute();
            _hapticToggle.isOn = HVibrate.IsHapticOn;

            if (GameManager.Ins.IsState(GameState.InGame))
            {
                GameManager.Ins.ChangeState(GameState.Pause);

                _headerText.text = "Pause";
                _group2GO.SetActive(false);
                _group3GO.SetActive(true);
            }
            else
            {
                _headerText.text = "Settings";
                _group2GO.SetActive(true);
                _group3GO.SetActive(false);
            }
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

        public void OnChangeMusicVolume(bool value)
        {
            AudioManager.Ins.ToggleBgmVolume(value);
            AudioManager.Ins.ToggleEnvironmentVolume(value);
            AudioManager.Ins.PlaySfx(AudioEnum.SfxType.ClickToggle);
        }

        public void OnChangeSoundVolume(bool value)
        {
            AudioManager.Ins.ToggleSfxVolume(value);
            AudioManager.Ins.PlaySfx(AudioEnum.SfxType.ClickToggle);
        }

        public void OnChangeEnvironmentVolume(bool value)
        {
            AudioManager.Ins.PlaySfx(AudioEnum.SfxType.ClickToggle);
            AudioManager.Ins.ToggleEnvironmentVolume(value);
        }

        public void OnChangeHapticValue(bool value)
        {
            AudioManager.Ins.PlaySfx(AudioEnum.SfxType.ClickToggle);
            HVibrate.OnToggleHaptic(value);
        }
        
        public void OnToggleHapticOff()
        {
            HVibrate.OnToggleHaptic(false);
        }

        public void OnToggleHapticOn()
        {
            HVibrate.OnToggleHaptic(true);
        }

        public void OnToggleGridOff()
        {
            FXManager.Ins.SwitchGridActive(true, false);
        }
        
        public void OnToggleGridOn()
        {
            FXManager.Ins.SwitchGridActive(true, true);
        }

        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }
        
        public void OnClickToggleGridButton()
        {
            FXManager.Ins.SwitchGridActive();
        }

        public void OnSupportClick()
        {
            UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
        }

        public void OnClickGoMenuButton()
        {
            LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex, false);
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnInterAdsStepCount, 1);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
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
            UIManager.Ins.OpenUI<TutorialPopup>();
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
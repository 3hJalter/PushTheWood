using _Game._Scripts.Managers;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Popup
{
    public class SettingsPopup : UICanvas
    {
        [SerializeField]
        private HButton _mainMenuButton;
        
        [SerializeField] private GameObject bgmMuteImage;
        [SerializeField] private GameObject sfxMuteImage;
        [SerializeField] private GameObject envMuteImage;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            if (GameManager.Ins.IsState(GameState.InGame))
            {
                _mainMenuButton.gameObject.SetActive(true);
                GameManager.Ins.ChangeState(GameState.Pause);
            }
            else
            {
                _mainMenuButton.gameObject.SetActive(false);
            }

            bgmMuteImage.SetActive(AudioManager.Ins.IsBgmMute());
            sfxMuteImage.SetActive(AudioManager.Ins.IsSfxMute());
            envMuteImage.SetActive(AudioManager.Ins.IsEnvironmentMute());
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

        public void OnToggleBgm()
        {
            bgmMuteImage.SetActive(AudioManager.Ins.ToggleBgmMute());
        }

        public void OnToggleSfx()
        {
            sfxMuteImage.SetActive(AudioManager.Ins.ToggleSfxMute());
        }

        public void OnToggleEnvSound()
        {
            envMuteImage.SetActive(AudioManager.Ins.ToggleEnvironmentMute());
        }

        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }
        
        public void OnClickGoMenuButton()
        {
            if (LevelManager.Ins.CurrentLevel.LevelType != LevelType.Normal)
            {
                LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex);
            }
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
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
    }
}

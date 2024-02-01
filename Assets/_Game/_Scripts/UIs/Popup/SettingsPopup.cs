﻿using _Game._Scripts.Managers;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;
using VinhLB;

namespace _Game.UIs.Popup
{
    public class SettingsPopup : UICanvas
    {
        [SerializeField]
        private HButton _mainMenuButton;
        
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

        public void OnClickChangeBgmStatusButton()
        {
            Debug.Log("Click change bgm button");
        }

        public void OnClickChangeSfxStatusButton()
        {
            Debug.Log("Click change sfx button");
        }

        public void OnClickLikeButton()
        {
            Debug.Log("Click like button");
        }

        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }

        public void OnClickSelectLevelButton()
        {
            // UIManager.Ins.CloseAll();
            // UIManager.Ins.OpenUI<WorldLevelScreen>();
        }
        
        public void OnClickGoMenuButton()
        {
            GameManager.Ins.ChangeState(GameState.MainMenu);
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

        public void OnClickTestBoosterWatchVideo()
        {
            UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
        }
    }
}

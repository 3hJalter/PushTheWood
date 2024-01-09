﻿using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using DG.Tweening;
using UnityEngine;

namespace _Game.UIs.Popup
{
    public class SettingsPopup : UICanvas
    {
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
            LevelManager.Ins.OnRestart();
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

﻿using _Game._Scripts.Managers;
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
        private GameObject _navigationGroupGO;

        [SerializeField]
        private GameObject[] _activeMoveChoices;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);

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

        public void OnClickGridToggleButton()
        {
            FXManager.Ins.SwitchGridActive();
        }

        public void OnClickSelectLevelButton()
        {
            // UIManager.Ins.CloseAll();
            // UIManager.Ins.OpenUI<WorldLevelScreen>();
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

        public void OnClickTestBoosterWatchVideo()
        {
            UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
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
    }
}

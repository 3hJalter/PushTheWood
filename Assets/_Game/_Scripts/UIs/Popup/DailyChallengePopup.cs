﻿using System;
using System.Collections.Generic;
using _Game._Scripts.UIs.Component;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using UnityEngine;

namespace _Game.UIs.Popup
{
    public class DailyChallengePopup : UICanvas
    {
        private const float PANEL_INIT_HEIGHT = 662f;
        [SerializeField] private RectTransform panel;
        [SerializeField] List<DailyChallengeButton> dailyChallengeButtons;
        private DailyChallengeButton _currentBtnClick;
        private int currentDay;
        private int daysInMonth;
        
        // interact Btn
        [SerializeField] private GameObject notYetBtn;
        [SerializeField] private HButton payToPlayBtn;
        [SerializeField] private HButton playBtn;
        [SerializeField] private HButton replayBtn;
        
        private void Awake()
        {
            // Get number of days in month
            daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            currentDay = DateTime.Now.Day;
            // Set panel height by adding 123f for each row, each row has 6 buttons
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, PANEL_INIT_HEIGHT + (123f * Mathf.CeilToInt(daysInMonth / 6f)));
            // Set listener for interact btn
            payToPlayBtn.onClick.AddListener(OnClickPayToPlayButton);
            playBtn.onClick.AddListener(OnClickPlayButton);
            replayBtn.onClick.AddListener(OnClickReplayButton);
        }

        private void OnDestroy()
        {
            // Remove listener for interact btn
            payToPlayBtn.onClick.RemoveAllListeners();
            playBtn.onClick.RemoveAllListeners();
            replayBtn.onClick.RemoveAllListeners();
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            // Set up buttons
            for (int index = 0; index < dailyChallengeButtons.Count; index++)
            {
                // if index is greater than days in month, disable button
                if (index >= daysInMonth)
                {
                    dailyChallengeButtons[index].gameObject.SetActive(false);
                    continue;
                }
                DailyChallengeButton dailyChallengeButton = dailyChallengeButtons[index];
                dailyChallengeButton.SetIndex(index, currentDay);
                dailyChallengeButton.onClick.AddListener(delegate
                {
                    OnClickDailyChallengeButton(
                        dailyChallengeButton.Index);
                });
                dailyChallengeButtons[index].gameObject.SetActive(true);
            }
            // click today button
            OnClickDailyChallengeButton(currentDay - 1);
        }
        
        public override void Close()
        {
            // Remove listener for buttons
            for (int index = 0; index < dailyChallengeButtons.Count; index++)
            {
                dailyChallengeButtons[index].onClick.RemoveAllListeners();
            }
            _currentBtnClick = null;
            base.Close();
        }

        private void OnClickDailyChallengeButton(int index)
        {
            if (dailyChallengeButtons[index] == _currentBtnClick) return;
            DevLog.Log(DevId.Hoang, $"OnClick Daily Challenge Button with index {index}");
            if (_currentBtnClick != null)
                _currentBtnClick.OnUnHover();
            _currentBtnClick = dailyChallengeButtons[index];
            _currentBtnClick.OnHover();
            OnHandleCurrentButton();
        }

        private void OnClickPlayButton()
        {
            DevLog.Log(DevId.Hoang, "OnClick Play Button");
            OnPlay();
        }

        private void OnClickPayToPlayButton()
        {
            DevLog.Log(DevId.Hoang, "OnClick Pay To Play Button");
            // TODO: show popup to pay to play before play
            // Temporary solution: just play
            DevLog.Log(DevId.Hoang, "Temporary solution: just play");
            OnPlay();
        }

        private void OnClickReplayButton()
        {
            DevLog.Log(DevId.Hoang, "OnClick Replay Button");
            // TODO: show popup to pay to play before play
            // Temporary solution: just play
            DevLog.Log(DevId.Hoang, "Temporary solution: just play");
            OnPlay();
        }

        private void OnPlay()
        {
            LevelManager.Ins.OnGoLevel(LevelType.DailyChallenge, _currentBtnClick.Index);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<InGameScreen>();
        }
        
        private void OnHandleCurrentButton()
        {
            // hide all interact btn
            notYetBtn.SetActive(false);
            payToPlayBtn.gameObject.SetActive(false);
            playBtn.gameObject.SetActive(false);
            replayBtn.gameObject.SetActive(false);
            switch (_currentBtnClick.State)
            {
                // if current button is not yet, show not yet btn
                case DailyChallengeButtonState.NotYet:
                    notYetBtn.SetActive(true);
                    break;
                // if current button is un clear, show pay to play btn
                case DailyChallengeButtonState.UnClear:
                    payToPlayBtn.gameObject.SetActive(true);
                    break;
                // if current button is clear, show replay btn
                case DailyChallengeButtonState.Clear:
                    replayBtn.gameObject.SetActive(true);
                    break;
                // if current button is today, show play btn
                case DailyChallengeButtonState.Today:
                    playBtn.gameObject.SetActive(true);
                    break;
            }
        }


    }
}

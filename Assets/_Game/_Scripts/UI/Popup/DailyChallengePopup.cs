using System;
using System.Collections.Generic;
using _Game._Scripts.UIs.Component;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Popup
{
    public class DailyChallengePopup : UICanvas
    {
        private const float PANEL_INIT_HEIGHT = 662f;
        [SerializeField] private RectTransform panel;
        [SerializeField] private Slider dailyChallengeSliderProgress;
        [SerializeField] private List<DailyChallengeRewardButton> dailyChallengeRewardButtons;
        [SerializeField] List<DailyChallengeButton> dailyChallengeButtons;
        private DailyChallengeButton _currentBtnClick;
        private int _currentDay;
        
        // interact Btn
        [SerializeField] private GameObject notYetBtn;
        [SerializeField] private HButton payToPlayBtn;
        [SerializeField] private HButton adsToPlayBtn;
        [SerializeField] private HButton playBtn;
        
        private void Awake()
        {
            // Set panel height by adding 123f for each row, each row has 7 buttons
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, PANEL_INIT_HEIGHT + (123f * Mathf.CeilToInt(Constants.DAILY_CHALLENGER_COUNT / 7f)));
            // Set listener for interact btn
            payToPlayBtn.onClick.AddListener(OnClickPayToPlayButton);
            adsToPlayBtn.onClick.AddListener(OnClickAdToPlayButton);
            playBtn.onClick.AddListener(OnClickPlayButton);
        }

        private void OnDestroy()
        {
            // Remove listener for interact btn
            payToPlayBtn.onClick.RemoveAllListeners();
            adsToPlayBtn.onClick.RemoveAllListeners();
            playBtn.onClick.RemoveAllListeners();
        }

        private void OnUpdateProgress()
        {
            float progress = (float) DataManager.Ins.GameData.user.dailyLevelIndexComplete.Count / Constants.DAILY_CHALLENGER_COUNT;
            dailyChallengeSliderProgress.value = progress;
        }

        public override void Setup(object param = null)
        {
            base.Setup(param);
            _currentDay = DataManager.Ins.GameData.user.currentDailyChallengerDay;
            // Set up buttons
            for (int index = 0; index < dailyChallengeButtons.Count; index++)
            {
                // if index is greater than days in month, disable button
                if (index >= Constants.DAILY_CHALLENGER_COUNT)
                {
                    dailyChallengeButtons[index].gameObject.SetActive(false);
                    continue;
                }
                DailyChallengeButton dailyChallengeButton = dailyChallengeButtons[index];
                dailyChallengeButton.SetIndex(index, _currentDay);
                dailyChallengeButton.onClick.AddListener(delegate
                {
                    OnClickDailyChallengeButton(
                        dailyChallengeButton.Index);
                });
                dailyChallengeButtons[index].gameObject.SetActive(true);
            }
            for (int index = 0; index < dailyChallengeRewardButtons.Count; index++)
            {
                dailyChallengeRewardButtons[index].OnInit(this, DataManager.Ins.ConfigData.dailyChallengeRewardMilestones[index]);
            }
            
            OnUpdateProgress();
            // click today button
            OnClickDailyChallengeButton(_currentDay - 1);
        }
        
        public override void Close()
        {
            // Remove listener for buttons
            for (int index = 0; index < dailyChallengeButtons.Count; index++)
            {
                dailyChallengeButtons[index].onClick.RemoveAllListeners();
            }
            _currentBtnClick.OnUnHover();
            _currentBtnClick = null;
            
            base.Close();
        }

        private void OnClickDailyChallengeButton(int index)
        {
            if (dailyChallengeButtons[index] == _currentBtnClick) return;
            if (_currentBtnClick != null)
                _currentBtnClick.OnUnHover();
            _currentBtnClick = dailyChallengeButtons[index];
            _currentBtnClick.OnHover();
            OnHandleCurrentButton();
        }
        
        private void OnClickPlayButton()
        {
            OnPlay();
        }

        private void OnClickAdToPlayButton()
        {
            AdsManager.Ins.RewardedAds.Show();
            OnPlay();
        }
        
        private void OnClickPayToPlayButton()
        {
            if (GameManager.Ins.TrySpendAdTickets(1))
            {
                // Temporary solution: just play
                OnPlay();
            };
        }

        private void OnPlay()
        {
            LevelManager.Ins.OnGoLevel(LevelType.DailyChallenge, _currentBtnClick.Index + 1);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<InGameScreen>();
        }
        
        private void OnHandleCurrentButton()
        {
            // hide all interact btn
            notYetBtn.SetActive(false);
            payToPlayBtn.gameObject.SetActive(false);
            playBtn.gameObject.SetActive(false);
            adsToPlayBtn.gameObject.SetActive(false);
            switch (_currentBtnClick.State)
            {
                // if current button is not yet, show not yet btn
                case DailyChallengeButtonState.NotYet:
                    notYetBtn.SetActive(true);
                    break;
                // if current button is un clear, show pay to play btn
                case DailyChallengeButtonState.UnClear:
                    if (DataManager.Ins.GameData.user.adTickets > 0)
                    {
                        payToPlayBtn.gameObject.SetActive(true);
                    }
                    else
                    {
                        adsToPlayBtn.gameObject.SetActive(true);
                    }
                    break;
                // if current button is clear, show replay btn
                case DailyChallengeButtonState.Clear:
                    // replayBtn.gameObject.SetActive(true); // TEMPORARY REMOVE
                    notYetBtn.SetActive(true);
                    break;
                // if current button is today, show play btn
                case DailyChallengeButtonState.Today:
                    playBtn.gameObject.SetActive(true);
                    break;
            }
        }


    }
}

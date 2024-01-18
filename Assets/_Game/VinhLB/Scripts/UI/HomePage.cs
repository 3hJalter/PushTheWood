using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class HomePage : TabPage
    {
        [SerializeField]
        private Button _dailyRewardButton;
        [SerializeField]
        private Button _dailyChallengeButton;
        [SerializeField]
        private Button _secretMapButton;

        private void Awake()
        {
            _dailyRewardButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily reward button");
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            });
            _dailyChallengeButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily challenge button");
                UIManager.Ins.OpenUI<DailyChallengePopup>();
            });
            _secretMapButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click secret map button");
            });
        }

        private void Start()
        {
            if (!DailyRewardManager.Ins.IsTodayRewardObtained)
            {
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            }
        }
    }
}
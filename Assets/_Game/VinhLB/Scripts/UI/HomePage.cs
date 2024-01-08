using _Game.Managers;
using _Game.UIs.Popup;
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

        private void Awake()
        {
            _dailyRewardButton.onClick.AddListener(() =>
            {
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            });
            _dailyChallengeButton.onClick.AddListener(() =>
            {
                UIManager.Ins.OpenUI<DailyChallengePopup>();
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
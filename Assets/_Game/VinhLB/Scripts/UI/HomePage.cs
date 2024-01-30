using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using _Game.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class HomePage : TabPage
    {
        [SerializeField] 
        private HButton dailyChallengeButton;
        [SerializeField]
        private Button _dailyRewardButton;
        [SerializeField]
        private Button _dailyMissionButton;
        [SerializeField]
        private Button _secretMapButton;
        [SerializeField]
        private TMP_Text _levelText;

        private void Awake()
        {
            dailyChallengeButton.onClick.AddListener( () =>
            {
                DevLog.Log(DevId.Vinh, "Click daily challenge button");
                UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
            });
            _dailyRewardButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily reward button");
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            });
            _dailyMissionButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click daily mission button");
                UIManager.Ins.OpenUI<DailyMissionPopup>();
            });
            _secretMapButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Click secret map button");
                UIManager.Ins.OpenUI<NotificationPopup>(Constants.FEATURE_COMING_SOON);
            });
        }

        private void Start()
        {
            if (!DailyRewardManager.Ins.IsTodayRewardObtained)
            {
                UIManager.Ins.OpenUI<DailyRewardPopup>();
            }
        }

        public override void Open()
        {
            base.Open();

            Invoke(nameof(UpdateStatus), 0.01f);
        }

        private void UpdateStatus()
        {
            _levelText.text = $"Level\n{LevelManager.Ins.NormalLevelIndex + 1}";
        }
    }
}
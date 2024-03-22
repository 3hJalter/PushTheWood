using System;
using _Game.Data;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Resource;
using _Game.UIs.Popup;
using UnityEngine;
using VinhLB;

namespace _Game._Scripts.UIs.Component
{
    [RequireComponent(typeof(HButton))]
    public class DailyChallengeRewardButton : MonoBehaviour
    {
        [SerializeField] private HButton hButton;
        [SerializeField] private GameObject lockIcon;
        [SerializeField] private GameObject unlockIcon;
        [SerializeField] private GameObject claimedIcon;

        private DailyChallengePopup _popup;
        private DailyChallengeRewardMilestone _milestone;
        private DcRewardState _state;
        
        public DcRewardState State
        {
            get => _state;
            set
            {
                _state = value;
                OnChangeState();
            }
        }

        private void Awake()
        {
            hButton.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            hButton.onClick.RemoveAllListeners();
        }

        public void OnInit(DailyChallengePopup popup, DailyChallengeRewardMilestone milestone)
        {
            _popup = popup;
            _milestone = milestone;
            // Verify state
            int clearCount = DataManager.Ins.GameData.user.dailyLevelIndexComplete.Count;
            if (clearCount >= _milestone.clearLevelNeed)
            {
                State = DataManager.Ins.GameData.user.dailyChallengeRewardCollected.Contains(_milestone.clearLevelNeed) ? DcRewardState.Claimed : DcRewardState.Unlock;
            }
            else
            {
                State = DcRewardState.Lock;
            } 
        }

        private void OnClick()
        {
            if (State is not DcRewardState.Unlock) return;
            // Claim reward
            // TODO: Change to show claim popup to verify
            State = DcRewardState.Claimed;
            UIManager.Ins.OpenUI<RewardPopup>(GetReward()).OnClicked += () =>
            {
                DataManager.Ins.GameData.user.dailyChallengeRewardCollected.Add(_milestone.clearLevelNeed);
                DataManager.Ins.Save();
                GameManager.Ins.PostEvent(EventID.OnUpdateUI);
            };
        }

        private void OnChangeState()
        {
            lockIcon.SetActive(State == DcRewardState.Lock);
            unlockIcon.SetActive(State == DcRewardState.Unlock);
            claimedIcon.SetActive(State == DcRewardState.Claimed);
            hButton.interactable = State == DcRewardState.Unlock;
        }
        
        private Reward[] GetReward()
        {
            Reward[] rewards = new Reward[_milestone.rewards.Count];
            for (int i = 0; i < _milestone.rewards.Count; i++)
            {
                // Convert each to Reward
                rewards[i] = new Reward
                {
                    CurrencyType = _milestone.rewards[i].currencyType,
                    Amount = _milestone.rewards[i].quantity,
                    RewardType = RewardType.Currency
                };
            }
            return rewards;
        }
    }

    public enum DcRewardState
    {
        Lock = 0,
        Unlock = 1,
        Claimed = 2,
    }
}

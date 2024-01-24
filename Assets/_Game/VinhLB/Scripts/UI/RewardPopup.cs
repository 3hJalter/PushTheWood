using System;
using System.Collections.Generic;
using _Game.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class RewardPopup : UICanvas
    {
        [SerializeField]
        private RewardItem _rewardItemPrefab;
        [SerializeField]
        private ScrollRect _rewardScrollRect;
        [SerializeField]
        private RectTransform _rewardContentRectTF;
        [SerializeField]
        private Button _collectButton;
        [SerializeField]
        private Button _claimX2Button;

        private List<RewardItem> _rewardItemList = new List<RewardItem>();

        private void Awake()
        {
            _collectButton.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Collect rewards");
                for (int i = 0; i < _rewardItemList.Count; i++)
                {
                    _rewardItemList[i].Reward.Obtain();
                }
                
                Close();
            });
            _claimX2Button.onClick.AddListener(() =>
            {
                DevLog.Log(DevId.Vinh, "Claim X2 rewards");
            });
        }

        public void Open(Reward[] rewards)
        {
            if (rewards != null && rewards.Length > 0)
            {
                // Adjust rewards parent
                if (rewards.Length < 3)
                {
                    _rewardScrollRect.enabled = false;
                    _rewardContentRectTF.anchorMin = new Vector2(0.5f, _rewardContentRectTF.anchorMin.y);
                    _rewardContentRectTF.anchorMax = new Vector2(0.5f, _rewardContentRectTF.anchorMax.y);
                    _rewardContentRectTF.pivot = new Vector2(0.5f, 0.5f);
                    _rewardContentRectTF.anchoredPosition = Vector2.zero;
                }
                else
                {
                    _rewardScrollRect.enabled = true;
                    _rewardContentRectTF.anchorMin = new Vector2(0f, _rewardContentRectTF.anchorMin.y);
                    _rewardContentRectTF.anchorMax = new Vector2(0f, _rewardContentRectTF.anchorMax.y);
                    _rewardContentRectTF.pivot = new Vector2(0f, 0.5f);
                    _rewardContentRectTF.anchoredPosition = Vector2.zero;
                }
                
                // Adjust _rewardItemList size
                int differentInSize = rewards.Length - _rewardItemList.Count;
                if (differentInSize > 0)
                {
                    for (int i = 0; i < differentInSize; i++)
                    {
                        RewardItem rewardItem = Instantiate(_rewardItemPrefab, _rewardContentRectTF);
                        
                        _rewardItemList.Add(rewardItem);
                    }
                }
                else if (differentInSize < 0)
                {
                    for (int i = _rewardItemList.Count - 1; i >= _rewardItemList.Count - 1 + differentInSize; i--)
                    {
                        Destroy(_rewardItemList[i].gameObject);
                        
                        _rewardItemList.RemoveAt(i);
                    }
                }
            
                // Initialize reward item
                for (int i = 0; i < rewards.Length; i++)
                {
                    _rewardItemList[i].Initialize(rewards[i]);
                }   
            }
            
            Open();
        }
    }
}
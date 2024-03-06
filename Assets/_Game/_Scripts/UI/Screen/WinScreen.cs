using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using AudioEnum;
using DG.Tweening;
using System;
using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class WinScreen : UICanvas
    {
        [SerializeField]
        private GameObject _container;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private Image _contentImage;
        [SerializeField]
        private RectTransform _rewardParentRectTF;
        [SerializeField]
        private RewardItem _rewardItemPrefab;
        [SerializeField]
        private Button _nextLevelButton;
        
        private Action _nextLevel;
        private List<RewardItem> _rewardItemList = new List<RewardItem>();
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            if (param is true)
            {
                _canvasGroup.alpha = 1f;
                _blockPanel.gameObject.SetActive(false);
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _blockPanel.gameObject.SetActive(true);
            }
            
            if (param is Reward[] rewards && rewards.Length > 0)
            {
                // Adjust _rewardItemList size
                int differentInSize = rewards.Length - _rewardItemList.Count;
                if (differentInSize > 0)
                {
                    for (int i = 0; i < differentInSize; i++)
                    {
                        RewardItem rewardItem = Instantiate(_rewardItemPrefab, _rewardParentRectTF);
                        
                        _rewardItemList.Add(rewardItem);
                    }
                }
                else if (differentInSize < 0)
                {
                    int startIndex = _rewardItemList.Count - 1;
                    for (int i = startIndex; i > startIndex + differentInSize; i--)
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
            
            // Hide the next level button if the current level is not Normal level
            Level level = LevelManager.Ins.CurrentLevel;
            if (level.LevelType != LevelType.Normal)
            {
                _nextLevelButton.gameObject.SetActive(false);
            }
            else
            {
                if (level.Index == DataManager.Ins.ConfigData.unlockBonusChestAtLevelIndex 
                    || level.Index == DataManager.Ins.ConfigData.unlockDailyChallengeAtLevelIndex
                    || level.Index == DataManager.Ins.ConfigData.unlockSecretLevelAtLevelIndex)
                {
                    _nextLevelButton.gameObject.SetActive(false);
                }
                else
                {
                    _nextLevelButton.gameObject.SetActive(true);
                }
            }
            _nextLevel = () =>
            {
                LevelManager.Ins.OnNextLevel(LevelType.Normal);
                Close();
            };
            
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            _container.SetActive(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                _container.SetActive(true);
                AudioManager.Ins.PlaySfx(SfxType.Win);
                
                if (param is not true)
                {
                    DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                        .OnComplete(() => _blockPanel.gameObject.SetActive(false));
                }
            });
        }
        
        public void OnClickNextButton()
        {           
            DevLog.Log(DevId.Vinh, "Collect rewards");
            for (int i = 0; i < _rewardItemList.Count; i++)
            {
                _rewardItemList[i].Reward.Obtain(_rewardItemList[i].IconImagePosition);
            }
            
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnCheckShowInterAds, _nextLevel);   
        }
        
        public void OnClickMainMenuButton()
        {
            DevLog.Log(DevId.Vinh, "Collect rewards");
            for (int i = 0; i < _rewardItemList.Count; i++)
            {
                _rewardItemList[i].Reward.Obtain(_rewardItemList[i].IconImagePosition);
            }
            
            LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex, false);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnCheckShowInterAds, null);
        }
    }
}

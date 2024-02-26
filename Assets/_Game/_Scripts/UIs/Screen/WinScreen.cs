using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using AudioEnum;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class WinScreen : UICanvas
    {
        [SerializeField] private GameObject container;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private Button _nextLevelButton;
        Action NextLevel;
        public override void Setup(object param = null)
        {
            base.Setup(param);
            _canvasGroup.alpha = 0f;
            // Hide the next level button if the current level is not Normal level
            _nextLevelButton.gameObject.SetActive(LevelManager.Ins.CurrentLevel.LevelType == LevelType.Normal);
            NextLevel = () =>
            {
                LevelManager.Ins.OnNextLevel(LevelType.Normal);
                Close();
            };
            
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            container.SetActive(false);
            DOVirtual.DelayedCall(1f, () =>
            {
                container.SetActive(true);
                AudioManager.Ins.PlaySfx(SfxType.Win);
                DOVirtual.Float(0f, 1f, 0.25f, value => _canvasGroup.alpha = value)
                    .OnComplete(() => _blockPanel.gameObject.SetActive(false));
            });
        }
        
        public void OnClickNextButton()
        {           
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnCheckShowInterAds, NextLevel);             
        }
        
        public void OnClickMainMenuButton()
        {
            LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
            GameManager.Ins.PostEvent(DesignPattern.EventID.OnCheckShowInterAds, null);
        }
    }
}

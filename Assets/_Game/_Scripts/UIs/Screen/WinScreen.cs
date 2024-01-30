﻿using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class WinScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private HButton nextLevelButton;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            _canvasGroup.alpha = 0f;
            // Hide the next level button if the current level is not Normal level
            nextLevelButton.gameObject.SetActive(LevelManager.Ins.CurrentLevel.LevelType == LevelType.Normal);
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            
            DOVirtual.Float(0f, 1f, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(() => _blockPanel.gameObject.SetActive(false));
        }
        
        public void OnClickNextButton()
        {
            LevelManager.Ins.OnNextLevel(LevelType.Normal);
            Close();
        }
        
        public void OnClickMainMenuButton()
        {
            LevelManager.Ins.OnNextLevel(LevelType.Normal, false);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
        }
    }
}

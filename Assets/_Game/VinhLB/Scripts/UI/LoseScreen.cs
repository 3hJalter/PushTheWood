using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
using _Game.Managers;
using _Game.DesignPattern;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VinhLB
{
    public class LoseScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;

        public override void Open()
        {
            base.Open();
            
            DOVirtual.Float(0, 1, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(() => _blockPanel.gameObject.SetActive(false));
        }

        public void OnClickRestartButton()
        {
            LevelManager.Ins.OnRestart();
            GameManager.Ins.PostEvent(EventID.StartGame);
            Close();
        }

        public void OnClickMoreTimeButton()
        {
            GameplayManager.Ins.OnResetTime();
            Close();
        }
    }
}
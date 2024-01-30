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
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            
            _canvasGroup.alpha = 0f;
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            
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
            GameManager.Ins.PostEvent(EventID.StartGame);
            Close();
        }
    }
}
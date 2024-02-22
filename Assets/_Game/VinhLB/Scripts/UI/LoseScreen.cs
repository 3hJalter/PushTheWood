using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
using _Game.Managers;
using _Game.DesignPattern;
using AudioEnum;
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
        [SerializeField] 
        private HButton moreTimeButton;
        public override void Setup(object param = null)
        {
            base.Setup(param);
            _canvasGroup.alpha = 0f;
            // cover param to bool
            bool isTimeOut = param != null && (bool) param;
            moreTimeButton.gameObject.SetActive(isTimeOut);
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            AudioManager.Ins.PlaySfx(SfxType.Lose);
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
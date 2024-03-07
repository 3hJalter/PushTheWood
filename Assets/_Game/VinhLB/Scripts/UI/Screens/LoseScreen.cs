using System.Collections;
using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.DesignPattern;
using _Game.UIs.Screen;
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
        private Image _contentImage;
        [SerializeField]
        private HButton _moreTimeButton;

        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeLayoutForBanner, ChangeLayoutForBanner);
            ChangeLayoutForBanner(AdsManager.Ins.BannerAds.IsBannerOpen);
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.OnChangeLayoutForBanner, ChangeLayoutForBanner);
        }
        
        private void ChangeLayoutForBanner(object isBannerActive)
        {
            int sizeAnchor = (bool)isBannerActive ? DataManager.Ins.ConfigData.bannerHeight : 0;
            MRectTransform.offsetMin = new Vector2(MRectTransform.offsetMin.x, sizeAnchor);
        }
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            _canvasGroup.alpha = 0f;
            // convert param to LevelLoseCondition
            LevelLoseCondition loseCondition = LevelLoseCondition.Timeout; // default
            if (param is LevelLoseCondition lC)
            {
                loseCondition = lC;
            }
            _contentImage.sprite = DataManager.Ins.UIResourceDatabase.LoseScreenResourceConfigDict[loseCondition].IconSprite;
            _moreTimeButton.gameObject.SetActive(loseCondition == LevelLoseCondition.Timeout);
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            AudioManager.Ins.PlaySfx(SfxType.Lose);
            DOVirtual.Float(0, 1, 0.25f, value => _canvasGroup.alpha = value)
                .OnComplete(() => _blockPanel.gameObject.SetActive(false));
        }
        
        public void OnClickMoreTimeButton()
        {
            GameplayManager.Ins.OnResetTime();
            GameManager.Ins.PostEvent(EventID.MoreTimeGame);
            Close();
        }

        public void OnClickRestartButton()
        {
            GameManager.Ins.PostEvent(EventID.StartGame);
            LevelManager.Ins.OnRestart();
            GameManager.Ins.PostEvent(EventID.OnInterAdsStepCount, 1);
            Close();
        }

        public void OnClickMainMenuButton()
        {
            LevelManager.Ins.OnGoLevel(LevelType.Normal, LevelManager.Ins.NormalLevelIndex, false);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<MainMenuScreen>();
            GameManager.Ins.PostEvent(EventID.OnInterAdsStepCount, 1);
        }

    }
}
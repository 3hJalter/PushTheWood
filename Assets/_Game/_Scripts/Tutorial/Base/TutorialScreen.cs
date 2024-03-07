using System;
using _Game._Scripts.Managers;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;

namespace _Game._Scripts.Tutorial
{
    public class TutorialScreen : UICanvas
    {
        private void Awake()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.OnChangeLayoutForBanner, ChangeLayoutForBanner);
            ChangeLayoutForBanner(AdsManager.Ins.IsBannerOpen);
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
            // if InInGameScreen is open, hide it
            if (UIManager.Ins.IsOpened<InGameScreen>())
            {
                UIManager.Ins.CloseUI<InGameScreen>();
                // Stop timer
                if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnPauseGame();
            }
            TutorialManager.Ins.currentTutorialScreenScreen = this;
        }

        public override void CloseDirectly(object param = null)
        {
            // Show InGameScreen
            TutorialManager.Ins.currentTutorialScreenScreen = null;
            if (!UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.OpenUI<InGameScreen>(param);
            if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnUnPause();
            // Close and destroy this screen
            base.CloseDirectly();
        }

        public override void Close()
        {
            TutorialManager.Ins.currentTutorialScreenScreen = null;
            base.Close();
        }
    }
}

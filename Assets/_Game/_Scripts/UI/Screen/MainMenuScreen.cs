using System.Globalization;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Popup;
using AudioEnum;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _blockPanel;
        [SerializeField]
        private TabGroup _bottomNavigationTabGroup;

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
        }

        public override void Open(object param = null)
        {
            base.Open(param);
            // CameraFollow.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            // FxManager.Ins.PlayTweenFog();
            DebugManager.Ins?.OpenDebugCanvas(UI_POSITION.MAIN_MENU);
            GameManager.Ins.ChangeState(GameState.MainMenu);
            CameraManager.Ins.ChangeCamera(ECameraType.PerspectiveCamera);
            CameraManager.Ins.ChangeCameraTargetPosition(LevelManager.Ins.player.transform.position);
            AudioManager.Ins.PlayBgm(BgmType.MainMenu, 1f);
            AudioManager.Ins.StopEnvironment();
            UIManager.Ins.OpenUI<StatusBarScreen>(param);
            if (param is true)
            {
                _bottomNavigationTabGroup.ResetSelectedTab(false);
            }
            else
            {
                DOVirtual.Float(0f, 1f, 1f, value => _canvasGroup.alpha = value)
                    .OnComplete(() => _blockPanel.gameObject.SetActive(false));

                _bottomNavigationTabGroup.ResetSelectedTab(true);
            }
        }

        public override void Close()
        {
            base.Close();

            _bottomNavigationTabGroup.ClearSelectedTab();
        }

        public void OnClickStart()
        {
            UIManager.Ins.OpenUI<InGameScreen>();
            LevelManager.Ins.InitLevel();
            Close();
        }

        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingsPopup>();
        }
    }
}
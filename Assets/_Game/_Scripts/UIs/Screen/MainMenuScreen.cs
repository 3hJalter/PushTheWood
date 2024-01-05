using _Game.Camera;
using _Game.Managers;
using _Game.UIs.Popup;
using DG.Tweening;
using UnityEngine;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private TabGroup _bottomNavigationTabGroup;
        
        private bool _isFirstOpen;

        public override void Open()
        {
            base.Open();
            // CameraFollow.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            // FxManager.Ins.PlayTweenFog();
            GameManager.Ins.ChangeState(GameState.MainMenu);
            CameraManager.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            _bottomNavigationTabGroup.ResetSelectedTab();
            DOVirtual.Float(0, 1, 1f, value => _canvasGroup.alpha = value);
        }

        public void OnClickStart()
        {
            // if (!_isFirstOpen)
            // {
            //     LevelManager.Ins.OnInit();
            //     _isFirstOpen = true;
            // }

            UIManager.Ins.OpenUI<InGameScreen>();
            // FxManager.Ins.StopTweenFog();
            Close();
        }
        
        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingsPopup>();
        }
    }
}

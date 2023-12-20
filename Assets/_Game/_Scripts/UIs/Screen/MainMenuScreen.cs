using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using DG.Tweening;
using UnityEngine;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField] private CanvasGroup canvasGroup;
        private bool _isFirstOpen = false;
        public override void Open()
        {
            base.Open();
            // CameraFollow.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            // FxManager.Ins.PlayTweenFog();
            CameraManager.Ins.ChangeCamera(ECameraType.MainMenuCamera);
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value);
        }

        public void OnClickStart()
        {
            if (!_isFirstOpen)
            {
                LevelManager.Ins.OnInit();
                GridBuildingManager.Ins.OnInit();
                _isFirstOpen = true;
            }
            UIManager.Ins.OpenUI<InGameScreen>();
            // FxManager.Ins.StopTweenFog();
            Close();
        }
    }
}

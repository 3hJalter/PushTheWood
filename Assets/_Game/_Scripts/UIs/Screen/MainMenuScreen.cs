using _Game.Managers;
using DG.Tweening;
using UnityEngine;
using CameraType = _Game.Managers.CameraType;

namespace _Game.UIs.Screen
{
    public class MainMenuScreen : UICanvas
    {
        [SerializeField] private CanvasGroup canvasGroup;
        public override void Open()
        {
            base.Open();
            CameraManager.Ins.ChangeCamera(CameraType.MainMenuCamera);
            FxManager.Ins.PlayTweenFog();
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value);
        }

        public void OnClickStart()
        {
            UIManager.Ins.OpenUI<InGameScreen>();
            Close();
        }
    }
}

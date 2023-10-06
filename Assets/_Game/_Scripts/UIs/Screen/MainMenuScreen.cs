using _Game._Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using CameraType = _Game._Scripts.Managers.CameraType;

namespace _Game._Scripts.UIs.Screen
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

        public override void Close()
        {
            FxManager.Ins.StopTweenFog();
            base.Close();
        }

        public void OnClickStart()
        {
            UIManager.Ins.OpenUI<InGameScreen>();
            Close();
        }
    }
}

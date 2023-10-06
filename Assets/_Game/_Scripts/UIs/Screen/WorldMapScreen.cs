using _Game._Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using CameraType = _Game._Scripts.Managers.CameraType;

namespace _Game._Scripts.UIs.Screen
{
    public class WorldMapScreen : UICanvas
    {
        [SerializeField] private CanvasGroup canvasGroup;
        public override void Open()
        {
            base.Open();
            CameraManager.Ins.ChangeCamera(CameraType.WorldMapCamera);
            FxManager.Ins.PlayTweenFog(false, 1.5f, 3f);
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value);

        }

        public void OnClickCloseButton()
        {
            Close();
            FxManager.Ins.StopTweenFog(3f);
            UIManager.Ins.OpenUI<InGameScreen>();
        }
    }
}

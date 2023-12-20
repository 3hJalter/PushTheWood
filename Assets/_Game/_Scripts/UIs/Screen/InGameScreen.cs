using _Game._Scripts.Managers;
using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        [SerializeField] private Image blockPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        public override void Setup()
        {
            base.Setup();
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.Ins
                .CurrentChoice); // TODO: Change to use PlayerRef 
            // if (CameraFollow.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            // CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
            if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            blockPanel.enabled = true;
        }

        public override void Open()
        {
            base.Open();
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value)
                .OnComplete(() => { blockPanel.enabled = false; });
        }

        public override void Close()
        {
            MoveInputManager.Ins.HideButton();
            base.Close();
        }

        public void OnClickSettingButton()
        {
            UIManager.Ins.OpenUI<SettingPopup>();
        }

        public void OnClickOpenMapButton()
        {
            Close();
            UIManager.Ins.OpenUI<WorldMapScreen>();
        }

        public void OnClickResetIslandButton()
        {
            LevelManager.Ins.ResetIslandPlayerOn();
        }

        public void OnClickToggleBuildingMode()
        {
            Close();
            UIManager.Ins.OpenUI<BuildingScreen>();

            GridBuildingManager.Ins.ToggleBuildMode();
        }
    }
}

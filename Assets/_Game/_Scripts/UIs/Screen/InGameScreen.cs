using _Game._Scripts.UIs.Tutorial;
using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using DG.Tweening;
using HControls;
using UnityEngine;
using UnityEngine.UI;
using VinhLB;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        [SerializeField] private HSwitch hSwitch;
        [SerializeField] private HDpad dpad;

        [SerializeField] private Image blockPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        public HSwitch HSwitch => hSwitch;
        public GameObject DpadObj => dpad.gameObject;

        private bool _isTutOpen;
        public override void Setup()
        {
            _isTutOpen = UIManager.Ins.IsOpened<TutorialScreen>();
            base.Setup();
            if (CameraFollow.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
            blockPanel.enabled = true;
        }

        public override void Open()
        {   
            if (_isTutOpen)
            {
                Close();
                return;
            }
            base.Open();
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value)
                .OnComplete(() =>
                {
                    blockPanel.enabled = false;
                });
        }

        public override void Close()
        {
            HInputManager.SetDefault();
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

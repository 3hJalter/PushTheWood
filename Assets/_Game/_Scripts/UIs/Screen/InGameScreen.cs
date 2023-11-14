using _Game.Camera;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using CnControls;
using DG.Tweening;
using HControls;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        
        [SerializeField] private HSwitch hSwitch;
        [SerializeField] private GameObject dpad;
        public HSwitch HSwitch => hSwitch;
        public GameObject Dpad => dpad;

        [SerializeField] private Image blockPanel;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public override void Setup()
        {
            base.Setup();
            // if (hSwitch.enabled) hSwitch.ResetSwitchPos();
            if (CameraFollow.Ins.IsCurrentCameraIs(ECameraType.InGameCamera)) return;
            FxManager.Ins.StopTweenFog();
            blockPanel.enabled = true;
        }

        public override void Open()
        {
            base.Open();
            CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
            DOVirtual.Float(0, 1, 1f, value => canvasGroup.alpha = value)
                .OnComplete(() =>
                {
                    blockPanel.enabled = false;
                    // TESTING
                    if (!UIManager.Ins.IsLoaded<TempTutorialScreen>()) UIManager.Ins.OpenUI<TempTutorialScreen>();
                    // END OF TESTING
                });
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
        
        // TEST
        public void OnClickMoveOptionPopup()
        {
            UIManager.Ins.OpenUI<MoveOptionPopup>();
        }
    }
}

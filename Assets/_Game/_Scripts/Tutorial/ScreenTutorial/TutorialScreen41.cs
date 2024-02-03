using System;
using _Game.Camera;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities.Timer;
using HControls;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen41 : TutorialScreen
    {
        [SerializeField] private Image panel;
        [SerializeField] private HHoldingButton holdingButton;
        [SerializeField] private GameObject deco;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            SetupHoldAction();
            panel.raycastTarget = false;
            TimerManager.Inst.WaitForTime(1.5f, () =>
            {
                if (UIManager.Ins.IsOpened<InGameScreen>())
                {
                    UIManager.Ins.CloseUI<InGameScreen>();
                    // Stop timer
                    if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnPauseGame();
                }
                panel.raycastTarget = true;
                panel.color = new Color(0,0,0,0.65f);
                deco.SetActive(true);
            });
        }

        private void SetupHoldAction()
        {
            
            holdingButton.OnInit(Constants.HOLD_TOUCH_TIME, null, PointerUpAction, HoldAction);
            return;

            void PointerUpAction()
            {
                if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.ZoomOutCamera)) CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera, Constants.ZOOM_OUT_TIME);
                CloseDirectly();
            }

            // Set up holding button
            void HoldAction()
            {
                if (!holdingButton.IsHolding) return;
                CameraManager.Ins.ChangeCamera(ECameraType.ZoomOutCamera, Constants.ZOOM_OUT_TIME);
                panel.color = new Color(1, 1, 1, 0);
                deco.SetActive(false);
            }
        }
    }
}

using System;
using _Game.Camera;
using _Game.Managers;
using UnityEngine;

namespace HControls
{
    public class HDpad : HMonoBehaviour
    {
        [Tooltip("0 is Left, 1 is Right, 2 is Up, 3 is Down")]
        [SerializeField]
        private bool highlightButton = true;

        [SerializeField] private HHoldingButton holdingButton;
        
        
        [SerializeField] private HDpadButton[] dpadButtons;


        private void Awake()
        {
            holdingButton.OnInit(Constants.HOLD_TOUCH_TIME, null, PointerUpAction, HoldAction);
        }
        
        void PointerUpAction()
        {
            if (CameraManager.Ins.IsCurrentCameraIs(ECameraType.ZoomOutCamera)) CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera, Constants.ZOOM_OUT_TIME);
            HInputManager.LockInput(false);
        }

        // Set up holding button
        void HoldAction()
        {
            if (!holdingButton.IsHolding) return;
            HInputManager.LockInput();
            CameraManager.Ins.ChangeCamera(ECameraType.ZoomOutCamera, Constants.ZOOM_OUT_TIME);
        }

        private void OnDisable()
        {
            for (int i = 0; i < dpadButtons.Length; i++) dpadButtons[i].PointerDownImg.SetActive(false);
            HInputManager.SetDirectionInput(Direction.None);
        }

        public void OnButtonPointerDown(int index)
        {
            if (highlightButton) dpadButtons[index].PointerDownImg.SetActive(true);
            HInputManager.SetDirectionInput(dpadButtons[index].Direction);
        }

        public void OnButtonPointerUp(int index)
        {
            dpadButtons[index].PointerDownImg.SetActive(false);
            HInputManager.SetDirectionInput(Direction.None);
        }

    }
}

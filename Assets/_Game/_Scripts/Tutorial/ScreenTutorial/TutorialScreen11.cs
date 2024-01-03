using _Game._Scripts.Managers;
using _Game.Utilities;
using HControls;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen11 : TutorialScreen
    {
        public void OnClickMoveForward()
        {
            CloseAndDestroy();
            DevLog.Log(DevId.Hoang, "Set direction input: " + HInputManager.GetDirectionInput());
            // HInputManager.SetDirectionInput(Direction.Forward);
            MoveInputManager.Ins.Dpad.OnButtonPointerDown((int) Direction.Forward);
            // Wait A frame, then OnButtonPointerUp
            // StartCoroutine(OnButtonPointerUpAfterAFrame());
        } 
    }
}

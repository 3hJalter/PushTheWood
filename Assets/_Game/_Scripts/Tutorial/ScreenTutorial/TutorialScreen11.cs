using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game.Utilities;
using HControls;
using MEC;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen11 : TutorialScreen
    {
        public void OnClickMoveForward()
        {
            CloseDirectly();
            DevLog.Log(DevId.Hoang, "Set direction input: " + HInputManager.GetDirectionInput());
            // HInputManager.SetDirectionInput(Direction.Forward);
            MoveInputManager.Ins.Dpad.OnButtonPointerDown((int) Direction.Forward);
            // Wait A frame, then OnButtonPointerUp
            Timing.RunCoroutine(WaitAFixedUpdate(() => MoveInputManager.Ins.Dpad.OnButtonPointerUp((int) Direction.Forward)));
        }

        private static IEnumerator<float> WaitAFixedUpdate(Action callback)
        {
            // wait for one fixed update 
            yield return Time.fixedDeltaTime;
            DevLog.Log(DevId.Hoang, "Reset direction input");
            callback?.Invoke();
        }
    }
}

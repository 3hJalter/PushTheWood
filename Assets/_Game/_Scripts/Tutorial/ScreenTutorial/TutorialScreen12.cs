using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game.Utilities;
using HControls;
using MEC;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen12 : TutorialScreen
    {
        public override void CloseDirectly()
        {
            MoveInputManager.Ins.WaitToForceResetMove(Time.fixedDeltaTime * 2);
            base.CloseDirectly();
        }
    }
}

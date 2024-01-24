using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using HControls;
using MEC;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen12 : TutorialScreen
    {
        [SerializeField] private Image panel;
        [SerializeField] private GameObject deco;
         
        public override void CloseDirectly()
        {
            // MoveInputManager.Ins.WaitToForceResetMove(Time.fixedDeltaTime * 2);
            base.CloseDirectly();
        }
        
        public void OpenInGameScreen()
        {
            if (!UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.OpenUI<InGameScreen>();
            panel.enabled = false;
            deco.SetActive(false);
        }
    }
}

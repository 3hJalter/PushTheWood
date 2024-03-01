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
    public class TutorialScreen31 : TutorialScreen
    {
        [SerializeField] private HButton undoButton;
        [SerializeField] private HButton resetButton;

        public void OnClickUndo()
        {
            GameplayManager.Ins.OnFreeUndo();
            CloseDirectly();
        }

        public void OnClickReset()
        {
            GameplayManager.Ins.OnFreeResetIsland(false);
            CloseDirectly();
        }
        
    }
}

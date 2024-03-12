﻿using System;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities.Timer;
using HControls;
using TMPro;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class GameTutorialScreen51 : GameTutorialScreen
    {
        [SerializeField] private GameObject panelContainer;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            HInputManager.LockInput();
            panelContainer.SetActive(false);
            TimerManager.Ins.WaitForFrame(1, () =>
            {
                if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnPauseGame();
                if (!UIManager.Ins.IsOpened<InGameScreen>()) return;
                UIManager.Ins.CloseUI<InGameScreen>();
                // Stop timer
            });
            TimerManager.Ins.WaitForTime(1f, () =>
            {
                panelContainer.SetActive(true);
                HInputManager.LockInput(false);
            });
        }

        public void OnClickGrowTree()
        {
            GameplayManager.Ins.OnFreeGrowTree();
            CloseDirectly();
        }
    }
}
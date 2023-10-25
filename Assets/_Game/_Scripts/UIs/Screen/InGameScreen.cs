﻿using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Popup;
using CnControls;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using CameraType = _Game.Managers.CameraType;

namespace _Game.UIs.Screen
{
    public class InGameScreen : UICanvas
    {
        [SerializeField] private SimpleJoystick joystick;
        [SerializeField] private Image blockPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        public override void Setup()
        {
            base.Setup();
            joystick.ResetJoyStickPos();
            if (CameraManager.Ins.IsCurrentCameraIs(CameraType.InGameCamera)) return;
            FxManager.Ins.StopTweenFog();
            blockPanel.enabled = true;
        }

        public override void Open()
        {
            base.Open();
            CameraManager.Ins.ChangeCamera(CameraType.InGameCamera);
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
    }
}

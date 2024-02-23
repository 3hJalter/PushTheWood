﻿using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game.Utilities;
using GG.Infrastructure.Utils.Swipe;
using HControls;
using MEC;
using UnityEngine;
using UnityEngine.Events;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen12 : TutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        private UnityAction<string> _testSwipeEvent;
        [SerializeField] private float delay = 2f;

        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            Timing.RunCoroutine(DelayAction(() =>
            {
                MoveInputManager.Ins.ShowContainer(true);
                // Add Close this screen when swipe
                _testSwipeEvent = _ =>
                {
                    DevLog.Log(DevId.Hoang, "Test swipe");
                };
                _swipeEvent = direction =>
                {
                    DevLog.Log(DevId.Hoang, $"Swipe - {direction}");
                    if (direction != DirectionId.ID_UP) return;
                    HInputManager.SetDirectionInput(Direction.Forward);
                    CloseDirectly(false);
                };
                // Remove the default swipe event
                MoveInputManager.Ins.HSwipe.RemoveListener();
                MoveInputManager.Ins.HSwipe.AddListener(_swipeEvent);
                MoveInputManager.Ins.HSwipe.AddListener(_testSwipeEvent);
            }));
        }

        public override void CloseDirectly(object param = null)
        {
            // Remove Close this screen when swipe 
            MoveInputManager.Ins.HSwipe.RemoveListener(_swipeEvent);
            MoveInputManager.Ins.HSwipe.RemoveListener(_testSwipeEvent);
            // Add the default swipe event
            MoveInputManager.Ins.HSwipe.AddListener();
            base.CloseDirectly(param);
        }
        
        private IEnumerator<float> DelayAction(Action callback)
        {
            yield return Timing.WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}

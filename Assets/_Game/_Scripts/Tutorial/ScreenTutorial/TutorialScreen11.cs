using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using MEC;
using UnityEngine;
using UnityEngine.Events;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen11 : TutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        [SerializeField] private float delay = 2f;

        public override void Setup(object param = null)
        {
            base.Setup(param);
            Timing.RunCoroutine(DelayAction(() =>
            {
                MoveInputManager.Ins.ShowContainer(true);
                // Add Close this screen when swipe
                _swipeEvent = _ =>
                {
                    CloseDirectly(false);
                };
                MoveInputManager.Ins.HSwipe.AddListener(_swipeEvent);
            }));
        }

        public override void CloseDirectly(object param = null)
        {
            // Remove Close this screen when swipe 
            MoveInputManager.Ins.HSwipe.RemoveListener(_swipeEvent);
            base.CloseDirectly(param);
        }
        
        private IEnumerator<float> DelayAction(Action callback)
        {
            yield return Timing.WaitForSeconds(delay);
            callback?.Invoke();
        }
    }
}

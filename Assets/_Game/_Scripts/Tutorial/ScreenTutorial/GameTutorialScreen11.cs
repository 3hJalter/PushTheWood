using System;
using System.Collections.Generic;
using _Game._Scripts.Managers;
using _Game.Managers;
using _Game.UIs.Screen;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class GameTutorialScreen11 : GameTutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        [SerializeField] private float moveTime = 1f;
        [SerializeField] private RectTransform _imageRectContainer;
        [SerializeField] private RectTransform _finalImageRectPosition;
        [SerializeField] private Image panel;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            MoveInputManager.Ins.ShowContainer(true);
            // Add Close this screen when swipe
            _swipeEvent = _ =>
            {
                // FXManager.Ins.TrailHint.OnPlay(new List<Vector3>()
                // {
                //     new(7,3,7),
                //     new(7,3,13),
                // }, 8f, true);
                AfterSwipe();
            };
            MoveInputManager.Ins.HSwipe.AddListener(_swipeEvent);
        }

        private void AfterSwipe()
        {
            // Set panel alpha to 0
            panel.color = new Color(1, 1, 1, 0);
            // Tween to move _imageRectContainer to _finalImageRectPosition
            TutorialManager.Ins.currentGameTutorialScreenScreen = null;
            UIManager.Ins.OpenUI<InGameScreen>(false);
            GameplayManager.Ins.OnFreePushHint(false, true);
            Timing.RunCoroutine(MoveImageRect().CancelWith(gameObject));
            MoveInputManager.Ins.HSwipe.RemoveListener(_swipeEvent);
        }

        private IEnumerator<float> MoveImageRect()
        {
            float time = 0;
            while (time < moveTime)
            {
                time += Time.deltaTime; 
                _imageRectContainer.anchoredPosition = Vector2.Lerp(_imageRectContainer.anchoredPosition, _finalImageRectPosition.anchoredPosition, time / moveTime);
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}

using _Game._Scripts.Managers;
using _Game.Managers;
using _Game.Utilities;
using GG.Infrastructure.Utils.Swipe;
using HControls;
using UnityEngine;
using UnityEngine.Events;
using VinhLB;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class GameTutorialScreen12 : GameTutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        private UnityAction<string> _testSwipeEvent;
        [SerializeField] private RectTransform mask;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);

            MoveInputManager.Ins.ShowContainer(true);
            // Add Close this screen when swipe
            _testSwipeEvent = _ => { DevLog.Log(DevId.Hoang, "Test swipe"); };
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
            // OpenMask();
        }

        private void OpenMask()
        {
            MaskData maskData = new()
            {
                Position = mask.position,
                Size = mask.sizeDelta + Vector2.one * 40f,
                MaskType = MaskType.Eclipse,
                ClickableItem = null,
                OnClickedCallback = () => UIManager.Ins.CloseUI<MaskScreen>(),
                // If you want to make mask fit and follow target
                //TargetRectTF = rectTransform
            };
            UIManager.Ins.OpenUI<MaskScreen>(maskData);
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
    }
}

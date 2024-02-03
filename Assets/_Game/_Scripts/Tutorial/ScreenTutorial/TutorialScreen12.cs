using _Game._Scripts.Managers;
using _Game.Utilities;
using GG.Infrastructure.Utils.Swipe;
using HControls;
using UnityEngine.Events;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen12 : TutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        private UnityAction<string> _testSwipeEvent;
        public override void Setup(object param = null)
        {
            base.Setup(param);
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

        // [SerializeField] private Image panel;
        // [SerializeField] private GameObject deco;
        // [SerializeField] private HDpad dpad;
        // [SerializeField] private HDpadButton dpadButtonLeft;
        // [SerializeField] private HDpadButton dpadButtonRight;
        // [SerializeField] private HDpadButton dpadButtonUp;
        // [SerializeField] private HDpadButton dpadButtonDown;
        //
        // public override void Open(object param = null)
        // {
        //     base.Open(param);
        //     Timing.RunCoroutine(WaitOneFrame(() =>
        //     {
        //         panel.enabled = true;
        //         deco.SetActive(true);
        //         dpadButtonLeft.PointerDownImg.SetActive(true);
        //         dpadButtonRight.PointerDownImg.SetActive(true);
        //         dpadButtonUp.PointerDownImg.SetActive(true);
        //         dpadButtonDown.PointerDownImg.SetActive(true);
        //     }));
        // }
        //
        // public override void CloseDirectly(object param = null)
        // {
        //     // MoveInputManager.Ins.WaitToForceResetMove(Time.fixedDeltaTime * 2);
        //     base.CloseDirectly(param);
        // }
        //
        // private IEnumerator<float> WaitOneFrame(Action action)
        // {
        //     yield return Timing.WaitForOneFrame;
        //     action?.Invoke();
        // }
        //
        // public void OpenInGameScreen()
        // {
        //     if (!UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.OpenUI<InGameScreen>();
        //     panel.enabled = false;
        //     deco.SetActive(false);
        // }
        //
        // public void OnClickUpButton()
        // {
        //     dpadButtonRight.PointerDownImg.SetActive(false);
        //     dpadButtonLeft.PointerDownImg.SetActive(false);
        //     dpadButtonDown.PointerDownImg.SetActive(false);
        // }
    }
}

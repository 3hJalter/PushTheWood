using _Game._Scripts.Managers;
using UnityEngine.Events;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen11 : TutorialScreen
    {
        private UnityAction<string> _swipeEvent;
        public override void Setup(object param = null)
        {
            base.Setup(param);
            MoveInputManager.Ins.ShowContainer(true);
            // Add Close this screen when swipe
            _swipeEvent = _ =>
            {
                CloseDirectly(false);
            };
            MoveInputManager.Ins.HSwipe.AddListener(_swipeEvent);
        }

        public override void CloseDirectly(object param = null)
        {
            // Remove Close this screen when swipe 
            MoveInputManager.Ins.HSwipe.RemoveListener(_swipeEvent);
            base.CloseDirectly(param);
        }
    }
}

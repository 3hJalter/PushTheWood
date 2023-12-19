using _Game._Scripts.Managers;

namespace _Game.UIs.Popup
{
    public class MoveOptionPopup : UICanvas
    {
        
        
        public void OnUseDpad()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.DPad);
        }
        
        public void OnUseSwitch()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.Switch);
        }
    
        public void OnUseSwipe()
        {
            MoveInputManager.Ins.OnChangeMoveChoice(MoveInputManager.MoveChoice.Swipe);
        }
        
    }

}


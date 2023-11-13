public class MoveOptionPopup : UICanvas
{
    public void OnUseDpad()
    {
        MovingOption.Ins.OnChangeMoveChoice(MovingOption.MoveChoice.DPad);
    }
    
    public void OnUseJoyLock()
    {
        MovingOption.Ins.OnChangeMoveChoice(MovingOption.MoveChoice.Switch);
    }

    public void OnUseSwipeMultiple()
    {
        MovingOption.Ins.OnChangeMoveChoice(MovingOption.MoveChoice.Swipe);
    }
}

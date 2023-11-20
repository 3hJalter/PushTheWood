using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;

public class MoveOptionPopup : UICanvas
{
    private enum MoveChoice
    {
        DPad,
        Switch,
        Swipe
    }
    
    public void OnUseDpad()
    {
        OnChangeMoveChoice(MoveChoice.DPad);
    }
    
    public void OnUseSwitch()
    {
        OnChangeMoveChoice(MoveChoice.Switch);
    }

    public void OnUseSwipe()
    {
        OnChangeMoveChoice(MoveChoice.Swipe);
    }
    
    public void OnShowDirectionIcon()
    {
        LevelManager.Ins.OnShowDirectionIcon();
    }
    
    public void OnChangeCellViewerState()
    {
        LevelManager.Ins.ChangeCellViewer();
    }
    
    private static void OnChangeMoveChoice(MoveChoice moveChoice)
    {
        InGameScreen igScreen = UIManager.Ins.GetUI<InGameScreen>();
        if (igScreen == null) return;
        HideButton(igScreen);
        switch (moveChoice)
        {
            case MoveChoice.DPad:
                igScreen.DpadObj.SetActive(true);
                break;
            case MoveChoice.Switch:
            {
                igScreen.HSwitch.gameObject.SetActive(true);
                igScreen.HSwitch.HideAllTime(false);
                break;
            }
            case MoveChoice.Swipe:
            {
                igScreen.HSwitch.gameObject.SetActive(true);
                igScreen.HSwitch.HideAllTime(true);
                break;
            }

        }
    }

    private static void HideButton(InGameScreen igScreen)
    {
        igScreen.DpadObj.SetActive(false);
        igScreen.HSwitch.gameObject.SetActive(false);
    }
}

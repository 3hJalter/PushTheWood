using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;

public class MovingOption : Singleton<MovingOption>
{
    public enum MoveChoice
    {
        DPad,
        Switch,
        Swipe
    }

    public void OnChangeMoveChoice(MoveChoice moveChoice)
    {
        InGameScreen igScreen = UIManager.Ins.GetUI<InGameScreen>();
        if (igScreen == null) return;
        LevelManager.Ins.ChangePlayerMoveChoice(moveChoice);
        HideButton(igScreen);
        switch (moveChoice)
        {
            case MoveChoice.DPad:
                igScreen.Dpad.SetActive(true);
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
        igScreen.Dpad.SetActive(false);
        igScreen.HSwitch.gameObject.SetActive(false);
    }
}

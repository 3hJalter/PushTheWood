using _Game.Managers;

namespace _Game.UIs.Screen
{
    public class WinScreen : UICanvas
    {
        public void OnClickNextButton()
        {
            LevelManager.Ins.GoNextLevel();
            Close();
        }
    }
}

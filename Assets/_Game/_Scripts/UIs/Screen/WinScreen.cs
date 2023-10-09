using _Game._Scripts.Managers;

namespace _Game._Scripts.UIs.Screen
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

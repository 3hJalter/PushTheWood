using _Game._Scripts.Managers;

namespace _Game.UIs.Screen
{
    public class ChooseLevelScreen : UICanvas
    {
        public void SeeNextLevel()
        {
            ChooseLevelManager.Ins.SeeNextLevel();
        }
        
        public void SeePreviousLevel()
        {
            ChooseLevelManager.Ins.SeePreviousLevel();
        }
    }
}

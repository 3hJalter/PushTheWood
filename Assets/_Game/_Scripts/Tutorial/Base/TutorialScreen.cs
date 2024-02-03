using _Game.Managers;
using _Game.UIs.Screen;

namespace _Game._Scripts.Tutorial
{
    public class TutorialScreen : UICanvas
    {
        public override void Setup(object param = null)
        {
            base.Setup(param);
            // if InInGameScreen is open, hide it
            if (UIManager.Ins.IsOpened<InGameScreen>())
            {
                UIManager.Ins.CloseUI<InGameScreen>();
                // Stop timer
                if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnPauseGame();
            }
        }

        public override void CloseDirectly(object param = null)
        {
            // Show InGameScreen
            if (!UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.OpenUI<InGameScreen>(param);
            if (GameManager.Ins.IsState(GameState.InGame)) GameplayManager.Ins.OnUnPause();
            // Close and destroy this screen
            base.CloseDirectly();
        }
    }
}

using _Game.GameGrid;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen91 : TutorialScreen
    {
        public override void CloseDirectly(object param = null)
        {   
            LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(false);
            base.CloseDirectly(param);
        }
    }
}

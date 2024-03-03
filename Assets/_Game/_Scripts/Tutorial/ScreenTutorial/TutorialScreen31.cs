using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen31 : TutorialScreen
    {
        [SerializeField] private HButton undoButton;
        [SerializeField] private HButton resetButton;

        public void OnClickUndo()
        {
            GameplayManager.Ins.OnFreeUndo();
            CloseDirectly();
        }

        public void OnClickReset()
        {
            GameplayManager.Ins.OnFreeResetIsland(false);
            CloseDirectly();
        }
        
    }
}

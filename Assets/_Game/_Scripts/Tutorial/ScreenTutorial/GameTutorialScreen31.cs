using _Game.Managers;
using TMPro;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class GameTutorialScreen31 : GameTutorialScreen
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

using _Game.Managers;
using HControls;
using TMPro;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class GameTutorialScreen31 : GameTutorialScreen
    {
        [SerializeField] private HButton undoButton;
        [SerializeField] private HButton resetButton;

        public override void Setup(object param = null)
        {
            HInputManager.LockInput();
            base.Setup(param);
        }

        public void OnClickUndo()
        {
            GameplayManager.Ins.OnFreeUndo();
            HInputManager.LockInput(false);
            CloseDirectly();
        }

        public void OnClickReset()
        {
            GameplayManager.Ins.OnFreeResetIsland(false);
            HInputManager.LockInput(false);
            CloseDirectly();
        }
        
    }
}

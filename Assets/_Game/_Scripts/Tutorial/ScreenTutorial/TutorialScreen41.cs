using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities.Timer;
using UnityEngine;
using UnityEngine.UI;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class TutorialScreen41 : TutorialScreen
    {
        [SerializeField] private Image panel;
        [SerializeField] private GameObject deco;

        public override void Setup()
        {
            base.Setup();
            panel.raycastTarget = false;
            TimerManager.Inst.WaitForTime(1.5f, () =>
            {
                panel.raycastTarget = true;
                panel.color = new Color(0,0,0,0.65f);
                deco.SetActive(true);
            });
        }

        public void OpenInGameScreen()
        {
            
            if (!UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.OpenUI<InGameScreen>();
            // Change panel alpha to 0
            panel.color = new Color(1,1,1,0);
            deco.SetActive(false);
        }
        
        
    }
}

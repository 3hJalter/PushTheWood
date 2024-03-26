using _Game._Scripts.Managers;
using _Game._Scripts.Tutorial;
using _Game.GameGrid;
using _Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class DebugInGameScreen : UICanvas
    {
        [SerializeField]
        Button onOffInGameUIBtn;
        [SerializeField]
        Button onOffTutorial;
        [SerializeField]
        Button onOffButton;
        [SerializeField]
        GameObject container;

        private void Awake()
        {
            onOffInGameUIBtn.onClick.AddListener(OnOffInGameScreen);
            onOffTutorial.onClick.AddListener(OnOffTutorial);
            onOffButton.onClick.AddListener(OnOffUI);
        }
        private void OnDestroy()
        {
            onOffInGameUIBtn.onClick.RemoveAllListeners();
            onOffTutorial.onClick.RemoveAllListeners();
            onOffButton.onClick.RemoveAllListeners();
        }

        private void OnOffInGameScreen()
        {
            if(UIManager.Ins.IsOpened<InGameScreen>())
                UIManager.Ins.CloseUI<InGameScreen>();
            else
                UIManager.Ins.OpenUI<InGameScreen>();
        }

        private void OnOffUI()
        {
            container.SetActive(!container.activeInHierarchy);
        }

        private void OnOffTutorial()
        {
            BaseTutorialData tut = TutorialManager.Ins.TutorialList[LevelManager.Ins.NormalLevelIndex] as BaseTutorialData;
            tut.CurrentScreen.isDestroyOnClose = false;


            if (tut == null)
                return;

            if (!tut.CurrentScreen.gameObject.activeInHierarchy)
            {
                tut.CurrentScreen.Open();
                GameplayManager.Ins.PushHint.OnStopHint();
                GameplayManager.Ins.OnFreePushHint(false, true);
            }
            else
            {
                tut.CurrentScreen.Close();
                GameplayManager.Ins.PushHintObject.SetActive(false);
            }
        }
        
    }
}
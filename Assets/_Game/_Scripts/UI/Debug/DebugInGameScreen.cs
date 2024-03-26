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
        Button onOffInGameBtn;
        [SerializeField]
        Button onOffButton;
        [SerializeField]
        GameObject container;

        private void Awake()
        {
            onOffInGameBtn.onClick.AddListener(OnOffInGameScreen);
            onOffButton.onClick.AddListener(OnOffUI);
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

        private void OnDestroy()
        {
            onOffInGameBtn.onClick.RemoveAllListeners();
            onOffButton.onClick.RemoveAllListeners();
        }
    }
}
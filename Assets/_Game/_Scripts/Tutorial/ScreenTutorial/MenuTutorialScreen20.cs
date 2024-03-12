using System.Collections.Generic;
using _Game.Data;
using _Game.GameGrid;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VinhLB;

namespace _Game._Scripts.Tutorial.ScreenTutorial
{
    public class MenuTutorialScreen20 : MenuTutorialScreen
    {
        [SerializeField] private Image panel;
        [SerializeField] private HButton secretMapButton;
        [SerializeField] private List<Image> secretMapImage;
        [SerializeField] private Transform lockImage;
        [SerializeField] private Image unlockImage;
        [SerializeField] private GameObject focusObject;
        [SerializeField] private GameObject fakeSecretMapPopup;
        
        public override void Setup(object param = null)
        {
            base.Setup(param);
            panel.color = new Color(0, 0, 0, 0);
            secretMapButton.interactable = false;
            focusObject.SetActive(false);
            fakeSecretMapPopup.SetActive(false);
            unlockImage.gameObject.SetActive(false);
            lockImage.gameObject.SetActive(true);
            if (UIManager.Ins.IsLoaded<HomePage>())
            {
                UIManager.Ins.GetUI<HomePage>().ShowSecretMapButton(false);
            }
            TimerManager.Ins.WaitForTime(1.5f, () =>
            {
                panel.color = new Color(0, 0, 0, 0.8f);
                OnUnlock();
            });
        }

        private void OnUnlock()
        {
            // TODO: Unlock Animation
            // 1. Set an animation which lock image is vibrate 3 times
            lockImage.DOShakePosition(0.5f, 10).SetLoops(2)
                .OnComplete(() =>
                {
                    // 2. Disable lock image, enable unlock image
                    lockImage.gameObject.SetActive(false);
                    unlockImage.gameObject.SetActive(true);
                    // 3. Fade out unlock image
                    unlockImage.DOFade(0, 0.3f).OnComplete(() =>
                    {
                        // 4. Make List<Image> dailyChallengeImage color to 0 0 0 0
                        for (int index = 0; index < secretMapImage.Count; index++)
                        {
                            Image image = secretMapImage[index];
                            image.color = new Color(1, 1, 1, 1);
                        }
                        // 5. Set dailyChallengeButton interactable
                        secretMapButton.interactable = true;
                        // 6. Set focusObject active
                        focusObject.SetActive(true);
                    });
                });
        }

        public void OnClickFakeSecretButton()
        {
            focusObject.SetActive(false);
            // 7. Show a fake DailyChallengePopup
            fakeSecretMapPopup.SetActive(true);
        }

        public void OnClickSecretLevel()
        {
            if (UIManager.Ins.IsLoaded<HomePage>())
            {
                UIManager.Ins.GetUI<HomePage>().ShowSecretMapButton(true);
            }
            CloseDirectly();
            DataManager.Ins.GameData.user.completedMenuTutorial.Add(
                DataManager.Ins.ConfigData.unlockSecretLevelAtLevelIndex);
            LevelManager.Ins.OnGoLevel(LevelType.Secret, 0);
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<InGameScreen>();
        }
    }
}

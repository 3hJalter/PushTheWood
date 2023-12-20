using _Game.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UIs.Screen
{
    public class TempTutorialScreen : UICanvas
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject vidTutorial;
        [SerializeField] private int tutorialDone;
        [SerializeField] private TMP_Text tutorialText;
        [SerializeField] private Image blockPanel;


        private void Start()
        {
            tutorialDone = 0;
        }

        public override void Setup()
        {
            if (UIManager.Ins.IsOpened<InGameScreen>()) UIManager.Ins.CloseUI<InGameScreen>();
            ChangeBlockPanelActiveStatus(true);
            base.Setup();
        }

        public override void Open()
        {
            base.Open();
            RunTutorial();
            DOVirtual.Float(0, 1, 0.5f, value => canvasGroup.alpha = value)
                .OnComplete(() => ChangeBlockPanelActiveStatus(false));
        }

        public override void Close()
        {
            base.Close();
            TapContinue();
            UIManager.Ins.OpenUI<InGameScreen>();
        }

        private void ChangeBlockPanelActiveStatus(bool status)
        {
            blockPanel.enabled = status;
        }

        private void RunTutorial()
        {
            switch (tutorialDone)
            {
                case 0:
                    break;
                case 1:
                    vidTutorial.SetActive(true);
                    tutorialText.text = "Push the log to form a bridge";
                    break;
                case 2:
                    tutorialText.text =
                        "You can step on stump to step on the log. And you can only walk through the log pointing direction";
                    break;
                case 3:
                    tutorialText.text =
                        "Push a log horizontally will make it roll forever. Use rock, stump or others log to stop it";
                    break;
                case 4:
                    tutorialText.text = "Go to the finish point to complete level, or find secret path";
                    break;
                case 5:
                    tutorialText.text = "Merge 2 logs to form a raft";
                    break;
            }
        }

        private void TapContinue()
        {
            if (tutorialDone == 1) vidTutorial.SetActive(false);
            OnTutorialDone();
        }

        private void OnTutorialDone()
        {
            tutorialDone++;
        }
    }
}

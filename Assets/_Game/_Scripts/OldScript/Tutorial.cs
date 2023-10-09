using _Game._Scripts.DesignPattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : Singleton<Tutorial>
{
    // public UIManager uiManager;
    public GameObject tutorialObj;
    public GameObject vidTutorial;
    public int tutorialDone;
    public Button tapContinueButton;
    public TMP_Text tutorialText;

    public void Start()
    {
        tapContinueButton.onClick.AddListener(TapContinue);
        tutorialDone = 0;
        RunTutorial();
    }

    public void ControlTutorial()
    {
        // uiManager.ControlBtns.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.8f).SetLoops(-1, LoopType.Yoyo);
    }

    public void RunTutorial()
    {
        switch (tutorialDone)
        {
            case 0:
                ControlTutorial();
                break;
            case 1:
                tapContinueButton.gameObject.SetActive(true);
                vidTutorial.SetActive(true);
                tutorialText.text = "Push the log to form a bridge";
                break;
            case 2:
                tapContinueButton.gameObject.SetActive(true);
                tutorialText.text =
                    "You can step on stump to step on the log. And you can only walk through the log pointing direction";
                break;
            case 3:
                tapContinueButton.gameObject.SetActive(true);
                tutorialText.text =
                    "Push a log horizontally will make it roll forever. Use rock, stump or others log to stop it";
                break;
            case 4:
                tapContinueButton.gameObject.SetActive(true);
                tutorialText.text = "Go to the finish point to complete level, or find secret path";
                break;
            case 5:
                tapContinueButton.gameObject.SetActive(true);
                tutorialText.text = "Merge 2 logs to form a raft";
                break;
        }
    }

    public void TapContinue()
    {
        tutorialObj.SetActive(false);
        vidTutorial.SetActive(false);
        tapContinueButton.gameObject.SetActive(false);
        OnTutorialDone();
    }

    public void OnTutorialDone()
    {
        switch (tutorialDone)
        {
            case 0:
                // uiManager.ControlBtns.DOKill();
                // uiManager.ControlBtns.localScale = Vector3.one;
                break;
        }

        tutorialDone++;
    }
}

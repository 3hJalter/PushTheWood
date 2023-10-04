using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        Tutorial.Inst.RunTutorial();
        Tutorial.Inst.tutorialObj.SetActive(true);
        gameObject.SetActive(false);
    }
}

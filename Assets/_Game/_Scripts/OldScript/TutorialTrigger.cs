using _Game.Managers;
using _Game.UIs.Screen;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        UIManager.Ins.OpenUI<TempTutorialScreen>();
        gameObject.SetActive(false);
    }
}

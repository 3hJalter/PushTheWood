using _Game._Scripts.Managers;
using _Game._Scripts.UIs.Screen;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        UIManager.Ins.OpenUI<TempTutorialScreen>();
        gameObject.SetActive(false);
    }
}

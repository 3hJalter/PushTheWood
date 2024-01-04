using _Game._Scripts.UIs.Tutorial;
using UnityEngine;

public class TutorialContext : HMonoBehaviour
{
    [SerializeField] private RectTransform rect;

    private TutorialScreen _tutorialScreen;

    public RectTransform Rect => rect;

    public TutorialScreen TutorialScreen
    {
        set => _tutorialScreen = value;
    }

    public void OnClose()
    {
        _tutorialScreen.Close();
    }
}

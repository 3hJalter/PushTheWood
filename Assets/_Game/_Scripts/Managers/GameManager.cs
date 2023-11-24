using _Game.Camera;
using _Game.DesignPattern;
using _Game.UIs.Screen;
using DG.Tweening;
using UnityEngine;

namespace _Game.Managers
{
    public enum GameState
    {
        MainMenu,
        InGame,
        Pause,
        Transition
    }

    public class GameManager : Dispatcher<GameManager>
    {
        [SerializeField] private GameState gameState;

        private void Awake()
        {
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            const int maxScreenHeight = 1280;
            float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
            if (Screen.currentResolution.height > maxScreenHeight)
                Screen.SetResolution(Mathf.RoundToInt(ratio * maxScreenHeight), maxScreenHeight, true);
            // TEST
            UIManager.Ins.OpenUI<MainMenuScreen>();
        }

        public void ChangeState(GameState gameStateI)
        {
            if (gameStateI == GameState.Pause)
            {
                DOTween.PauseAll();
                AudioManager.Ins.PauseSfx();
                PostEvent(EventID.Pause);
            }
            else if (gameState == GameState.Pause)
            {
                DOTween.PlayAll();
                AudioManager.Ins.UnPauseSfx();
                PostEvent(EventID.UnPause);
            }

            gameState = gameStateI;
        }

        public bool IsState(GameState gameStateI)
        {
            return gameState == gameStateI;
        }
    }
}

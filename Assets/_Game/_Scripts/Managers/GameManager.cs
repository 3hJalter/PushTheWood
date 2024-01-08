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
        WorldMap,
        Pause,
        Transition
    }

    public class GameManager : Dispatcher<GameManager>
    {
        [SerializeField]
        private GameState _gameState;
        [SerializeField]
        private bool _reduceScreenResolution;

        private void Awake()
        {
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (_reduceScreenResolution)
            {
                const int maxScreenHeight = 1280;
                float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
                if (Screen.currentResolution.height > maxScreenHeight)
                    Screen.SetResolution(Mathf.RoundToInt(ratio * maxScreenHeight), maxScreenHeight, true);
            }

            // TEST
            if (PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0) != 0) UIManager.Ins.OpenUI<MainMenuScreen>();
            // DontDestroyOnLoad(Tf.root.gameObject);
        }

        public void ChangeState(GameState gameStateI)
        {
            if (gameStateI == GameState.Pause)
            {
                DOTween.PauseAll();
                AudioManager.Ins.PauseSfx();
                PostEvent(EventID.Pause);
            }
            else if (_gameState == GameState.Pause)
            {
                DOTween.PlayAll();
                AudioManager.Ins.UnPauseSfx();
                PostEvent(EventID.UnPause);
            }

            _gameState = gameStateI;
        }

        public bool IsState(GameState gameStateI)
        {
            return _gameState == gameStateI;
        }
    }
}

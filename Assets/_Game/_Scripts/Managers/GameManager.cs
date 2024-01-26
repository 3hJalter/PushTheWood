using _Game.Data;
using _Game.DesignPattern;
using _Game.UIs.Screen;
using _Game.Utilities;
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
        Transition,
        EndGame,
    }
    
    [DefaultExecutionOrder(-95)]
    public class GameManager : Dispatcher<GameManager>
    {
        [SerializeField]
        private GameState gameState;
        [SerializeField]
        private bool reduceScreenResolution;

        private GameData _gameData;
        
        private void Awake()
        {
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (reduceScreenResolution)
            {
                const int maxScreenHeight = 1280;
                float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
                if (Screen.currentResolution.height > maxScreenHeight)
                    Screen.SetResolution(Mathf.RoundToInt(ratio * maxScreenHeight), maxScreenHeight, true);
            }
            
            _gameData = DataManager.Ins.GameData;
            // TODO: Handle Loaded Data
            
            // TEST
            if (_gameData.user.normalLevelIndex > 0) UIManager.Ins.OpenUI<MainMenuScreen>();
            // DontDestroyOnLoad(Tf.root.gameObject);
        }

        #region Game State Handling

        public void ChangeState(GameState gameStateI)
        {
            if (gameStateI == GameState.Pause)
            {
                DOTween.PauseAll();
                PostEvent(EventID.Pause);
            }
            else if (gameStateI == GameState.MainMenu)
            {
                PostEvent(EventID.OnResetToMainMenu);
            }
            
            if (gameState == GameState.Pause && gameStateI != GameState.Pause)
            {
                DOTween.PlayAll();
                PostEvent(EventID.UnPause);
            } 
            
            gameState = gameStateI;
        }

        public bool IsState(GameState gameStateI)
        {
            return gameState == gameStateI;
        }

        #endregion

        #region Income Data function
        
        public void AddGold(int gold)
        {
            _gameData.user.gold += gold;
            PostEvent(EventID.OnGoldMoneyChange, _gameData.user.gold);
            Database.SaveData(_gameData);
        }
        
        public void AddGem(int gem)
        {
            _gameData.user.gems += gem;
            PostEvent(EventID.OnGemMoneyChange, _gameData.user.gems);
            Database.SaveData(_gameData);
        }
        
        public void SpendGold(int gold)
        {
            _gameData.user.gold -= gold;
            PostEvent(EventID.OnGoldMoneyChange, _gameData.user.gold);
            Database.SaveData(_gameData);
        }
        
        public void SpendGem(int gem)
        {
            _gameData.user.gems -= gem;
            PostEvent(EventID.OnGemMoneyChange, _gameData.user.gems);
            Database.SaveData(_gameData);
        }

        #endregion
        

        #region OnApplication

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;
            DevLog.Log(DevId.Hoang, "Application is in background");
            DataManager.Ins.Save();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            DevLog.Log(DevId.Hoang, "Application is in background");
            DataManager.Ins.Save();
        }
        
        private void OnApplicationQuit()
        {
            DevLog.Log(DevId.Hoang, "Application is quitting");
            DataManager.Ins.Save();
        }
        
        #endregion
        
    }
}

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
            VerifyGameData();
        }

        private void VerifyGameData()
        {
            _gameData = DataManager.Ins.GameData;

            #region Handle first day of month

            // bool check if today is the first day of month
            bool isFirstDayOfMonth = System.DateTime.Now.Day == 1;
            if (isFirstDayOfMonth)
            {
                if (!_gameData.user.isFirstDayOfMonthCheck)
                {
                    // Clear daily level progress
                    _gameData.user.dailyLevelIndexComplete.Clear();
                    _gameData.user.isFirstDayOfMonthCheck = true;
                    DevLog.Log(DevId.Hoang, "First day of month, clear daily level progress");
                }
            }
            else
            {
                DevLog.Log(DevId.Hoang, "Not first day of month");
                _gameData.user.isFirstDayOfMonthCheck = false;
            }

            #endregion

            #region Handle If user passes first level

            if (_gameData.user.normalLevelIndex > 0) UIManager.Ins.OpenUI<MainMenuScreen>();
            
            #endregion
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
                PostEvent(EventID.UnPause, gameStateI);
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
        public void AddSecretMapPiece(int piece)
        {
            _gameData.user.secretMapPieces += piece;          
            if(_gameData.user.secretMapPieces >= Constants.REQUIRE_SECRET_MAP_PIECES)
            {
                _gameData.user.secretLevelUnlock += 1;
                _gameData.user.secretMapPieces -= Constants.REQUIRE_SECRET_MAP_PIECES;
            }
            PostEvent(EventID.OnSecretMapPieceChange, _gameData.user.secretMapPieces);
            PostEvent(EventID.OnUnlockSecretMap, _gameData.user.secretLevelUnlock);
            Database.SaveData(_gameData);
        }
        public bool SpendGold(int gold)
        {
            if (_gameData.user.gold < gold) return false;
            _gameData.user.gold -= gold;
            PostEvent(EventID.OnGoldMoneyChange, _gameData.user.gold);
            Database.SaveData(_gameData);
            return true;
        }       
        public bool SpendGem(int gem)
        {
            if (_gameData.user.gems < gem) return false;
            _gameData.user.gems -= gem;
            PostEvent(EventID.OnGemMoneyChange, _gameData.user.gems);
            Database.SaveData(_gameData);
            return true;
        }
        public void ResetUserData()
        {
            _gameData.user.gold = 0;
            _gameData.user.gems = 0;
            _gameData.user.secretLevelUnlock = 0;
            _gameData.user.secretLevelIndex = 0;
        }
        public int SecretLevelUnlock => _gameData.user.secretLevelUnlock;
        public int Gold => _gameData.user.gold;
        public int Gem => _gameData.user.gems;
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

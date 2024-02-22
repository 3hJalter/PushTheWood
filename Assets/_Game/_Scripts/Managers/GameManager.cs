using System;
using _Game.Data;
using _Game.DesignPattern;
using _Game.UIs.Screen;
using _Game.Utilities;
using DG.Tweening;
using UnityEngine;
using VinhLB;

namespace _Game.Managers
{
    public enum GameState
    {
        MainMenu,
        InGame,
        WorldMap,
        Pause,
        Transition,
        WinGame,
        LoseGame,
    }
    
    [DefaultExecutionOrder(-95)]
    public class GameManager : Dispatcher<GameManager>
    {
        [SerializeField]
        private GameState gameState;
        [SerializeField]
        private bool reduceScreenResolution;

        private GameData _gameData;
        
        public float ReduceRatio { get; private set; }
        public bool IsReduce { get; private set; }

        private void Awake()
        {
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // BUGS: Indicator fall when reduce resolution
            if (reduceScreenResolution)
            {
                const int maxScreenHeight = 1280;
                float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
                if (Screen.currentResolution.height > maxScreenHeight)
                {
                    IsReduce = true;
                    int newScreenWidth = Mathf.RoundToInt(ratio * maxScreenHeight);
                    DevLog.Log(DevId.Hoang, $"New resolution: {newScreenWidth} x {maxScreenHeight}");
                    ReduceRatio = Screen.currentResolution.width / (float) newScreenWidth;
                    DevLog.Log(DevId.Hoang, $"Reduce screen resolution with ratio {ReduceRatio}");
                    Screen.SetResolution(newScreenWidth, maxScreenHeight, true);
                }
            }
            VerifyGameData();
        }

        private void VerifyGameData()
        {
            _gameData = DataManager.Ins.GameData;

            #region Handle first day of week

            // bool check if today is the first day of month
            // bool isFirstDayOfMonth = System.DateTime.Now.Day == 1;
            bool isFirstDayOfWeek = System.DateTime.Now.DayOfWeek == DayOfWeek.Monday;
            if (isFirstDayOfWeek)
            {
                if (!_gameData.user.isFirstDayOfWeekCheck)
                {
                    // Clear daily level progress
                    _gameData.user.dailyLevelIndexComplete.Clear();
                    _gameData.user.isFirstDayOfWeekCheck = true;
                    // DevLog.Log(DevId.Hoang, "First day of month, clear daily level progress");
                    DevLog.Log(DevId.Hoang, "First day of week, clear daily level progress");

                }
            }
            else
            {
                // DevLog.Log(DevId.Hoang, "Not first day of month");
                DevLog.Log(DevId.Hoang, "Not first day of week");
                _gameData.user.isFirstDayOfWeekCheck = false;
            }

            #endregion

            #region Handle If user passes first level

            if (_gameData.user.normalLevelIndex > 0) UIManager.Ins.OpenUI<MainMenuScreen>(true);
            
            #endregion

            SmoothGold = Gold;
            SmoothGems = Gems;
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
        
        public int SecretLevelUnlock => _gameData.user.secretLevelUnlock;
        public int SecretMapPieces => _gameData.user.secretMapPieces;
        public int Gold => _gameData.user.gold;
        public int Gems => _gameData.user.gems;
        public float SmoothGold { get; set; }
        public float SmoothGems { get; set; }
        
        public void GainGold(int value, object source = null)
        {
            TryModifyGold(value, source);
        }
        
        public void GainGems(int value,  object source = null)
        {
            TryModifyGems(value, source);
        }
        
        public void GainSecretMapPiece(int amount)
        {
            _gameData.user.secretMapPieces += amount;          
            if(_gameData.user.secretMapPieces >= Constants.REQUIRE_SECRET_MAP_PIECES)
            {
                _gameData.user.secretLevelUnlock += 1;
                _gameData.user.secretMapPieces -= Constants.REQUIRE_SECRET_MAP_PIECES;
            }
            PostEvent(EventID.OnSecretMapPieceChange, _gameData.user.secretMapPieces);
            PostEvent(EventID.OnUnlockSecretMap, _gameData.user.secretLevelUnlock);
            Database.SaveData(_gameData);
        }
        
        public bool TrySpendGold(int amount, object source = null)
        {
            return TryModifyGold(-amount, source);
        }
        
        public bool TrySpendGems(int amount, object source = null)
        {
            return TryModifyGems(-amount, source);
        }
        
        public void ResetUserData()
        {
            TrySpendGold(_gameData.user.gold);
            TrySpendGems(_gameData.user.gems);
            
            _gameData.user.secretLevelUnlock = 0;
            _gameData.user.secretLevelIndex = 0;
        }

        private bool TryModifyGold(int amount, object source)
        {
            if (_gameData.user.gold + amount < 0)
            {
                return false;
            }
            
            ResourceChangeData data = new ResourceChangeData()
            {
                ChangedAmount = amount,
                OldValue = _gameData.user.gold,
                NewValue = _gameData.user.gold + amount,
                Source = source
            };
            
            _gameData.user.gold += amount;
            PostEvent(EventID.OnGoldChange, data);
            Database.SaveData(_gameData);

            if (amount < 0)
            {
                SmoothGold = _gameData.user.gold;
            }

            return true;
        }

        private bool TryModifyGems(int amount, object source)
        {
            if (_gameData.user.gems + amount < 0)
            {
                return false;
            }
            
            ResourceChangeData data = new ResourceChangeData()
            {
                ChangedAmount = amount,
                OldValue = _gameData.user.gems,
                NewValue = _gameData.user.gems + amount,
                Source = source
            };
            
            _gameData.user.gems += amount;
            PostEvent(EventID.OnGemsChange, data);
            Database.SaveData(_gameData);
            
            if (amount < 0)
            {
                SmoothGems = _gameData.user.gems;
            }

            return true;
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

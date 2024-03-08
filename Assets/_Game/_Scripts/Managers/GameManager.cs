using System;
using System.Collections.Generic;
using _Game.Data;
using _Game.DesignPattern;
using _Game.Resource;
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
            DontDestroyOnLoad(gameObject);
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
            if(_gameData.user.normalLevelIndex == 0) //NOTE: First time run game
            {
                DataManager.Ins.SetUnlockCharacterSkin(0, true);
                DataManager.Ins.SetCharacterSkinIndex(0);
            }
            #region Handle day online

            // If player not pass level 8, reset daily level index
            // if (_gameData.user.normalLevelIndex < DataManager.Ins.ConfigData.unlockDailyChallengeAtLevelIndex)
            // {
            //     _gameData.user.isCompleteDailyChallengerTutorial = false;
            //     _gameData.user.currentDailyChallengerDay = 1;
            //     _gameData.user.dailyLevelIndexComplete.Clear();
            //     _gameData.user.dailyChallengeRewardCollected.Clear();
            // }
            // else
            {
                // Check if pass daily challenger tutorial
                // if (!_gameData.user.isCompleteDailyChallengerTutorial)
                // {
                //     _gameData.user.currentDailyChallengerDay = 1;
                //     _gameData.user.dailyLevelIndexComplete.Clear();
                //     _gameData.user.dailyChallengeRewardCollected.Clear();
                // }
                // else
                {
                    // Get now day & last login day
                    int day = (int)(DateTime.UtcNow.Date - _gameData.user.lastTimeLogOut.ToUniversalTime().Date).TotalDays;
                    
                    
                    if (day > 0)
                    {
                        _gameData.user.playedDay += 1;
                        _gameData.user.currentDailyChallengerDay += day;
                        AnalysticManager.Ins.Day();
                    }
                    // if the currentDailyChallengerDay is greater than the number of daily challenger, reset it
                    if (_gameData.user.currentDailyChallengerDay > Constants.DAILY_CHALLENGER_COUNT)
                    {
                        _gameData.user.currentDailyChallengerDay %= 7 + 1;
                        _gameData.user.dailyLevelIndexComplete.Clear();
                        _gameData.user.dailyChallengeRewardCollected.Clear();
                    }
                }
            }

            #endregion
         
            SmoothAdTickets = AdTickets;
            SmoothGold = Gold;
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
            PostEvent(EventID.OnChangeGameState, gameStateI);
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
        public int AdTickets => _gameData.user.adTickets;
        public int RewardKeys => _gameData.user.rewardChestKeys;
        public int LevelProgress => _gameData.user.levelChestProgress;
        public float SmoothGold { get; set; }
        public float SmoothAdTickets { get; set; }
        
        public void GainGold(int value, object source = null)
        {
            TryModifyGold(value, source);
        }
        
        public void GainAdTickets(int value, object source = null)
        {
            TryModifyAdTickets(value, source);
        }
        
        public void GainSecretMapPiece(int amount)
        {
            _gameData.user.secretMapPieces += amount;          
            if(_gameData.user.secretMapPieces >= DataManager.Ins.ConfigData.requireSecretMapPiece)
            {
                _gameData.user.secretLevelUnlock += _gameData.user.secretMapPieces / DataManager.Ins.ConfigData.requireSecretMapPiece;
                _gameData.user.secretMapPieces = _gameData.user.secretMapPieces % DataManager.Ins.ConfigData.requireSecretMapPiece;
                //PostEvent(EventID.OnUnlockSecretMap, _gameData.user.secretLevelUnlock);
            }
            //PostEvent(EventID.OnSecretMapPieceChange, _gameData.user.secretMapPieces);
            PostEvent(EventID.OnUpdateUIs);
            Database.SaveData(_gameData);
        }
        
        public void GainBooster(BoosterType type, int amount)
        {
            switch (type)
            {
                case BoosterType.Undo:
                    _gameData.user.undoCount += amount;
                    break;
                case BoosterType.PushHint:
                    _gameData.user.pushHintCount += amount;
                    break;
                case BoosterType.GrowTree:
                    _gameData.user.growTreeCount += amount;
                    break;
                case BoosterType.ResetIsland:
                    _gameData.user.resetIslandCount += amount;
                    break;
            }
            PostEvent(EventID.OnUpdateUIs);
            Database.SaveData(_gameData);
        }
        
        public void GainRewardKey(int amount)
        {
            _gameData.user.rewardChestKeys += amount;
            if(_gameData.user.rewardChestKeys >= DataManager.Ins.ConfigData.requireRewardKey)
            {
                _gameData.user.rewardChestUnlock += _gameData.user.rewardChestKeys / DataManager.Ins.ConfigData.requireRewardKey;
                _gameData.user.rewardChestKeys = _gameData.user.rewardChestKeys % DataManager.Ins.ConfigData.requireRewardKey;
                //PostEvent(EventID.OnUnlockRewardChest, _gameData.user.rewardChestUnlock);
            }
            //PostEvent(EventID.OnRewardChestKeyChange, _gameData.user.rewardChestKeys);
            PostEvent(EventID.OnUpdateUIs);
            Database.SaveData(_gameData);
        }

        public void GainLevelProgress(int amount)
        {
            _gameData.user.levelChestProgress += amount;
            if (_gameData.user.levelChestProgress >= DataManager.Ins.ConfigData.requireLevelProgress)
            {
                _gameData.user.levelChestUnlock += _gameData.user.levelChestProgress / DataManager.Ins.ConfigData.requireLevelProgress;
                _gameData.user.levelChestProgress = _gameData.user.levelChestProgress % DataManager.Ins.ConfigData.requireLevelProgress;
            }
            PostEvent(EventID.OnUpdateUIs);
            Database.SaveData(_gameData);
        }

        
        public bool TrySpendGold(int amount, object source = null)
        {
            return TryModifyGold(-amount, source);
        }

        public bool TrySpendAdTickets(int amount, object source = null)
        {
            return TryModifyAdTickets(-amount, source);
        }
        
        public void ResetUserData()
        {
            TrySpendGold(_gameData.user.gold);
            TrySpendAdTickets(_gameData.user.adTickets);
            
            _gameData.user.secretLevelUnlock = 0;
            _gameData.user.secretLevelIndex = 0;
        }

        private bool TryModifyGold(int amount, object source)
        {
            if (_gameData.user.gold + amount < 0)
            {
                return false;
            }
            #region ANALYSTIC
            if(amount > 0)
            {
                AnalysticManager.Ins.ResourceEarn(CurrencyType.Gold, Placement.None, amount);
            }
            else if(amount < 0)
            {
                AnalysticManager.Ins.ResourceSpend(CurrencyType.Gold, Placement.None, amount);
            }
            #endregion
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

        private bool TryModifyAdTickets(int amount, object source)
        {
            if (_gameData.user.adTickets + amount < 0)
            {
                return false;
            }
            
            ResourceChangeData data = new ResourceChangeData()
            {
                ChangedAmount = amount,
                OldValue = _gameData.user.adTickets,
                NewValue = _gameData.user.adTickets + amount,
                Source = source
            };
            
            _gameData.user.adTickets += amount;
            PostEvent(EventID.OnAdTicketsChange, data);
            Database.SaveData(_gameData);

            if (amount < 0)
            {
                SmoothAdTickets = _gameData.user.adTickets;
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
            _gameData.user.lastTimeLogOut = DateTime.Now;
            DataManager.Ins.Save();
        }
        
        #endregion
    }
}

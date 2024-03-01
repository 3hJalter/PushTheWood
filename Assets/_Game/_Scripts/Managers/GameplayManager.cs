using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
using _Game._Scripts.UIs.Component;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Resource;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using TMPro;
using UnityEngine;
using VinhLB;

namespace _Game.Managers
{
    [DefaultExecutionOrder(-90)]
    public class GameplayManager : Singleton<GameplayManager>
    {
        [SerializeField] private PushHintObject pushHintObject;

        private readonly Dictionary<int, bool> isBoughtPushHintInIsland = new();
        private PushHint _pushHint;
        private bool isBoughtGrowTree;
        private bool isCanGrowTree = true;
        private bool isCanResetIsland = true;
        private bool isCanUndo = true;

        private InGameScreen screen;

        public InGameScreen Screen => screen;

        private int time;
        private STimer timer;

        public PushHintObject PushHintObject => pushHintObject;

        public SavePushHint SavePushHint { get; private set; }

        public bool IsCanUndo
        {
            get => isCanUndo;
            set
            {
                isCanUndo = value;
                screen.SetActiveUndo(value);
            }
        }

        public bool IsCanResetIsland
        {
            get => isCanResetIsland;
            set
            {
                isCanResetIsland = value;
                screen.SetActiveResetIsland(value);
            } 
        }

        public bool IsBoughtGrowTree
        {
            get => isBoughtGrowTree;
            set
            {
                isBoughtGrowTree = value;
                screen.OnBoughtGrowTree(value);
            }
        }

        public bool IsCanGrowTree
        {
            get => isCanGrowTree;
            set
            {
                isCanGrowTree = value;
                screen.SetActiveGrowTree(value);
            }
        }

        private void Awake()
        {
            screen = UIManager.Ins.GetUI<InGameScreen>();
            screen.Close();
            timer = TimerManager.Ins.PopSTimer();
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.RegisterListenerEvent(EventID.MoreTimeGame, OnMoreTimeGame);
            GameManager.Ins.RegisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.RegisterListenerEvent(EventID.LoseGame, OnLoseGame);
            GameManager.Ins.RegisterListenerEvent(EventID.Pause, OnPauseGame);
            GameManager.Ins.RegisterListenerEvent(EventID.UnPause, OnUnPauseGame);
            // GameManager.Ins.RegisterListenerEvent(EventID.OnResetToMainMenu, OnToMainMenu);
            // TODO: Refactor Booster later to avoid duplicate code
            screen.OnUndo += OnUndo;
            screen.OnGrowTree += OnGrowTree;
            screen.OnUsePushHint += OnPushHint;
            screen.OnResetIsland += OnResetIsland;
            screen.undoButton.SetAmount(DataManager.Ins.GameData.user.undoCount);
            screen.pushHintButton.SetAmount(DataManager.Ins.GameData.user.pushHintCount);
            screen.growTreeButton.SetAmount(DataManager.Ins.GameData.user.growTreeCount);
            screen.resetIslandButton.SetAmount(DataManager.Ins.GameData.user.resetIslandCount);
        }

        private void Start()
        {
            EventGlobalManager.Ins.OnChangeBoosterAmount.AddListener(OnChangeBoosterAmount);
            EventGlobalManager.Ins.OnPlayerChangeIsland.AddListener(OnPlayerChangeIsland);
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.MoreTimeGame, OnMoreTimeGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.LoseGame, OnLoseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.Pause, OnPauseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.UnPause, OnUnPauseGame);
            EventGlobalManager.Ins.OnChangeBoosterAmount?.RemoveListener(OnChangeBoosterAmount);
            screen.OnUndo -= OnUndo;
            screen.OnGrowTree -= OnGrowTree;
            screen.OnUsePushHint -= OnPushHint;
            EventGlobalManager.Ins.OnPlayerChangeIsland?.RemoveListener(OnPlayerChangeIsland);
            TimerManager.Ins.PushSTimer(timer);
        }

        private bool IsBoughtPushHintInIsland(int islandID)
        {
            return isBoughtPushHintInIsland[islandID];
        }

        private void SetBoughtPushHintInIsland(int islandID, bool value)
        {
            isBoughtPushHintInIsland[islandID] = value;
        }

        private void OnChangeBoosterAmount(BoosterType boosterType, int amount)
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.undoCount, screen.undoButton, amount);
                    break;
                case BoosterType.GrowTree:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.growTreeCount, screen.growTreeButton,
                        amount);
                    break;
                case BoosterType.PushHint:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.pushHintCount, screen.pushHintButton,
                        amount);
                    break;
                case BoosterType.ResetIsland:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.resetIslandCount, screen.resetIslandButton,
                        amount);
                    break;
            }
        }

        private void UpdateBoosterCount(ref int boosterCount, BoosterButton button, int amount)
        {
            boosterCount += amount;
            button.SetAmount(boosterCount);
        }
        
        private void UpdateBoosterCount(ref int boosterCount, TMP_Text text, int amount)
        {
            boosterCount += amount;
            text.text = boosterCount.ToString();
        }

        private void OnToMainMenu()
        {
            _pushHint?.OnStopHint();
            timer.Stop();
            LevelManager.Ins.OnRestart();
        }

        public void OnPauseGame()
        {
            timer.Stop();
        }

        public void OnUnPause()
        {
            timer.Start(1f, CountTime, true);
        }

        private void OnUnPauseGame(object o)
        {
            // Cast o to GameState
            GameState nextState = (GameState)o;
            // More check for some level that not need timer
            if (nextState is GameState.InGame) timer.Start(1f, CountTime, true);
        }

        public void OnResetTime()
        {
            LevelType levelType = LevelManager.Ins.CurrentLevel.LevelType;
            time = levelType is LevelType.Normal
                ? DataManager.Ins.GetLevelTime(levelType, LevelManager.Ins.CurrentLevel.LevelNormalType)
                : DataManager.Ins.GetLevelTime(LevelManager.Ins.CurrentLevel.LevelType);
            // if first level of normal level, or daily time is MaxValue because it is tutorial level
            if (LevelManager.Ins.IsFirstTutorialLevel) time = int.MaxValue;
            screen.Time = time;
            timer.Start(1f, CountTime, true);
        }

        private void OnMoreTimeGame()
        {
            OnResetTime();
            IsCanResetIsland = true;
            IsCanUndo = true;
            GameManager.Ins.ChangeState(GameState.InGame);
            LevelManager.Ins.player.SetActiveAgent(false);
        }

        private void OnStartGame()
        {
            if (LevelManager.Ins.IsSavePlayerPushStep) SavePushHint = new SavePushHint();
            PushHintObject.SetActive(false);
            OnResetTime();
            screen.OnHandleTutorial();
            screen.OnCheckBoosterLock();
            IsCanResetIsland = true;
            IsCanUndo = true;
            IsBoughtGrowTree = false;
            IsCanGrowTree = false;
            isBoughtPushHintInIsland.Clear();
            _pushHint = new PushHint(LevelManager.Ins.CurrentLevel.GetPushHint());
            screen.OnSetBoosterAmount();
            GameManager.Ins.ChangeState(GameState.InGame);
            LevelManager.Ins.player.SetActiveAgent(false);
            // If hard level, show a notification -> If it None -> not show
            DevLog.Log(DevId.Hoang, $"LEVEL NORMAL TYPE: {LevelManager.Ins.CurrentLevel.LevelNormalType}");
        }

        private void OnWinGame()
        {
            if (LevelManager.Ins.IsSavePlayerPushStep) SavePushHint.Save();
            _pushHint.OnStopHint();
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Win Screen");
            UIManager.Ins.OpenUI<WinScreen>();
            GameManager.Ins.ChangeState(GameState.WinGame);
            LevelManager.Ins.player.SetActiveAgent(false);
        }

        private void OnLoseGame()
        {
            _pushHint.OnStopHint();
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Lose Screen");
            // Show Different Lose based on reason (Ex: Lose by Die will not show More time booster, instead show Revive) -> Check by the time remaining
            UIManager.Ins.OpenUI<LoseScreen>(time <= 0);
            GameManager.Ins.ChangeState(GameState.LoseGame);
            LevelManager.Ins.player.SetActiveAgent(false);
        }

        private void CountTime()
        {
            if (time < 0) return;
            time -= 1;
            screen.Time = time;
            if (time <= 0) OnLoseGame();
        }

        #region Booster

        public void OnFreeUndo()
        {
            if (!isCanUndo) return;
            if (!LevelManager.Ins.OnUndo()) return;
            if (!_pushHint.IsStartHint)
            {
                if (_pushHint.IsPlayerMakeHintWrong) _pushHint.OnStopHint();
            }
            else
            {
                if (_pushHint.IsPlayerMakeHintWrong) _pushHint.OnContinueHint();
                else
                {
                    _pushHint.OnRevertHint();
                }
            }
        }
        
        private void OnUndo()
        {
            if (!isCanUndo) return;
            if (DataManager.Ins.GameData.user.undoCount <= 0)
            {
                DevLog.Log(DevId.Hoang, "Show popup to buy undo");
                BoosterConfig boosterConfig = DataManager.Ins.ConfigData.boosterConfigList[(int)BoosterType.Undo];
                boosterConfig.UIResourceConfig = DataManager.Ins.GetBoosterUIResourceConfig(boosterConfig.Type);
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(boosterConfig);
            }
            else
            {
                if (!LevelManager.Ins.OnUndo()) return;
                DataManager.Ins.GameData.user.undoCount--;
                screen.undoButton.SetAmount(DataManager.Ins.GameData.user.undoCount);
                // TEMPORARY
                if (!_pushHint.IsStartHint)
                {
                    if (_pushHint.IsPlayerMakeHintWrong) _pushHint.OnStopHint();
                }
                else
                {
                    if (_pushHint.IsPlayerMakeHintWrong) _pushHint.OnContinueHint();
                    else
                    {
                        _pushHint.OnRevertHint();
                    }
                }
            }
        }

        public void OnFreeResetIsland()
        {
            LevelManager.Ins.ResetLevelIsland();
            if (!_pushHint.IsStartHint) return;
            _pushHint.OnStopHint();
            _pushHint.OnStartHint(LevelManager.Ins.player.islandID);
        }
        
        private void OnResetIsland()
        {
            if (DataManager.Ins.GameData.user.resetIslandCount <= 0)
            {
                DevLog.Log(DevId.Hoang, "Show popup to buy reset");
                BoosterConfig boosterConfig = DataManager.Ins.ConfigData.boosterConfigList[(int)BoosterType.ResetIsland];
                boosterConfig.UIResourceConfig = DataManager.Ins.GetBoosterUIResourceConfig(boosterConfig.Type);
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(boosterConfig);
            }
            else
            {
                DataManager.Ins.GameData.user.resetIslandCount--;
                screen.resetIslandButton.SetAmount(DataManager.Ins.GameData.user.resetIslandCount);
                LevelManager.Ins.ResetLevelIsland();
                if (!_pushHint.IsStartHint) return;
                _pushHint.OnStopHint();
                if (screen.resetIslandButton.IsFocus || !_pushHint.IsPlayerMakeHintWrong) _pushHint.OnStartHint(LevelManager.Ins.player.islandID);
            }
        }

        private void OnGrowTree()
        {
            if (DataManager.Ins.GameData.user.growTreeCount <= 0)
            {
                DevLog.Log(DevId.Hoang, "Show popup to buy grow tree");
                BoosterConfig boosterConfig = DataManager.Ins.ConfigData.boosterConfigList[(int)BoosterType.GrowTree];
                boosterConfig.UIResourceConfig = DataManager.Ins.GetBoosterUIResourceConfig(boosterConfig.Type);
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(boosterConfig);
            }
            else
            {
                if (EventGlobalManager.Ins.OnGrowTree.listenerCount <= 0) return;
                if (!IsBoughtGrowTree)
                {
                    IsBoughtGrowTree = true;
                    DataManager.Ins.GameData.user.growTreeCount--;
                    screen.growTreeButton.SetAmount(DataManager.Ins.GameData.user.growTreeCount);
                }
                EventGlobalManager.Ins.OnGrowTree.Dispatch();
                if (_pushHint.IsStartHint) _pushHint.OnStopHint();
            }
        }

        # region PUSH HINT

        public void OnFreePushHint(bool isReset, bool isShowHint)
        {
            int playerIslandID = LevelManager.Ins.player.islandID;
            if (_pushHint.IsStartHint) return;
            if (isReset) LevelManager.Ins.ResetLevelIsland();
            OnStartHintOnPlayerIsland(playerIslandID, isShowHint);
        }
        
        private void OnPushHint()
        {
            int playerIslandID = LevelManager.Ins.player.islandID;
            if (!isBoughtPushHintInIsland.ContainsKey(playerIslandID)) return;
            if (DataManager.Ins.GameData.user.pushHintCount <= 0)
            {
                if (!IsBoughtPushHintInIsland(playerIslandID)) UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(DataManager.Ins.ConfigData.boosterConfigList[(int)BoosterType.PushHint]);
                else OnStartHintOnPlayerIsland(playerIslandID);
            }
            else
            {
                if (_pushHint.IsStartHint) return;
                // If this is first player step cell in this island, return
                if (!IsBoughtPushHintInIsland(playerIslandID))
                {
                    DataManager.Ins.GameData.user.pushHintCount--;
                    screen.pushHintButton.SetAmount(DataManager.Ins.GameData.user.pushHintCount);
                    SetBoughtPushHintInIsland(playerIslandID, true);
                    LevelManager.Ins.ResetLevelIsland();
                }
                OnStartHintOnPlayerIsland(playerIslandID);
            }
        }

        private void OnStartHintOnPlayerIsland(int islandID, bool isShowHint = true)
        {
            _pushHint.OnStartHint(islandID, isShowHint);
            DevLog.Log(DevId.Hoang, $"Show hint in {islandID}");
        }

        private void OnPlayerChangeIsland(bool isInit)
        {
            int islandId = LevelManager.Ins.player.islandID;
            screen.ActivePushHintIsland(_pushHint.ContainIsland(islandId));
            // Check if contain
            if (!isBoughtPushHintInIsland.ContainsKey(islandId))
            {
                SetBoughtPushHintInIsland(islandId, false);
            }
            bool isBoughtPushHint = IsBoughtPushHintInIsland(islandId);
            screen.OnBoughtPushHintOnIsland(islandId, isBoughtPushHint, isInit);
        }

        public void OnShowTryHintAgain(bool show)
        {
            screen.OnShowTryHintAgain(show);
        }

        #endregion

        #endregion
    }
}

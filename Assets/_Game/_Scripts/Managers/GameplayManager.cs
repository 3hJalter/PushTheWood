using System.Collections.Generic;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
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
        private int time;
        STimer timer;

        InGameScreen screen;

        private bool isCanUndo = true;
        private bool isCanResetIsland = true;
        private bool isBoughtGrowTree = false;
        private bool isCanGrowTree = true;

        public SaveHint SaveHint { get; private set; }

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
            GameManager.Ins.RegisterListenerEvent(EventID.OnResetToMainMenu, OnToMainMenu);
            // TODO: Refactor Booster later to avoid duplicate code
            screen.OnUndo += OnUndo;
            screen.OnResetIsland += OnResetIsland;
            screen.OnHint += OnShowHint;
            screen.OnCancelHint += OnHideHint;
            screen.OnGrowTree += OnGrowTree;
            screen.undoCountText.text = DataManager.Ins.GameData.user.undoCount.ToString();
            screen.resetCountText.text = DataManager.Ins.GameData.user.resetCount.ToString();
            screen.hintCountText.text = DataManager.Ins.GameData.user.hintCount.ToString();
            screen.growTreeCountText.text = DataManager.Ins.GameData.user.growTreeCount.ToString();
        }

        private void Start()
        {
            EventGlobalManager.Ins.OnChangeBoosterAmount.AddListener(OnChangeBoosterAmount);
        }

        private void OnChangeBoosterAmount(BoosterType boosterType, int amount)
        {
            switch (boosterType)
            {
                case BoosterType.Undo:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.undoCount, screen.undoCountText, amount);
                    break;
                case BoosterType.ResetIsland:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.resetCount, screen.resetCountText, amount);
                    break;
                case BoosterType.Hint:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.hintCount, screen.hintCountText, amount);
                    break;
                case BoosterType.GrowTree:
                    UpdateBoosterCount(ref DataManager.Ins.GameData.user.growTreeCount, screen.growTreeCountText, amount);
                    break;
            }
        }
        
        private void UpdateBoosterCount(ref int boosterCount, TMP_Text text, int amount)
        {
            boosterCount += amount;
            text.text = boosterCount.ToString();
        }
        
        private void OnToMainMenu()
        {
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
            GameState nextState = (GameState) o;
            // More check for some level that not need timer
            if (nextState is GameState.InGame) timer.Start(1f, CountTime, true);
        }
        
        public void OnResetTime()
        {
            time = 0;
            time += DataManager.Ins.GetLevelTime(LevelManager.Ins.CurrentLevel.LevelType);
            // if first level of normal level, or daily time is MaxValue because it is tutorial level
            if (LevelManager.Ins.IsTutorialLevel) time = int.MaxValue;
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
            if (LevelManager.Ins.IsSavePlayerPushStep)
            {
                SaveHint = new SaveHint();
            }
            OnResetTime();
            screen.OnHideIfTutorial();
            IsCanResetIsland = true;
            IsCanUndo = true;
            IsBoughtGrowTree = false;
            GameManager.Ins.ChangeState(GameState.InGame);
            LevelManager.Ins.player.SetActiveAgent(false);
            // If hard level, show a notification
            DevLog.Log(DevId.Hoang, $"Hard Level: {LevelManager.Ins.CurrentLevel.IsHardLevel}");
        }

        private void OnWinGame()
        {
            if (LevelManager.Ins.IsSavePlayerPushStep)
            {
                SaveHint.Save();
            }
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Win Screen");
            UIManager.Ins.OpenUI<WinScreen>();
            GameManager.Ins.ChangeState(GameState.WinGame);
            LevelManager.Ins.player.SetActiveAgent(false);
        }

        private void OnLoseGame()
        {
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Lose Screen");
            // Show Different Lose based on reason (Ex: Lose by Die will not show More time booster, instead show Revive) -> Check by the time remaining
            UIManager.Ins.OpenUI<LoseScreen>(time <= 0);
            GameManager.Ins.ChangeState(GameState.LoseGame);
            LevelManager.Ins.player.SetActiveAgent(false);
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.MoreTimeGame, OnMoreTimeGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.LoseGame, OnLoseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.Pause, OnPauseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.UnPause, OnUnPauseGame);
            EventGlobalManager.Ins.OnChangeBoosterAmount.RemoveListener(OnChangeBoosterAmount);
            screen.OnUndo -= OnUndo;
            screen.OnResetIsland -= OnResetIsland;
            screen.OnHint -= OnShowHint;
            screen.OnCancelHint -= OnHideHint;
            screen.OnGrowTree -= OnGrowTree;
            TimerManager.Ins.PushSTimer(timer);         
        }

        #region Booster

        private void OnUndo()
        {
            if (!isCanUndo) return;
            if (DataManager.Ins.GameData.user.undoCount <= 0)
            {
                DevLog.Log(DevId.Hoang, "Show popup to buy undo");
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(DataManager.Ins.ConfigData.boosterConfigs[BoosterType.Undo]);
            }
            else
            {
                if (!LevelManager.Ins.OnUndo()) return;
                DataManager.Ins.GameData.user.undoCount--;
                screen.undoCountText.text = DataManager.Ins.GameData.user.undoCount.ToString();
            }
            
        }

        private void OnResetIsland()
        {
            if (!isCanResetIsland) return;
            if (DataManager.Ins.GameData.user.resetCount <= 0)
            {
                // TODO: Show popup to buy reset
                DevLog.Log(DevId.Hoang, "Show popup to buy reset");
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(DataManager.Ins.ConfigData.boosterConfigs[BoosterType.ResetIsland]);
            }
            else
            {
                DataManager.Ins.GameData.user.resetCount--;
                screen.resetCountText.text = DataManager.Ins.GameData.user.resetCount.ToString();
                LevelManager.Ins.ResetLevelIsland();
            }
        }

        private void OnShowHint()
        {
            if (DataManager.Ins.GameData.user.hintCount <= 0)
            {
                // TODO: Show popup to buy hint
                DevLog.Log(DevId.Hoang, "Show popup to buy hint");
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(DataManager.Ins.ConfigData.boosterConfigs[BoosterType.Hint]);
            }
            else
            {
                DataManager.Ins.GameData.user.hintCount--;
                screen.hintCountText.text =  DataManager.Ins.GameData.user.hintCount.ToString();
                LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(false);               
                FXManager.Ins.TrailHint.OnPlay(LevelManager.Ins.CurrentLevel.HintLinePosList);
                LevelManager.Ins.player.SetActiveAgent(true);
                LevelManager.Ins.ResetLevelIsland();
            }
        }

        private void OnHideHint()
        {
            LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(true);
            FXManager.Ins.TrailHint.OnCancel();
            LevelManager.Ins.player.SetActiveAgent(false);
        }
        
        private void OnGrowTree()
        {
            if (DataManager.Ins.GameData.user.growTreeCount <= 0)
            {
                DevLog.Log(DevId.Hoang, "Show popup to buy grow tree");
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>(DataManager.Ins.ConfigData.boosterConfigs[BoosterType.GrowTree]);
            }
            else
            {
                if (EventGlobalManager.Ins.OnGrowTree.listenerCount <= 0) return;
                if (!IsBoughtGrowTree)
                {
                    IsBoughtGrowTree = true;
                    DataManager.Ins.GameData.user.growTreeCount--;
                    screen.growTreeCountText.text = DataManager.Ins.GameData.user.growTreeCount.ToString();
                }
                EventGlobalManager.Ins.OnGrowTree.Dispatch();
            }
        }

        #endregion
        
        private void CountTime()
        {
            time -= 1;
            screen.Time = time;
            if(time <= 0)
            {
                OnLoseGame();
            }
        }
    }
}
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
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
        
        public bool IsBoughtHint { get; set; }

        private void Awake()
        {
            screen = UIManager.Ins.GetUI<InGameScreen>();
            screen.Close();
            timer = TimerManager.Inst.PopSTimer();
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, OnStartGame);
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
            screen.undoCountText.text = DataManager.Ins.GameData.user.undoCount.ToString();
            screen.resetCountText.text = DataManager.Ins.GameData.user.resetCount.ToString();
            screen.hintCountText.text = DataManager.Ins.GameData.user.hintCount.ToString();
        }

        private void OnToMainMenu()
        {
            timer.Stop();
            LevelManager.Ins.OnRestart();
        }
        
        private void OnPauseGame()
        {
            timer.Stop();
        }

        private void OnUnPauseGame()
        {
            timer.Start(1f, CountTime, true);
        }
        
        public void OnResetTime()
        {
            time = 0;
            time += DataManager.Ins.ConfigData.timePerNormalLevel;
            screen.Time = time;
            timer.Start(1f, CountTime, true);
        }
        
        private void OnStartGame()
        {
            OnResetTime();
            IsCanResetIsland = true;
            IsCanUndo = true;
            GameManager.Ins.ChangeState(GameState.InGame);
        }

        private void OnWinGame()
        {
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Win Screen");
            UIManager.Ins.OpenUI<WinScreen>();
            GameManager.Ins.ChangeState(GameState.EndGame);
        }

        private void OnLoseGame()
        {
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Lose Screen");
            // TODO: Show Different Lose based on reason (Ex: Lose by Die will not show More time booster, instead show retry)
            UIManager.Ins.OpenUI<LoseScreen>();
            GameManager.Ins.ChangeState(GameState.EndGame);
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.LoseGame, OnLoseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.Pause, OnPauseGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.UnPause, OnUnPauseGame);
            screen.OnUndo -= OnUndo;
            screen.OnResetIsland -= OnResetIsland;
            screen.OnHint -= OnShowHint;
            screen.OnCancelHint -= OnHideHint;
            TimerManager.Inst.PushSTimer(timer);         
        }

        #region Booster

        private void OnUndo()
        {
            if (!isCanUndo) return;
            if (DataManager.Ins.GameData.user.undoCount <= 0)
            {
                // TODO: Show popup to buy undo
                DevLog.Log(DevId.Hoang, "Show popup to buy undo");
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
                // Temporary +10 undo
                DevLog.Log(DevId.Hoang, "Temporary +10 undo");
                DataManager.Ins.GameData.user.undoCount+=10;
                screen.undoCountText.text = DataManager.Ins.GameData.user.undoCount.ToString();
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
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
                // Temporary +10 reset
                DevLog.Log(DevId.Hoang, "Temporary +10 reset");
                DataManager.Ins.GameData.user.resetCount+=10;
                screen.resetCountText.text = DataManager.Ins.GameData.user.resetCount.ToString();
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
                UIManager.Ins.OpenUI<BoosterWatchVideoPopup>();
                // Temporary +10 hint
                DevLog.Log(DevId.Hoang, "Temporary +10 hint");
                DataManager.Ins.GameData.user.hintCount+=10;
                screen.hintCountText.text = DataManager.Ins.GameData.user.hintCount.ToString();
            }
            else
            {
                DataManager.Ins.GameData.user.hintCount--;
                screen.hintCountText.text =  DataManager.Ins.GameData.user.hintCount.ToString();
                LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(false);
                FXManager.Ins.TrailHint.OnPlay(LevelManager.Ins.CurrentLevel.HintLinePosList);
            }
        }

        private void OnHideHint()
        {
            LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(true);
            FXManager.Ins.TrailHint.OnCancel();
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
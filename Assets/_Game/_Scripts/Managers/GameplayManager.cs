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
            screen.OnUndo += OnUndo;
            screen.OnResetIsland += OnResetIsland;
            screen.OnHint += OnShowHint;
            screen.OnCancelHint += OnHideHint;
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

        private void OnUndo()
        {
            if (!isCanUndo) return;
            LevelManager.Ins.OnUndo();
        }

        private void OnResetIsland()
        {
            if (!isCanResetIsland) return;
            LevelManager.Ins.ResetLevelIsland();
        }

        private void OnShowHint()
        {
            if (!IsBoughtHint) return;
            LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(false);
            FXManager.Ins.TrailHint.OnPlay(
                LevelManager.Ins.CurrentLevel.HintLinePosList);
        }

        private void OnHideHint()
        {
            if (!IsBoughtHint) return;
            LevelManager.Ins.CurrentLevel.ChangeShadowUnitAlpha(true);
            FXManager.Ins.TrailHint.OnCancel();
        }
        
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
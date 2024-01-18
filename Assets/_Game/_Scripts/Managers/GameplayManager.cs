using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Managers
{
    [DefaultExecutionOrder(-90)]
    public class GameplayManager : Singleton<GameplayManager>
    {
        private int time;
        private int undoCount;
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
        private void Awake()
        {
            screen = UIManager.Ins.GetUI<InGameScreen>();
            screen.Close();
            timer = TimerManager.Inst.PopSTimer();
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.RegisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.RegisterListenerEvent(EventID.LoseGame, OnLoseGame);
            screen.OnUndo += OnUndo;
            screen.OnResetIsland += OnResetIsland;
        }
        private void OnStartGame()
        {
            time = Constants.LEVEL_TIME;
            undoCount = Constants.UNDO_COUNT;
            screen.Time = time;
            IsCanResetIsland = true;
            IsCanUndo = true;
            timer.Start(1f, CountTime, true);
            void CountTime()
            {
                time -= 1;
                screen.Time = time;
                if(time <= 0)
                {
                    OnLoseGame();
                }
            }
        }

        private void OnWinGame()
        {
            UIManager.Ins.OpenUI<WinScreen>();
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Win Screen");
        }

        private void OnLoseGame()
        {
            timer.Stop();
            DevLog.Log(DevId.Hung, "ENDGAME - Show Lose Screen");
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.UnregisterListenerEvent(EventID.LoseGame, OnLoseGame);
            screen.OnUndo -= OnUndo;
            screen.OnResetIsland -= OnResetIsland;
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
    }
}
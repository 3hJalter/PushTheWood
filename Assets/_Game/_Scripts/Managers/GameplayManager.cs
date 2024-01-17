using _Game.DesignPattern;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
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
        private void Awake()
        {
            screen = UIManager.Ins.GetUI<InGameScreen>();
            screen.Close();
            timer = TimerManager.Inst.PopSTimer();
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, OnStartGame);
            GameManager.Ins.RegisterListenerEvent(EventID.WinGame, OnWinGame);
            GameManager.Ins.RegisterListenerEvent(EventID.LoseGame, OnLoseGame);
        }
        private void OnStartGame()
        {
            time = Constants.LEVEL_TIME;
            undoCount = Constants.UNDO_COUNT;
            screen.Time = time;
            timer.Start(1f, CountTime, true);
            void CountTime()
            {
                time -= 1;
                screen.Time = time;
                if(time <= 0)
                {
                    DevLog.Log(DevId.Hung, "Out of time!");
                }
            }
        }

        private void OnWinGame()
        {
            timer.Stop();

        }

        private void OnLoseGame()
        {
            timer.Stop();

        }

        private void OnDestroy()
        {
            TimerManager.Inst.PushSTimer(timer);
        }
    }
}
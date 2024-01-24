using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
using _Game.Managers;
using UnityEngine;

namespace VinhLB
{
    public class LoseScreen : UICanvas
    {
        public void OnClickRestartButton()
        {
            LevelManager.Ins.OnRestart();
            Close();
        }

        public void OnClickMoreTimeButton()
        {
            GameplayManager.Ins.OnResetTime();
            Close();
        }
    }
}
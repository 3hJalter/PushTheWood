using System.Collections;
using System.Collections.Generic;
using _Game.GameGrid;
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
    }
}
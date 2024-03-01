﻿using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel5", menuName = "ScriptableObjects/TutorialData/Lvl5", order = 1)]
    public class TutorialConditionLevel5 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if Player at cell 7, 9 -> Change to Cell that Player init in level 5
                // TODO: Screen for growing tree
                if (!(Math.Abs(cell.WorldX - 7) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 9) < Constants.TOLERANCE)) return;
                DevLog.Log(DevId.Hoang, "TutorialConditionLevel4, HandleShowTutorial, show tutorial 1");
                // Open TutorialScreen
                UIManager.Ins.OpenUIDirectly(tutorialScreens[currentTutIndex]);
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
                return;
            }
        }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
        
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        {
            
        }

       
    }
}

using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel19", menuName = "ScriptableObjects/TutorialData/Lvl19", order = 1)]
    public class TutorialConditionLevel19 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if Player at cell 7, 9 -> Change to Cell that Player init in level 19
                if (!(Math.Abs(cell.WorldX - 7) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 9) < Constants.TOLERANCE)) return;
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

using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel3", menuName = "ScriptableObjects/TutorialData/Lvl3", order = 1)]
    public class TutorialConditionLevel3 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            }
            else if (currentTutIndex == 1)
            {
                if (triggerUnit is not Player) return;
               // If player move to cell (9-17) then show hint
               if (Math.Abs(cell.WorldX - 9) < TOLERANCE && Math.Abs(cell.WorldY - 17) < TOLERANCE)
               {
                   GameplayManager.Ins.OnFreePushHint(false, true);
                   currentTutIndex++;
               }
            }
        }

        private const double TOLERANCE = 0.01;

        public void OnForceShowTutorial(int index, bool isIncrement = true)
        { }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
    }
}

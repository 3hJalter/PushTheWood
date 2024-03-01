using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel2", menuName = "ScriptableObjects/TutorialData/Lvl2", order = 1)]
    public class TutorialConditionLevel2 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if player at cell 13,7
                if (!(Math.Abs(cell.WorldX - 13) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 7) < Constants.TOLERANCE)) return;
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            }
            
        }

        public void OnForceShowTutorial(int index, bool isIncrement = true)
        { }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
    }
}

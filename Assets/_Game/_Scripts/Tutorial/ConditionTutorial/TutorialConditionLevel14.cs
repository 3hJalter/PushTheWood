using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel14", menuName = "ScriptableObjects/TutorialData/Lvl14", order = 1)]
    public class TutorialConditionLevel14 : BaseTutorialData, ITutorialCondition
    {
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        {
            
        }

        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if player at cell 11, 13
                if (!(Math.Abs(cell.WorldX - 11) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 13) < Constants.TOLERANCE)) return;
                // Show tutorial
                UIManager.Ins.OpenUIDirectly(tutorialScreens[currentTutIndex]);
                currentTutIndex++;
            }
        }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
    }
}

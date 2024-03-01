using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel15", menuName = "ScriptableObjects/TutorialData/Lvl15", order = 1)]
    public class TutorialConditionLevel15 : BaseTutorialData, ITutorialCondition
    {
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        {
            
        }

        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if player at cell 11, 13 -> Change to Cell that Player init in level 15
                if (!(Math.Abs(cell.WorldX - 11) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 13) < Constants.TOLERANCE)) return;
                // Show tutorial
                UIManager.Ins.OpenUIDirectly(tutorialScreens[currentTutIndex]);
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            }
        }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
    }
}

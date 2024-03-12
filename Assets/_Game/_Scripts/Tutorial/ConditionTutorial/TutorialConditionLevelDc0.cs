using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevelDC0", menuName = "ScriptableObjects/TutorialData/LvlDC0", order = 1)]
    public class TutorialConditionLevelDc0 :  BaseTutorialData, ITutorialCondition
    {
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        { }

        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // if player at cell 9, 7 -> Change to Cell that Player init in Tutorial Daily Challenge
                if (!(Math.Abs(cell.WorldX - 9) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 7) < Constants.TOLERANCE)) return;
                // Show tutorial
                UIManager.Ins.OpenUIDirectly(tutorialScreens[currentTutIndex]);
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            }
        }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        { }
    }
}

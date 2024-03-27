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
    [CreateAssetMenu(fileName = "TutorialLevel5", menuName = "ScriptableObjects/TutorialData/Lvl5", order = 1)]
    public class TutorialConditionLevel5 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                // Open TutorialScreen
                UIManager.Ins.OpenUIDirectly(tutorialScreens[currentTutIndex]);
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            } else if (currentTutIndex == 1)
            {
                if (triggerUnit is not Player) return;
                // If player move to cell (17 13) then show hint
                if (Math.Abs(cell.WorldX - 17) < TOLERANCE && Math.Abs(cell.WorldY - 13) < TOLERANCE)
                {
                    GameplayManager.Ins.OnFreePushHint(false, true);
                    currentTutIndex++;
                }
            }
        }

        private const double TOLERANCE = 0.01;

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
        
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        {
            
        }

       
    }
}

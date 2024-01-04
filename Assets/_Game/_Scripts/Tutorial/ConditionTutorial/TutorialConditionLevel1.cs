using System;
using _Game._Scripts.Managers;
using _Game._Scripts.Tutorial;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel1", menuName = "ScriptableObjects/TutorialData/Lvl1", order = 1)]
    public class TutorialConditionLevel1 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (triggerUnit is not Player) return;
            switch (currentScreenIndex)
            {
                // If first screen is not showing
                case 0:
                {
                    // Player at cell 7,5
                    if (Math.Abs(cell.WorldX - 7) < TOLERANCE && Math.Abs(cell.WorldY - 5) < TOLERANCE) 
                    {
                        MoveInputManager.Ins.OnForceResetMove();
                        UICanvas ui = tutorialScreens[currentScreenIndex];
                        UIManager.Ins.OpenUIDirectly(ui);
                        currentScreenIndex++;
                    }

                    break;
                }
                // if currentScreenIndex == 1
                case 1:
                {
                    // if player at cell 7, 11
                    if (Math.Abs(cell.WorldX - 7) < TOLERANCE && Math.Abs(cell.WorldY - 11) < TOLERANCE) 
                    {
                        MoveInputManager.Ins.OnForceResetMove();
                        UICanvas ui = tutorialScreens[currentScreenIndex];
                        UIManager.Ins.OpenUIDirectly(ui);
                        currentScreenIndex++;
                    }

                    break;
                }
            }
        }

        private const double TOLERANCE = 0.01f;

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            
        }
    }
}

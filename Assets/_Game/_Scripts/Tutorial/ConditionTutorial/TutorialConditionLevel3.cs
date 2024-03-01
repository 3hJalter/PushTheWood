using System;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Chump;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using UnityEngine;
using Tree = _Game.GameGrid.Unit.StaticUnit.Tree;

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
                // if player at cell 13,7
                if (!(Math.Abs(cell.WorldX - 7) < Constants.TOLERANCE) ||
                    !(Math.Abs(cell.WorldY - 7) < Constants.TOLERANCE)) return;
                GameplayManager.Ins.OnFreePushHint(false, true);
                currentTutIndex++;
            }
            
        }

        public void OnForceShowTutorial(int index, bool isIncrement = true)
        { }

        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            if (triggerUnit is not Player p) return;
            if (currentTutIndex == 1)
            {
                if (targetUnit is Tree t)
                {
                    if (p.LastPushedDirection is Direction.Forward)
                    {
                        currentTutIndex++;
                    }
                    else
                    {
                        UIManager.Ins.OpenUIDirectly(tutorialScreens[0]);
                        currentTutIndex += 2; // To avoid the next condition
                    }
                }
            }
            else  if (currentTutIndex == 2)
            {
                if (targetUnit is Chump c)
                {
                    if (p.LastPushedDirection is Direction.Right)
                    {
                        currentTutIndex++;
                    }
                    else
                    {
                        UIManager.Ins.OpenUIDirectly(tutorialScreens[0]);
                        currentTutIndex ++; // To avoid the next condition
                    }
                }   
            }
        }
    }
}

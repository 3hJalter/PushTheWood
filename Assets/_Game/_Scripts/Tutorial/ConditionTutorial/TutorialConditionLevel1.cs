﻿using System;
using _Game._Scripts.Managers;
using _Game._Scripts.Tutorial;
using _Game._Scripts.Tutorial.ObjectTutorial;
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
        private BaseObjectTutorial glowSpot;
        private ArrowDirection arrowDirection;
        
        public void OnForceShowTutorial(int index, bool isIncrement = true)
        {
            if (index < 0 || index >= tutorialScreens.Count) return;
            currentScreenIndex = index;
            currentScreen = UIManager.Ins.OpenUIDirectly(tutorialScreens[currentScreenIndex]);
            if (isIncrement) currentScreenIndex++;
        }
        
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (triggerUnit is not Player) return;
            switch (currentScreenIndex)
            {
                // Case 0 already handle when the cutscene is playing
                case 1:
                {
                    // if player at cell 7, 11
                    if (Math.Abs(cell.WorldX - 7) < TOLERANCE && Math.Abs(cell.WorldY - 11) < TOLERANCE) 
                    {
                        MoveInputManager.Ins.OnForceResetMove();
                        currentScreen = UIManager.Ins.OpenUIDirectly(tutorialScreens[currentScreenIndex]);
                        currentScreenIndex++;
                        Destroy(glowSpot.gameObject);
                        Destroy(arrowDirection.gameObject);
                    }
                    else
                    {
                        if (currentScreen is not null)
                        {
                            currentScreen.CloseDirectly();
                            currentScreen = null;
                            glowSpot = Instantiate(TutorialManager.Ins.TutorialObjList[TutorialObj.LightSpot],
                                new Vector3(7,0,11), Quaternion.identity);
                            arrowDirection = (ArrowDirection) Instantiate(TutorialManager.Ins.TutorialObjList[TutorialObj.Arrow],
                                new Vector3(9,0,7), Quaternion.identity);
                            arrowDirection.PointerToHeight(2, true);
                        }
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

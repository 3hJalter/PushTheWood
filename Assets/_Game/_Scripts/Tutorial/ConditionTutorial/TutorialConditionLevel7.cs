﻿using _Game._Scripts.Managers;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Resource;
using UnityEngine;
using VinhLB;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel7", menuName = "ScriptableObjects/TutorialData/Lvl7", order = 1)]
    public class TutorialConditionLevel7 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (currentTutIndex == 0)
            {
                if (triggerUnit is not Player) return;
                int boosterUnlockIndex = DataManager.Ins.ConfigData.boosterConfigList[(int) BoosterType.GrowTree].UnlockAtLevel;
                if (!TutorialManager.Ins.IsOneTimeTutCompleted(boosterUnlockIndex))
                {
                    GameTutorialScreenPage ui = (GameTutorialScreenPage) UIManager.Ins.OpenUIDirectly(tutorialScreens[0]);
                    ui.OnCloseCallback += () =>
                    {
                        UIManager.Ins.OpenUI<OverlayScreen>().BoosterUnlockEffectUI.PlayUnlockEffect(BoosterType.GrowTree);                      
                        DataManager.Ins.GameData.user.completedOneTimeTutorial
                            .Add(boosterUnlockIndex); // UNDO index = 0
                    };
                }
                currentTutIndex++;
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

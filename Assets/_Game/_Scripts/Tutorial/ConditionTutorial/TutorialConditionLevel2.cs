using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using UnityEngine;

namespace _Game._Scripts.Tutorial.ConditionTutorial
{
    [CreateAssetMenu(fileName = "TutorialLevel2", menuName = "ScriptableObjects/TutorialData/Lvl2", order = 1)]
    public class TutorialConditionLevel2 : BaseTutorialData, ITutorialCondition
    {
        public void HandleShowTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            if (triggerUnit is Player)
            {
                if (currentScreenIndex == 0)
                {
                    
                }
            }
        }

        public void OnForceShowTutorial(int index, bool isIncrement = true)
        { }
        
        public void HandleShowTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        { }
    }
}

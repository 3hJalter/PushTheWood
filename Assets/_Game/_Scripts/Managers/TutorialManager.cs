using System.Collections.Generic;
using _Game._Scripts.Tutorial;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<int, ITutorialCondition> tutorialList = new();

        public void OnUnitGoToCell(GameGridCell cell, GridUnit triggerUnit)
        {
            // Try Get tutorial data == LevelManager.CurrentLevel.Index
            if (!tutorialList.TryGetValue(LevelManager.Ins.CurrentLevel.Index, out ITutorialCondition tutorialData)) return;
            // Show tutorial data
            tutorialData.HandleShowTutorial(cell, triggerUnit);
        }
        
        public void OnUnitActWithOther(GridUnit triggerUnit, GridUnit targetUnit)
        {
            // Try Get tutorial data == LevelManager.CurrentLevel.Index
            if (!tutorialList.TryGetValue(LevelManager.Ins.CurrentLevel.Index, out ITutorialCondition tutorialData)) return;
            // Show tutorial data
            tutorialData.HandleShowTutorial(triggerUnit, targetUnit);
        }
        
        [ContextMenu("Reset All Tutorial")]
        private void ResetAllTutorial()
        {
            foreach (KeyValuePair<int, ITutorialCondition> tutorial in tutorialList)
            {
                tutorial.Value.ResetTutorial();
            }
        }
    }
}

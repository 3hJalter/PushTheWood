using System;
using System.Collections.Generic;
using _Game._Scripts.Tutorial;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<int, ITutorialCondition> tutorialList = new();

        public Dictionary<int, ITutorialCondition> TutorialList => tutorialList;

        [SerializeField] private FirstCutsceneHandler firstCutscenePf;
        private readonly List<Transform> _objectOnCutscene = new();
        
        private void Start()
        {
            int index = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            if (index == 0) // TEMPORARY: need other way to handle this
            {
                DevLog.Log(DevId.Hoang, "Play Animation");
                Instantiate(firstCutscenePf, Tf).OnStartCutscene();
            }
        }

        public void AddCutsceneObject(Transform cutsceneObject)
        {
            cutsceneObject.SetParent(Tf);
            _objectOnCutscene.Add(cutsceneObject);
        }
        
        public void OnDestroyCutsceneObject()
        {
            for (int i = 0; i < _objectOnCutscene.Count; i++)
            {
                Destroy(_objectOnCutscene[i].gameObject);
            }
            _objectOnCutscene.Clear();
        }
        
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

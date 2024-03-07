﻿using System;
using System.Collections.Generic;
using _Game._Scripts.Tutorial;
using _Game._Scripts.Tutorial.ObjectTutorial;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Managers;
using _Game.Utilities;
using UnityEngine;

namespace _Game._Scripts.Managers
{
    public enum TutorialObj
    {
        LightSpot,
        Arrow,
    }
    
    public class TutorialManager : Singleton<TutorialManager>
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<TutorialObj, BaseObjectTutorial> tutorialObjList = new();
        
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<int, ITutorialCondition> tutorialList = new();
        
        public Dictionary<TutorialObj, BaseObjectTutorial> TutorialObjList => tutorialObjList;
        public Dictionary<int, ITutorialCondition> TutorialList => tutorialList;

        // TEMPORARY: cutscene
        [SerializeField] private FirstCutsceneHandler firstCutscenePf;
        public TutorialScreen currentTutorialScreenScreen;
        
        private void Awake()
        {
            actData = new Queue<TutorialInputOnActData>();
            moveData = new Queue<TutorialInputOnMoveData>();
        }

        private void Start()
        {
            GameManager.Ins.RegisterListenerEvent(EventID.StartGame, DequeueTutorialData);
            int index = DataManager.Ins.GameData.user.normalLevelIndex;
            if (index == 0) // TEMPORARY: need other way to handle this
            {
                DevLog.Log(DevId.Hoang, "Play Animation");
                Instantiate(firstCutscenePf, Tf).OnStartCutscene();
            }
        }

        private void OnDestroy()
        {
            GameManager.Ins.UnregisterListenerEvent(EventID.StartGame, DequeueTutorialData);
        }

        private void DequeueTutorialData()
        {
            if (moveData.Count > 0)
            {
                // deque all move data
                while (moveData.Count > 0)
                {
                    TutorialInputOnMoveData data = moveData.Dequeue();
                    OnGetMoveTutorial(data.cell, data.triggerUnit);
                }
            }
            if (actData.Count > 0)
            {
                while (actData.Count > 0)
                {
                    TutorialInputOnActData data = actData.Dequeue();
                    OnGetActTutorial(data.triggerUnit, data.targetUnit);
                }
            }
        }

        #region Tutorial shows by unit moving

        private Queue<TutorialInputOnMoveData> moveData;
        
        public void OnUnitMoveToCell(GameGridCell cell, GridUnit triggerUnit)
        {
            // TEMPORARY: CANCEL IF NOT NORMAL LEVEL
            if (LevelManager.Ins.CurrentLevel.LevelType != LevelType.Normal) return;
            // NOTE: If not in game state and a unit go to cell, save data
            if (!GameManager.Ins.IsState(GameState.InGame))
            {
                moveData.Enqueue(new TutorialInputOnMoveData{cell = cell, triggerUnit = triggerUnit});
                return;
            }
            OnGetMoveTutorial(cell, triggerUnit);
        }
        
        private void OnGetMoveTutorial(GameGridCell cell, GridUnit triggerUnit)
        {
            // Try Get tutorial data == LevelManager.CurrentLevel.Index
            if (!tutorialList.TryGetValue(LevelManager.Ins.CurrentLevel.Index, out ITutorialCondition tutorialData)) return;
            // Show tutorial data
            tutorialData.HandleShowTutorial(cell, triggerUnit);
        }

        #endregion

        #region Tutorial shows by unit interacting

        private Queue<TutorialInputOnActData> actData;
        
        public void OnUnitActWithOther(GridUnit triggerUnit, GridUnit targetUnit)
        {
            // TEMPORARY: CANCEL IF NOT NORMAL LEVEL
            if (LevelManager.Ins.CurrentLevel.LevelType != LevelType.Normal)
            {
                return;
            }
            // NOTE: If not in game state and a unit go to cell, save data
            if (!GameManager.Ins.IsState(GameState.InGame))
            {
                actData.Enqueue(new  TutorialInputOnActData{triggerUnit = triggerUnit, targetUnit = targetUnit});
                return;
            }
            OnGetActTutorial(triggerUnit, targetUnit);
        }

        private void OnGetActTutorial(GridUnit triggerUnit, GridUnit targetUnit)
        {
            // Try Get tutorial data == LevelManager.CurrentLevel.Index
            if (!tutorialList.TryGetValue(LevelManager.Ins.CurrentLevel.Index, out ITutorialCondition tutorialData)) return;
            // Show tutorial data
            tutorialData.HandleShowTutorial(triggerUnit, targetUnit);
        }

        #endregion
        
        [ContextMenu("Reset All Tutorial")]
        private void ResetAllTutorial()
        {
            foreach (KeyValuePair<int, ITutorialCondition> tutorial in tutorialList)
            {
                tutorial.Value.ResetTutorial();
            }
        }

        private record TutorialInputOnActData
        {
            public GridUnit triggerUnit;
            public GridUnit targetUnit;
        }

        private record TutorialInputOnMoveData
        {
            public GameGridCell cell;
            public GridUnit triggerUnit;
        }
    }
}

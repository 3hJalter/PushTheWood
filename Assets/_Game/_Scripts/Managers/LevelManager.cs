﻿using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Grid;
using UnityEngine;
using static _Game.Utilities.Grid.Grid<_Game.GameGrid.GameGridCell, _Game.GameGrid.GameGridCellData>;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        // private readonly Dictionary<int, Level> _preLoadLevels = new();

        // public Dictionary<int, Level> PreLoadLevels => _preLoadLevels;

        [SerializeField] private int levelIndex;
        private Level _currentLevel;
        //Test
        [SerializeField] Material FontMaterial;

        public Level CurrentLevel => _currentLevel;
        public bool IsConstructingLevel;

        private int _tutorialIndex;
        private CareTaker savingState;
        public Player player;

        private void Start()
        {
            // TEST
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, 0);
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            _tutorialIndex = PlayerPrefs.GetInt(Constants.TUTORIAL_INDEX, 0);
            GridUtilities.OverlayMaterial = FontMaterial;
            OnInit();
        }

        // private void CheckPreload()
        // {
        //     if (!_preLoadLevels.ContainsKey(levelIndex))
        //     {
        //         _currentLevel = new Level(levelIndex);
        //     }
        //     else
        //     {
        //         _currentLevel = _preLoadLevels[levelIndex];
        //         _preLoadLevels.Remove(levelIndex);
        //     }
        //     // Preload previous level
        //     if (levelIndex > 0 && !_preLoadLevels.ContainsKey(levelIndex - 1))
        //     {
        //         _preLoadLevels.Add(levelIndex - 1, new Level(levelIndex - 1));
        //     }
        //     // Preload next level
        //     if (levelIndex < DataManager.Ins.CountLevel - 1 && !_preLoadLevels.ContainsKey(levelIndex + 1))
        //     {
        //         _preLoadLevels.Add(levelIndex + 1, new Level(levelIndex + 1));
        //     }
        //     // Clear all other levels and remove it from preload list
        //     RemoveFarLevelFromPreLoad();
        // }

        // We only store the previous level and the next level, other levels will be removed from preload list
        // public void RemoveFarLevelFromPreLoad()
        // {
        //     List<int> keys = _preLoadLevels.Keys.ToList();
        //     foreach (int key in keys.Where(key => key != levelIndex - 1 && key != levelIndex && key != levelIndex + 1))
        //     {
        //         _preLoadLevels[key].OnDeSpawnLevel();
        //         _preLoadLevels.Remove(key);
        //     }
        // }

        public void OnInit()
        {
            // CheckPreload();
            IsConstructingLevel = true;
            _currentLevel = new Level(levelIndex);
            _currentLevel.OnInitLevelSurfaceAndUnit();
            _currentLevel.OnInitPlayerToLevel();
            // SetCameraToPlayer();
            _currentLevel.GridMap.CompleteObjectInit();
            IsConstructingLevel = false;
            savingState = new CareTaker(this);
            SetCameraToPlayerIsland();
            //NOTE: Test
            DebugManager.Ins?.DebugGridData(_currentLevel.GridMap);
            // CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetCenterPos());
        }

        public void SetCameraToPlayerIsland()
        {
            CameraManager.Ins.ChangeCameraTargetPosition(CurrentLevel.GetIsland(player.islandID).centerIslandPos);
        }

        public void OnWin()
        {
            // Show win screen
            UIManager.Ins.OpenUI<WinScreen>();
            // +1 LevelIndex and save
            levelIndex++;
            // Temporary handle when out of level
            if (levelIndex >= DataManager.Ins.CountLevel) levelIndex = 0;
            PlayerPrefs.SetInt(Constants.LEVEL_INDEX, levelIndex);
            // Future: Add reward collected in-game
        }

        public void OnGoLevel(int index)
        {
            if (levelIndex == index) return;
            levelIndex = index;
            IsConstructingLevel = true;
            _currentLevel.OnDeSpawnLevel();
            OnInit();
        }

        public void OnNextLevel()
        {
            // Load next level
            IsConstructingLevel = true;
            _currentLevel.OnDeSpawnLevel();
            OnInit();
            
            // OnChangeTutorialIndex();
        }

        private void OnChangeTutorialIndex()
        {
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, _tutorialIndex);
        }

        public void OnLose()
        {
            // Show lose screen
        }

        public void OnRestart()
        {
            CurrentLevel.ResetAllIsland();
            CurrentLevel.GridMap.Reset();
            player.OnDespawn();
            player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            player.OnInit(CurrentLevel.firstPlayerInitCell);
            SetCameraToPlayerIsland();
            // FxManager.Ins.ResetTrackedTrampleObjectList();
        }

        public void ResetGameState()
        {
            savingState.Reset();
        }
        public void OnUndo()
        {
            savingState.Undo();
            SetCameraToPlayerIsland();
        }
        public void SaveGameState(bool isMerge)
        {
            savingState.Save(isMerge);
        }
        private void SetCameraToPlayer()
        {
            // CameraFollow.Ins.SetTarget(Player.Tf);`
            CameraManager.Ins.ChangeCameraTarget(ECameraType.InGameCamera, player.Tf);
        }

        public class CareTaker
        {
            LevelManager main;
            Stack<IMemento> dataHistorys = new Stack<IMemento>();
            Stack<List<IMemento>> objectHistorys = new Stack<List<IMemento>>();
            public CareTaker(LevelManager main)
            {
                this.main = main;
                #region Init Spawn State
                SavingObjects();
                HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                foreach (GridUnit gridUnit in gridUnits)
                {
                    gridUnit.MainCell.ValueChange();
                }
                SavingState();
                #endregion
            }
            public void Undo()
            {
                if (main.CurrentLevel.GridMap.IsChange)
                {
                    SavingState(true);
                    if(objectHistorys.Count < dataHistorys.Count)
                        objectHistorys.Push(objectHistorys.Peek());
                }
                if (dataHistorys.Count > 0)
                {
                    dataHistorys.Pop().Restore();
                    foreach (IMemento objectRevert in objectHistorys.Pop())
                    {
                        objectRevert.Restore();
                    }
                    if (objectHistorys.Count == 0)
                    {
                        SavingObjects();
                    }
                    DevLog.Log(DevId.Hung, "UNDO_STATE - SUCCESS!!");
                }
                else
                {

                    DevLog.Log(DevId.Hung, "UNDO_STATE - FAILURE!!");
                }
            }

            public void Save(bool isMerge = false)
            {
                SavingState(isMerge);
                SavingObjects(isMerge);
            }
            private void SavingState(bool isMerge = false)
            {
                if (!isMerge || dataHistorys.Count == 0)
                {
                    if (main.CurrentLevel.GridMap.IsChange)
                    {
                        IMemento states = main._currentLevel.GridMap.Save();
                        dataHistorys.Push(states);
                    }
                }
                else
                {
                    GridMemento stateData = (GridMemento)dataHistorys.Peek();
                    stateData.Merge((GridMemento)main._currentLevel.GridMap.Save());
                }
            }
            private void SavingObjects(bool isMerge = false)
            {
                List<IMemento> states;
                if (!isMerge || objectHistorys.Count == 0)
                {
                    states = new List<IMemento>() { main.player.Save() };
                    HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                    foreach (GridUnit gridUnit in gridUnits)
                    {
                        if(gridUnit.IsSpawn)
                            states.Add(gridUnit.Save());
                    }
                    objectHistorys.Push(states);
                }
                else
                {
                    states = objectHistorys.Peek();
                    HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                    foreach (GridUnit gridUnit in gridUnits)
                    {
                        if(gridUnit.IsSpawn && !states.Any(x => x.Id == gridUnit.GetHashCode()))
                            states.Add(gridUnit.Save());
                    }
                }
            }
            public void Reset()
            {
                dataHistorys.Clear();
                objectHistorys.Clear();
                #region Init Spawn State
                SavingObjects();
                HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                foreach (GridUnit gridUnit in gridUnits)
                {
                    gridUnit.MainCell.ValueChange();
                }
                SavingState();
                #endregion
            }
        }
    }

}
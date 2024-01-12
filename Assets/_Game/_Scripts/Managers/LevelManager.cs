using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
using _Game._Scripts.Tutorial;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Grid;
using UnityEngine;
using VinhLB;
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
        [SerializeField]
        private FishSpawner _fishSpawner;

        public Level CurrentLevel => _currentLevel;
        public bool IsConstructingLevel;
        private bool isCanUndo = true;
        public bool IsCanUndo
        {
            get => isCanUndo;
            set
            {
                isCanUndo = value;
                UIManager.Ins.GetUI<InGameScreen>().SetActiveUndo(value);
            }
        }
        
        private CareTaker savingState;
        public Player player;

        private void Start()
        {
            // TEST
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            GridUtilities.OverlayMaterial = FontMaterial;
            OnGenerateLevel(levelIndex == 0);
            SetCameraToPosition(CurrentLevel.GetCenterPos());
        }

        public void OnGenerateLevel(bool needInit)
        {
            OnCheckTutorial();
            IsConstructingLevel = true;
            _currentLevel = new Level(levelIndex);
            if (needInit && !CurrentLevel.IsInit)
            {
                InitLevel();
            }
            
            _fishSpawner.SpawnFish();
        }

        public void InitLevel()
        {
            if (CurrentLevel.IsInit) return;
            _currentLevel.OnInitLevelSurfaceAndUnit();
            _currentLevel.OnInitPlayerToLevel();
            savingState = new CareTaker(this);
            _currentLevel.GridMap.CompleteObjectInit();
            IsConstructingLevel = false;
            savingState = new CareTaker(this);
            SetCameraToPlayerIsland();
            //NOTE: Test
            DebugManager.Ins?.DebugGridData(_currentLevel.GridMap);
            // TEMPORARY: CUTSCENE, player will be show when cutscene end
            if (levelIndex == 0) HidePlayer(true);
        }

        public void ResetLevelIsland()
        {
            _currentLevel.ResetIslandPlayerOn();
            _fishSpawner.SpawnFish(false);
        }

        private void OnCheckTutorial()
        {
            if (TutorialManager.Ins.TutorialList.TryGetValue(levelIndex, out ITutorialCondition tutorialData))
            {
                tutorialData.ResetTutorial();
            }
        }
        
        public void HidePlayer(bool isHide)
        {
            player.gameObject.SetActive(!isHide);
        }
        
        public void SetCameraToPlayerIsland()
        {
            SetCameraToIsland(player.islandID);
        }

        private void SetCameraToIsland(int index)
        {
            CameraManager.Ins.ChangeCameraTargetPosition(CurrentLevel.GetIsland(index).centerIslandPos);
        }
        
        private void SetCameraToPosition(Vector3 position)
        {
            CameraManager.Ins.ChangeCameraTargetPosition(position);
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
            OnGenerateLevel(true);
        }

        public void OnNextLevel()
        {
            // Load next level
            IsConstructingLevel = true;
            _currentLevel.OnDeSpawnLevel();
            // TEMPORARY: Destroy object on cutscene at level 1, need other way to handle this
            if (levelIndex - 1 == 0)
            {
                TutorialManager.Ins.OnDestroyCutsceneObject();
            }
            OnGenerateLevel(true);
            
            // OnChangeTutorialIndex();
        }

        public void OnLose()
        {
            // Show lose screen
        }

        public void OnRestart()
        {
            player.OnDespawn();
            CurrentLevel.ResetAllIsland();
            CurrentLevel.GridMap.Reset();
            // player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            // player.OnInit(CurrentLevel.firstPlayerInitCell);
            SetCameraToPlayerIsland();
            // FxManager.Ins.ResetTrackedTrampleObjectList();
        }

        public void ResetGameState()
        {
            savingState.Reset();
        }
        public void OnUndo()
        {
            if (!isCanUndo) return;
            savingState.Undo();
            SetCameraToPlayerIsland();
        }
        public void SaveGameState(bool isMerge)
        {
            savingState.Save(isMerge);
        }
        public void DiscardSaveState()
        {
            savingState.PopSave();
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
            public void PopSave()
            {
                dataHistorys.Pop();
                objectHistorys.Pop();
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
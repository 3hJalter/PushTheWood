using System;
using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.InGame;
using _Game._Scripts.Managers;
using _Game._Scripts.Tutorial;
using _Game.Camera;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Interface;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static _Game.Utilities.Grid.Grid<_Game.GameGrid.GameGridCell, _Game.GameGrid.GameGridCellData>;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        // private readonly Dictionary<int, Level> _preLoadLevels = new();

        // public Dictionary<int, Level> PreLoadLevels => _preLoadLevels;

        public event Action OnLevelGenerated;
        public event Action OnLevelRestarted;
        public event Action OnLevelIslandReset;
        public event Action OnCheckWinCondition;

        [SerializeField]
        [ReadOnly]
        private int normalLevelIndex;
        [SerializeField]
        [ReadOnly]
        private int secretLevelIndex;

        private Level _currentLevel;
        [SerializeField]


        public int NormalLevelIndex => normalLevelIndex;
        public Level CurrentLevel => _currentLevel;
        public bool IsConstructingLevel;


        private CareTaker savingState;
        
        [ReadOnly]
        public Player player;

        [ReadOnly] public List<IEnemy> enemies = new();
        
        // DEBUG:
        [ReadOnly]
        [SerializeField] private LevelWinCondition levelWinCondition;
        
        private void Start()
        {
            if (DebugManager.Ins && DebugManager.Ins.Level >= 0)
            {
                DataManager.Ins.GameData.user.normalLevelIndex = DebugManager.Ins.Level;
            }
            normalLevelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            secretLevelIndex = DataManager.Ins.GameData.user.secretLevelIndex;
            OnGenerateLevel(LevelType.Normal,normalLevelIndex == 0);
            SetCameraToPosition(CurrentLevel.GetCenterPos());
        }

        public void OnGenerateLevel(LevelType type, bool needInit)
        {
            if (type == LevelType.Normal) OnCheckTutorial();
            IsConstructingLevel = true;
            _currentLevel = new Level(type, normalLevelIndex);
            enemies.Clear();
            if (needInit && !_currentLevel.IsInit)
            {
                InitLevel();
            }
            OnLevelGenerated?.Invoke();
        }

        public void InitLevel()
        {
            levelWinCondition = CurrentLevel.LevelWinCondition;
            OnAddWinCondition(CurrentLevel.LevelWinCondition);
            GameManager.Ins.PostEvent(EventID.StartGame);
            if (_currentLevel.IsInit) return;
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
            if (normalLevelIndex == 0) HidePlayer(true);
        }

        public void ResetLevelIsland()
        {
            _currentLevel.ResetIslandPlayerOn();
            
            OnLevelIslandReset?.Invoke();
        }

        private void OnCheckTutorial()
        {
            if (TutorialManager.Ins.TutorialList.TryGetValue(normalLevelIndex, out ITutorialCondition tutorialData))
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
            switch (_currentLevel.LevelType)
            {
              case LevelType.Normal:
                  // +1 LevelIndex and save
                  normalLevelIndex++;
                  // Temporary handle when out of level
                  if (normalLevelIndex >= DataManager.Ins.CountNormalLevel) normalLevelIndex = 0;
                  DataManager.Ins.GameData.user.normalLevelIndex = normalLevelIndex;
                  break;
              case LevelType.Secret:
                  // +1 LevelIndex and save
                  secretLevelIndex++;
                  // Temporary handle when out of level
                  if (secretLevelIndex >= DataManager.Ins.CountSecretLevel) secretLevelIndex = 0; 
                  DataManager.Ins.GameData.user.secretLevelIndex = secretLevelIndex;
                  break;
              case LevelType.DailyChallenger:
                  // Check if contain
                  if (DataManager.Ins.GameData.user.dailyLevelIndexComplete.Contains(DateTime.Now.Day)) break;
                  DataManager.Ins.GameData.user.dailyLevelIndexComplete.Add(DateTime.Now.Day);
                  break;
              case LevelType.None:
              default:
                  break;
            }
            DataManager.Ins.Save();
            GameManager.Ins.PostEvent(EventID.WinGame);
            // Future: Add reward collected in-game
        }

        public void OnGoLevel(LevelType type, int index)
        {
            if (normalLevelIndex == index) return;
            normalLevelIndex = index;
            IsConstructingLevel = true;
            OnRemoveWinCondition();
            _currentLevel.OnDeSpawnLevel();
            OnGenerateLevel(type, true);
        }

        public void OnNextLevel(LevelType type, bool needInit = true)
        {
            // Load next level
            IsConstructingLevel = true;
            OnRemoveWinCondition();
            _currentLevel.OnDeSpawnLevel();
            // TEMPORARY: Destroy object on cutscene at level 1, need other way to handle this
            if (normalLevelIndex - 1 == 0)
            {
                TutorialManager.Ins.OnDestroyCutsceneObject();
            }
            OnGenerateLevel(type, true);
            // OnChangeTutorialIndex();
        }

        public void OnRestart()
        {
            player.OnDespawn();
            CurrentLevel.ResetAllIsland();
            CurrentLevel.ResetNonIslandUnit();
            CurrentLevel.GridMap.Reset();
            // player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            // player.OnInit(CurrentLevel.firstPlayerInitCell);
            SetCameraToPlayerIsland();
            // FxManager.Ins.ResetTrackedTrampleObjectList();
            savingState.Reset();
            
            OnLevelRestarted?.Invoke();
        }

        private void OnAddWinCondition(LevelWinCondition condition)
        {
            switch (condition)
            {
                case LevelWinCondition.DefeatAllEnemy:
                    OnCheckWinCondition += () =>
                    {
                        if (enemies.Count == 0)
                        {
                            OnWin();
                        }
                    };
                    break;
                case LevelWinCondition.FindingChest: 
                case LevelWinCondition.CollectAllStar:
                default:
                    break;
            }
            if (OnCheckWinCondition is not null) EventGlobalManager.Ins.OnEnemyDie.AddListener(OnCheckWinCondition);
        }
        
        private void OnRemoveWinCondition()
        {
            // Remove all win condition
            if (OnCheckWinCondition is not null)
            {
                EventGlobalManager.Ins.OnEnemyDie.RemoveListener(OnCheckWinCondition);
                OnCheckWinCondition = null;
            }
        }
        
        public void ResetGameState()
        {
            savingState.Reset();
        }

        public bool OnUndo()
        {
            bool success = savingState.Undo();
            SetCameraToPlayerIsland();
            return success;
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

            public bool Undo()
            {
                if (main.CurrentLevel.GridMap.IsChange)
                {
                    SavingState(true);
                    if (objectHistorys.Count < dataHistorys.Count)
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
                    return true;
                }
                DevLog.Log(DevId.Hung, "UNDO_STATE - FAILURE!!");
                return false;
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
                        if (gridUnit.IsSpawn && gridUnit.MainCell != null)
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
                        if (gridUnit.IsSpawn && !states.Any(x => x.Id == gridUnit.GetHashCode()))
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
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
using _Game.GameGrid.Unit.Interface;
using _Game.GameGrid.Unit.StaticUnit.Chest;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Timer;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
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
        public event Action OnLevelNext;
        public event Action OnObjectiveChange;

        [SerializeField]
        [ReadOnly]
        private int normalLevelIndex;
        [SerializeField]
        [ReadOnly]
        private int dailyLevelIndex;
        [SerializeField]
        [ReadOnly]
        private int secretLevelIndex;

        private Level _currentLevel;
        private bool _isRestarting;
        private bool _isResetting;
        public int KeyRewardCount = 0;
        public int SecretMapPieceCount = 0;
        public int NormalLevelIndex => normalLevelIndex;
        public int DailyLevelIndex => dailyLevelIndex;
        public int SecretLevelIndex => secretLevelIndex;
        public Level CurrentLevel => _currentLevel;
        public bool IsConstructingLevel;

        public bool IsFirstLevel => normalLevelIndex == 0;

        private CareTaker savingState;
        private readonly Vector3 _cameraDownOffset = new(0, 0, Constants.DOWN_CAMERA_CELL_OFFSET * Constants.CELL_SIZE);

        public Vector3 CameraDownOffset => _cameraDownOffset;

        public bool IsFirstTutorialLevel => (CurrentLevel.LevelType == LevelType.Normal && NormalLevelIndex == 0)
                                       || (CurrentLevel.LevelType == LevelType.DailyChallenge && DailyLevelIndex == 0);

        [ReadOnly]
        public Player player;

        #region Objective Win Condition Number

        [ReadOnly] [SerializeField] private int objectiveTotal;
        [ReadOnly]  private readonly HashSet<IEnemy> enemies = new();
        [ReadOnly]  private readonly HashSet<IFinalPoint> finalPoints = new();
        public int ObjectiveTotal => objectiveTotal;
        private int EnemyNums => enemies.Count;
        private int FinalPointNums => finalPoints.Count;
        
        public void OnAddEnemy(IEnemy enemy)
        {
            bool isAdded = enemies.Add(enemy);
            if (isAdded && CurrentLevel.LevelWinCondition is LevelWinCondition.DefeatAllEnemy)
            {
                if (enemies.Count > objectiveTotal) objectiveTotal = enemies.Count;
                OnObjectiveChange?.Invoke();
            }
        }

        public void OnRemoveEnemy(IEnemy enemy)
        {
            bool isRemoved = enemies.Remove(enemy);
            if (isRemoved && CurrentLevel.LevelWinCondition is LevelWinCondition.DefeatAllEnemy)
            {
                OnObjectiveChange?.Invoke();
                EventGlobalManager.Ins.OnChangeLevelCollectingObjectNumber.Dispatch();
            }
        }
        
       public void OnAddFinalPoint(IFinalPoint finalPoint)
        {
            bool isAdded = finalPoints.Add(finalPoint);
            if (isAdded && CurrentLevel.LevelWinCondition is not LevelWinCondition.DefeatAllEnemy)
            {
                if (finalPoints.Count > objectiveTotal) objectiveTotal = finalPoints.Count;
                OnObjectiveChange?.Invoke();
            }
        }
       
       public void OnRemoveFinalPoint(IFinalPoint finalPoint, bool checkWin = true)
        {
            bool isRemoved = finalPoints.Remove(finalPoint);
            if (isRemoved && CurrentLevel.LevelWinCondition is not LevelWinCondition.DefeatAllEnemy)
            {
                OnObjectiveChange?.Invoke();
                if (checkWin) EventGlobalManager.Ins.OnChangeLevelCollectingObjectNumber.Dispatch();
                else objectiveTotal--; // No check win  -> The FinalPoint is DeSpawn by undo or reset the level -> decrease the objectiveTotal to add them again later
            }
        }
       
        public int ObjectiveCounterLeft()
        {
            LevelWinCondition condition = _currentLevel.LevelWinCondition;
            if (condition is LevelWinCondition.DefeatAllEnemy)
            {
                return objectiveTotal - EnemyNums;
            }
            return objectiveTotal - FinalPointNums;
        }
        
        #endregion

        public HashSet<BChest> CollectedChests { get; } = new();

        // DEBUG:
        [ReadOnly]
        [SerializeField] private LevelWinCondition levelWinCondition;

        [SerializeField] private bool isSavePlayerPushStep;
        public bool IsSavePlayerPushStep => isSavePlayerPushStep;
        
        private void Start()
        {
            if (DebugManager.Ins && DebugManager.Ins.Level >= 0)
            {
                DataManager.Ins.GameData.user.normalLevelIndex = DebugManager.Ins.Level;
            }
            normalLevelIndex = DataManager.Ins.GameData.user.normalLevelIndex;
            secretLevelIndex = DataManager.Ins.GameData.user.secretLevelIndex;
            OnGenerateLevel(LevelType.Normal, normalLevelIndex, normalLevelIndex == 0);
            SetCameraToPosition(CurrentLevel.GetCenterPos());

            #region Handle If user passes first level

            if (DataManager.Ins.GameData.user.normalLevelIndex > 0) UIManager.Ins.OpenUI<MainMenuScreen>(true);

            #endregion
        }

        public void OnGenerateLevel(LevelType type, int index, bool needInit)
        {
            if (type == LevelType.Normal) OnCheckTutorial();
            IsConstructingLevel = true;
            _currentLevel = new Level(type, index);
            if (needInit && !_currentLevel.IsInit)
            {
                InitLevel();
            }
            OnLevelGenerated?.Invoke();
        }

        public void InitLevel()
        {   
            GameManager.Ins.PostEvent(EventID.StartGame);          
            player.SetActiveAgent(false);
            objectiveTotal = 0;
            ConstructingLevel();
            SetCameraToPlayerIsland();
            OnObjectiveChange?.Invoke();
            //NOTE: Test
            DebugManager.Ins?.DebugGridData(_currentLevel.GridMap);
            // TEMPORARY: CUTSCENE, player will be show when cutscene end
            if (normalLevelIndex == 0) HidePlayer(true);
            KeyRewardCount = 0;
            SecretMapPieceCount = 0;
        }

        public void ConstructingLevel()
        {
            if (_currentLevel.IsInit) return;
            levelWinCondition = CurrentLevel.LevelWinCondition;
            OnAddWinCondition(CurrentLevel.LevelWinCondition);
            enemies.Clear();
            finalPoints.Clear();
            CollectedChests.Clear();
            _currentLevel.OnInitLevelSurfaceAndUnit();
            _currentLevel.OnInitPlayerToLevel();
            _currentLevel.GridMap.CompleteObjectInit();
            IsConstructingLevel = false;
            savingState = new CareTaker(this);
        }

        public void ResetLevelIsland()
        {
            _isResetting = true;
            _currentLevel.ResetIslandPlayerOn();
            SetCameraToPlayerIsland();
            OnLevelIslandReset?.Invoke();
            _isResetting = false;
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
            if (player.islandID == -1) return;

            SetCameraToIsland(player.islandID);
        }

        private void SetCameraToIsland(int index)
        {
            Vector3 position = CurrentLevel.GetIsland(index).centerIslandPos + _cameraDownOffset;
            CameraManager.Ins.ChangeCameraTargetPosition(position);
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
                case LevelType.DailyChallenge:
                    // Check if contain
                    if (dailyLevelIndex == 0) break; // No store the tutorial level (index 0)
                    if (DataManager.Ins.GameData.user.dailyLevelIndexComplete.Contains(dailyLevelIndex)) break;
                    DataManager.Ins.GameData.user.dailyLevelIndexComplete.Add(dailyLevelIndex);
                    break;
                case LevelType.None:
                default:
                    break;
            }
            GameManager.Ins.GainRewardKey(KeyRewardCount);
            GameManager.Ins.GainSecretMapPiece(SecretMapPieceCount);
            if(CurrentLevel.LevelType == LevelType.Normal)
                GameManager.Ins.GainLevelProgress(1);
            DataManager.Ins.Save();
            GameManager.Ins.PostEvent(EventID.WinGame);
            // Future: Add reward collected in-game
        }

        public void OnGoLevel(LevelType type, int index, bool needInit = true)
        {
            // Dev: Currently make it work only for daily challenge
            switch (type)
            {
                case LevelType.Normal: normalLevelIndex = index; break;
                case LevelType.Secret: secretLevelIndex = index; break;
                case LevelType.DailyChallenge: dailyLevelIndex = index; break;
                case LevelType.None:
                default: break;
            }
            IsConstructingLevel = true;
            OnRemoveWinCondition();
            _currentLevel.OnDeSpawnLevel();
            OnGenerateLevel(type, index, needInit);
        }

        public void OnNextLevel(LevelType type, bool needInit = true)
        {
            // Load next level
            IsConstructingLevel = true;
            OnRemoveWinCondition();
            _currentLevel.OnDeSpawnLevel();
            OnGenerateLevel(type, normalLevelIndex, false);
            // Zoom out
            CameraManager.Ins.ChangeCamera(ECameraType.MainMenuCamera, 0f);
            // Hide the screen
            UIManager.Ins.HideUI<InGameScreen>();
            // SetCameraToPosition(CurrentLevel.GetCenterPos());
            if (!_currentLevel.IsInit)
            {
                InitLevel();
            }
            SetCameraToPlayerIsland();
            // Delay 2.5 second for zoom out
            TimerManager.Ins.WaitForTime(0.25f, () =>
            {
                OnLevelNext?.Invoke();
                CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
                TimerManager.Ins.WaitForTime(0.5f, () =>
                {
                    UIManager.Ins.ShowUI<InGameScreen>();
                });
            });
        }

        public void OnRestart()
        {
            if (!CurrentLevel.IsInit) return; // No Init -> Means No Restart
            _isRestarting = true;
            objectiveTotal = 0;
            enemies.Clear();
            finalPoints.Clear();
            CollectedChests.Clear();
            CurrentLevel.GridMap.Reset();
            IsConstructingLevel = true;
            player.OnDespawn();
            CurrentLevel.ResetAllIsland();
            CurrentLevel.ResetNonIslandUnit();
            player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            player.OnInit(CurrentLevel.FirstPlayerInitCell);
            SetCameraToPlayerIsland();
            // FxManager.Ins.ResetTrackedTrampleObjectList();
            IsConstructingLevel = false;
            savingState.Reset();
            OnLevelRestarted?.Invoke();
            OnObjectiveChange?.Invoke();
            _isRestarting = false;
        }

        private void OnAddWinCondition(LevelWinCondition condition)
        {
            switch (condition)
            {
                case LevelWinCondition.DefeatAllEnemy:
                    OnCheckWinCondition += () =>
                    {
                        if (EnemyNums == 0 && GameManager.Ins.IsState(GameState.InGame) && !_isRestarting && !_isResetting)
                        {
                            OnWin();
                        }
                    };
                    break;
                case LevelWinCondition.CollectAllChest: 
                case LevelWinCondition.FindingFruit:
                case LevelWinCondition.FindingChest:
                case LevelWinCondition.FindingChickenBbq:
                    OnCheckWinCondition += () =>
                    {
                        if (FinalPointNums == 0 && GameManager.Ins.IsState(GameState.InGame) && !_isRestarting && !_isResetting)
                        {
                            OnWin();
                        }
                    };
                    break;
            }

            if (OnCheckWinCondition is not null)
            {
                EventGlobalManager.Ins.OnEnemyDie.AddListener(OnCheckWinCondition);
                EventGlobalManager.Ins.OnChangeLevelCollectingObjectNumber.AddListener(OnCheckWinCondition);
            }
            
        }

        private void OnRemoveWinCondition()
        {
            // Remove all win condition
            if (OnCheckWinCondition is not null)
            {
                EventGlobalManager.Ins.OnEnemyDie.RemoveListener(OnCheckWinCondition);
                EventGlobalManager.Ins.OnChangeLevelCollectingObjectNumber.RemoveListener(OnCheckWinCondition);
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
            player.SetUpCamera(CurrentLevel.Islands[player.islandID], player.MainCell);
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
                    // TEMPORARY
                    SavingState(true);
                    if (objectHistorys.Count < dataHistorys.Count)
                        objectHistorys.Push(objectHistorys.Peek());
                }
                if (dataHistorys.Count > 1)
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
                    // TODO: (Later) Try to optimize this by not set the unit which are out of island (set unit island id to -1 before or check active...)
                    HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                    foreach (GridUnit gridUnit in gridUnits)
                    {
                        IMemento save = gridUnit.Save();
                        if (save != null && gridUnit.MainCell != null)
                            states.Add(save);
                    }
                    objectHistorys.Push(states);
                }
                else
                {
                    states = objectHistorys.Peek();
                    HashSet<GridUnit> gridUnits = main._currentLevel.Islands[main.player.islandID].GridUnits;
                    foreach (GridUnit gridUnit in gridUnits)
                    {
                        IMemento save = gridUnit.Save();
                        if (save != null && !states.Any(x => x.Id == gridUnit.GetHashCode()))
                            states.Add(save);
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
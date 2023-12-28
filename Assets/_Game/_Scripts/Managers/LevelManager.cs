using System;
using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.UIs.Tutorial;
using _Game.Camera;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Grid;
using DG.Tweening;
using GameGridEnum;
using MapEnum;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private int levelIndex;

        private readonly Dictionary<int, Island> _islandDic = new();
        private GameGridCell _firstPlayerInitCell;

        private GridSurface.GridSurface[,] _gridSurfaceMap;
        private TextGridData _textGridData;
        private int _tutorialIndex;
        private int gridSizeX;
        private int gridSizeY;
        private CareTaker savingState;
        public Grid<GameGridCell, GameGridCellData> GridMap { get; private set; }

        public Player Player { get; private set; }

        private void Start()
        {
            // TEST
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, 0);
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            _tutorialIndex = PlayerPrefs.GetInt(Constants.TUTORIAL_INDEX, 0);
            OnInit();
        }

        public void OnShowTutorial()
        {
            UIManager.Ins.OpenUI<TutorialScreen>()
                .LoadContext(Instantiate(DataManager.Ins.GetTutorial(_tutorialIndex)));
            _tutorialIndex++;
            if (_tutorialIndex >= DataManager.Ins.CountTutorial) _tutorialIndex = 0;
        }

        public void OnInit()
        {
            _textGridData = GameGridDataHandler.CreateGridData(levelIndex);
            CreateGridMap();
            SpawnGridSurfaceToGrid();
            AddIslandIdToSurface();
            SpawnGridUnitToGrid();
            SetCameraToPlayer();
            savingState = new CareTaker(this);
            savingState.SavingState();
            // SetCameraToPlayerIsland();
            // CameraManager.Ins.ChangeCameraTargetPosition(GetCenterPos());
        }

        public void SetCameraToPlayerIsland()
        {
            CameraManager.Ins.ChangeCameraTargetPosition(_islandDic[Player.islandID].GetCenterIslandPos());
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

        public void OnNextLevel()
        {
            // Load next level
            OnDespawnLevel();
            OnInit();
            OnChangeTutorialIndex();
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
            ResetAllIsland();
            Player.OnDespawn();
            Player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            Player.OnInit(_firstPlayerInitCell);
            savingState = new CareTaker(this);
            // FxManager.Ins.ResetTrackedTrampleObjectList();
        }
        public void OnUndo()
        {
            savingState.Undo();
        }

        public Vector3 GetCenterPos(float offsetY = 5f)
        {
            float centerX = (GridMap.GetGridCell(0, 0).WorldX + GridMap.GetGridCell(gridSizeX - 1, 0).WorldX) / 2;
            float centerZ = (GridMap.GetGridCell(0, 0).WorldY + GridMap.GetGridCell(0, gridSizeY - 1).WorldY) / 2;
            return new Vector3(centerX, offsetY, centerZ);
        }
        
        public GameGridCell GetCell(Vector2Int position)
        {
            return GridMap.GetGridCell(position.x, position.y);
        }

        public GameGridCell GetCellWorldPos(Vector3 position)
        {
            return GridMap.GetGridCell(position);
        }

        public void SetFirstPlayerStepOnIsland(GameGridCell cell)
        {
            _islandDic[Player.islandID].SetFirstPlayerStepCell(cell);
            CameraManager.Ins.ChangeCameraTargetPosition(_islandDic[Player.islandID].GetCenterIslandPos());
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            if (!_islandDic.ContainsKey(unit.islandID)) return;
            _islandDic[unit.islandID].AddNewUnitToIsland(unit);
        }

        public void ResetIslandPlayerOn()
        {
            if (!_islandDic.ContainsKey(Player.islandID)) return;
            _islandDic[Player.islandID].ResetIsland();
            Player.OnDespawn();
            Player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            Player.OnInit(_islandDic[Player.islandID].FirstPlayerStepCell);
        }

        private void ResetAllIsland()
        {
            for (int i = 0; i < _islandDic.Count; i++) _islandDic[i].ResetIsland();
        }

        private void OnDespawnLevel()
        {
            // Despawn all groundUnit
            for (int index0 = 0; index0 < _gridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < _gridSurfaceMap.GetLength(1); index1++)
            {
                GridSurface.GridSurface gridSurface = _gridSurfaceMap[index0, index1];
                if (gridSurface is not null) gridSurface.OnDespawn();
            }

            // Despawn all unit in each island
            for (int i = 0; i < _islandDic.Count; i++)
            {
                Island island = _islandDic[i];
                island.ClearIsland();
            }

            // Set player to null
            Player.OnDespawn();
            Player = null;
            _firstPlayerInitCell = null;
            // Clear all _islandDic data
            _islandDic.Clear();
            // Clear all _gridSurfaceMap data
            _gridSurfaceMap = null;
            // Clear all _gridMap data
            GridMap = null;
        }

        private void SetCameraToPlayer()
        {
            // CameraFollow.Ins.SetTarget(Player.Tf);`
            CameraManager.Ins.ChangeCameraTarget(ECameraType.InGameCamera, Player.Tf);
        }

        private void SpawnPlayerUnit(int x, int y, Direction direction)
        {
            GameGridCell cell = GridMap.GetGridCell(x, y);
            Player = SimplePool.Spawn<Player>(
                DataManager.Ins.GetGridUnit(PoolType.Player));
            Player.OnInit(cell, HeightLevel.One, true, direction);
            _islandDic[cell.Data.gridSurface.IslandID].SetFirstPlayerStepCell(cell);
            _firstPlayerInitCell = cell;
        }

        private void SpawnInitUnit(int x, int y, PoolType type, Direction direction)
        {
            GameGridCell cell = GridMap.GetGridCell(x, y);
            GridUnit unit = SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(type));
            unit.OnInit(cell, HeightLevel.One, true, direction);
            if (cell.Data.gridSurface == null) return;
            _islandDic[cell.Data.gridSurface.IslandID].AddInitUnitToIsland(unit, type, cell);
        }

        private void CreateGridMap()
        {
            Vector2Int size = _textGridData.GetSize();
            gridSizeX = size.x;
            gridSizeY = size.y;
            GridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE, default,
                () => new GameGridCell(), GridPlane.XZ);
        }

        private void SpawnGridSurfaceToGrid()
        {
            string[] surfaceData = _textGridData.SurfaceData.Split('\n');
            string[] surfaceRotationDirectionData = _textGridData.SurfaceRotationDirectionData.Split('\n');
            surfaceRotationDirectionData = surfaceRotationDirectionData.Skip(1).ToArray();
            string[] surfaceMaterialData = _textGridData.SurfaceMaterialData.Split('\n');
            surfaceMaterialData = surfaceMaterialData.Skip(1).ToArray();
            _gridSurfaceMap = new GridSurface.GridSurface[surfaceData.Length, surfaceData[0].Split(' ').Length];
            for (int x = 0; x < gridSizeX; x++)
            {
                string[] surfaceDataSplit = surfaceData[x].Split(' ');
                string[] surfaceRotationDirectionDataSplit = surfaceRotationDirectionData[x].Split(' ');
                string[] surfaceMaterialDataSplit = surfaceMaterialData[x].Split(' ');
                if (surfaceDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(surfaceDataSplit[y], out int cell)) continue;
                    if (!Enum.IsDefined(typeof(PoolType), cell)) continue;
                    GridSurface.GridSurface gridSurface = DataManager.Ins.GetGridSurface((PoolType)cell);
                    if (gridSurface is null) return;
                    GameGridCell gridCell = GridMap.GetGridCell(x, y);
                    GridSurface.GridSurface surfaceClone = SimplePool.Spawn<GridSurface.GridSurface>(gridSurface,
                        new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity);
                    gridCell.SetSurface(surfaceClone);
                    _gridSurfaceMap[x, y] = gridCell.Data.gridSurface;
                    
                    if (!int.TryParse(surfaceRotationDirectionDataSplit[y], out int directionSurface)) continue;
                    if (!Enum.IsDefined(typeof(Direction), directionSurface)) continue;
                    if (!int.TryParse(surfaceMaterialDataSplit[y], out int materialSurface)) continue;
                    if (!Enum.IsDefined(typeof(MaterialEnum), materialSurface)) continue;
                    surfaceClone.OnInit((Direction) directionSurface,
                        (MaterialEnum) materialSurface);
                }
            }
        }

        private void SpawnGridUnitToGrid()
        {
            string[] unitData = _textGridData.UnitData.Split('\n');
            // Remove the first line of unitData
            unitData = unitData.Skip(1).ToArray();
            string[] unitRotationDirectionData = _textGridData.UnitRotationDirectionData.Split('\n');
            unitRotationDirectionData = unitRotationDirectionData.Skip(1).ToArray();
            for (int x = 0; x < gridSizeX; x++)
            {
                string[] unitDataSplit = unitData[x].Split(' ');
                string[] unitRotationDirectionDataSplit = unitRotationDirectionData[x].Split(' ');
                if (unitDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(unitDataSplit[y], out int unitCell)) continue;
                    if (!Enum.IsDefined(typeof(PoolType), unitCell)) continue;

                    if (!int.TryParse(unitRotationDirectionDataSplit[y], out int directionCell)) continue;
                    if (!Enum.IsDefined(typeof(Direction), directionCell)) continue;

                    if ((PoolType)unitCell is PoolType.Player)
                        SpawnPlayerUnit(x, y, (Direction)directionCell);
                    else
                        SpawnInitUnit(x, y, (PoolType)unitCell, (Direction)directionCell);
                }
            }
        }

        private void AddIslandIdToSurface()
        {
            int currentIslandID = 0;
            for (int y = 0; y < _gridSurfaceMap.GetLength(1); y++)
            for (int x = 0; x < _gridSurfaceMap.GetLength(0); x++)
                if (IsGridSurfaceHadIsland(x, y, out GridSurface.GridSurface gridSurface))
                {
                    FloodFillIslandID(gridSurface, x, y, currentIslandID);
                    currentIslandID++;
                }

            return;

            void FloodFillIslandID(GridSurface.GridSurface gridSurface, int x, int y, int islandID)
            {
                gridSurface.IslandID = islandID;
                _islandDic.TryAdd(islandID, new Island(islandID));
                _islandDic[islandID].AddGridCell(GridMap.GetGridCell(x, y));
                if (IsGridSurfaceHadIsland(x - 1, y, out GridSurface.GridSurface leftGridSurface))
                    FloodFillIslandID(leftGridSurface, x - 1, y, islandID);
                if (IsGridSurfaceHadIsland(x + 1, y, out GridSurface.GridSurface rightGridSurface))
                    FloodFillIslandID(rightGridSurface, x + 1, y, islandID);
                if (IsGridSurfaceHadIsland(x, y - 1, out GridSurface.GridSurface downGridSurface))
                    FloodFillIslandID(downGridSurface, x, y - 1, islandID);
                if (IsGridSurfaceHadIsland(x, y + 1, out GridSurface.GridSurface upGridSurface))
                    FloodFillIslandID(upGridSurface, x, y + 1, islandID);
            }

            bool IsGridSurfaceHadIsland(int x, int y, out GridSurface.GridSurface gridSurface)
            {
                gridSurface = null;
                int rows = _gridSurfaceMap.GetLength(0);
                int cols = _gridSurfaceMap.GetLength(1);
                if (x < 0 || x >= rows || y < 0 || y >= cols) return false;
                gridSurface = _gridSurfaceMap[x, y];
                if (gridSurface is null) return false;
                if (gridSurface.SurfaceType == GridSurfaceType.Water) return false;
                return gridSurface.IslandID < 0;
            }
        }

        public class CareTaker
        {
            LevelManager main;
            Stack<IMemento[]> historys = new Stack<IMemento[]>();
            public CareTaker(LevelManager main)
            {
                this.main = main;
                main.Player.OnSavingState += SavingState;
            }
            public void Undo()
            {
                if(historys.Count > 0)
                {
                    IMemento[] states = historys.Pop();
                    foreach(IMemento state in states)
                    {
                        state.Restore();
                    }
                    DevLog.Log(DevId.Hung, "UNDO_STATE - SUCCESS!!");
                }
                else
                {
                    DevLog.Log(DevId.Hung, "UNDO_STATE - FAILURE!!");
                }
            }
            public void SavingState()
            {
                HashSet<GridUnit> gridUnits = main._islandDic[main.Player.islandID].GridUnits;
                IMemento[] states = new IMemento[gridUnits.Count + 1];
                int index = 1;
                states[0] = main.GridMap.Save();
                foreach(GridUnit gridUnit in gridUnits)
                {
                    states[index] = gridUnit.Save();
                }
            }
        }

    }

    public class Island
    {
        private readonly HashSet<GridUnit> _gridUnits = new();

        private readonly Dictionary<GameGridCell, PoolType> _initGridUnitDic = new();
        private readonly int _islandID;
        public HashSet<GridUnit> GridUnits => _gridUnits;
        public Island(int islandID)
        {
            _islandID = islandID;
        }

        public List<GameGridCell> GridCells { get; } = new();

        public GameGridCell FirstPlayerStepCell { get; private set; }
        
        public Vector3 GetCenterIslandPos()
        {
            Vector3 centerPos = Vector3.zero;
            for (int i = 0; i < GridCells.Count; i++) centerPos += GridCells[i].WorldPos;
            centerPos /= GridCells.Count;
            return centerPos;
        }
            
        public void SetFirstPlayerStepCell(GameGridCell cell)
        {
            FirstPlayerStepCell ??= cell;
        }

        public void AddGridCell(GameGridCell cell)
        {
            GridCells.Add(cell);
        }

        public void AddInitUnitToIsland(GridUnit unit, PoolType type, GameGridCell cell)
        {
            _gridUnits.Add(unit);
            _initGridUnitDic.Add(cell, type);
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            _gridUnits.Add(unit);
        }

        public void ClearIsland()
        {
            for (int i = 0; i < GridCells.Count; i++)
            {
                GameGridCell cell = GridCells[i];
                cell.ClearGridUnit();
            }

            foreach (GridUnit unit in _gridUnits.Where(unit => unit.gameObject.activeSelf))
            {
                if (unit.islandID != _islandID) continue;
                unit.OnDespawn();
            }

            _gridUnits.Clear();
        }

        public void ResetIsland()
        {
            DOTween.KillAll();
            ClearIsland();
            foreach (KeyValuePair<GameGridCell, PoolType> pair in _initGridUnitDic)
            {
                GridUnit unit =
                    SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }
        }
    }

    
}

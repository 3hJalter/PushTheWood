using System;
using System.Collections.Generic;
using System.Linq;
using _Game._Scripts.UIs.Tutorial;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities.Grid;
using DG.Tweening;
using GameGridEnum;
using MapEnum;
using UnityEngine;

namespace _Game.GameGrid
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private int levelIndex;
        private int _tutorialIndex;

        // Test
        [SerializeField] private Transform currentPCellViewer;

        private readonly Dictionary<int, Island> _islandDic = new();
        private GameGridCell _firstPlayerInitCell;
        private Grid<GameGridCell, GameGridCellData> _gridMap;

        public Grid<GameGridCell, GameGridCellData> GridMap => _gridMap;

        private GridSurface.GridSurface[,] _gridSurfaceMap;
        private Player _pUnit;
        private TextGridData _textGridData;
        private int gridSizeX;
        private int gridSizeY;

        public Player Player => _pUnit;
        
        private void Start()
        {
            // TEST
            PlayerPrefs.SetInt(Constants.TUTORIAL_INDEX, 0);
            // PlayerPrefs.SetInt(Constants.LEVEL_INDEX, 0);
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            _tutorialIndex = PlayerPrefs.GetInt(Constants.TUTORIAL_INDEX, 0);
        }

        public void OnShowTutorial()
        {
            UIManager.Ins.OpenUI<TutorialScreen>()
                .LoadContext(Instantiate(DataManager.Ins.GetTutorial(_tutorialIndex)));
            _tutorialIndex++;
            if (_tutorialIndex >= DataManager.Ins.CountTutorial) _tutorialIndex = 0;
        }

        //

        public void MoveCellViewer(GameGridCell cell)
        {
            currentPCellViewer.DOMove(cell.WorldPos + Vector3.up * 1.25f, 0.15f).SetEase(Ease.Linear);
        }

        public void ChangeCellViewer()
        {
            // set active false if is active now, and vice versa
            currentPCellViewer.gameObject.SetActive(!currentPCellViewer.gameObject.activeSelf);
        }

        public void OnInit()
        {
            _textGridData = GameGridDataHandler.CreateGridData(levelIndex);
            CreateGridMap();
            SpawnGridSurfaceToGrid();
            AddIslandIdToSurface();
            SpawnGridUnitToGrid();
            SetCameraToPlayer();
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
            _pUnit.OnDespawn();
            _pUnit = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            _pUnit.OnInit(_firstPlayerInitCell);
            currentPCellViewer.position = _firstPlayerInitCell.WorldPos + Vector3.up * 1.25f;
        }

        public GameGridCell GetCell(Vector2Int position)
        {
            return _gridMap.GetGridCell(position.x, position.y);
        }

        public GameGridCell GetCellWorldPos(Vector3 position)
        {
            return _gridMap.GetGridCell(position);
        }

        public void SetFirstPlayerStepOnIsland(GameGridCell cell)
        {
            _islandDic[_pUnit.islandID].SetFirstPlayerStepCell(cell);
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            if (!_islandDic.ContainsKey(unit.islandID)) return;
            _islandDic[unit.islandID].AddNewUnitToIsland(unit);
        }

        public void ResetIslandPlayerOn()
        {
            if (!_islandDic.ContainsKey(_pUnit.islandID)) return;
            _islandDic[_pUnit.islandID].ResetIsland();
            _pUnit.OnDespawn();
            _pUnit = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            _pUnit.OnInit(_islandDic[_pUnit.islandID].FirstPlayerStepCell);
            currentPCellViewer.position = _islandDic[_pUnit.islandID].FirstPlayerStepCell.WorldPos + Vector3.up * 1.25f;
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
            _pUnit.OnDespawn();
            _pUnit = null;
            _firstPlayerInitCell = null;
            // Clear all _islandDic data
            _islandDic.Clear();
            // Clear all _gridSurfaceMap data
            _gridSurfaceMap = null;
            // Clear all _gridMap data
            _gridMap = null;
        }

        private void SetCameraToPlayer()
        {
            CameraFollow.Ins.SetTarget(_pUnit.Tf);
        }

        private void SpawnPlayerUnit(int x, int y)
        {
            GameGridCell cell = _gridMap.GetGridCell(x, y);
            _pUnit = SimplePool.Spawn<Player>(
                DataManager.Ins.GetGridUnit(PoolType.Player));
            _pUnit.OnInit(cell);
            currentPCellViewer.position = cell.WorldPos + Vector3.up * 1.25f;
            _islandDic[cell.Data.gridSurface.IslandID].SetFirstPlayerStepCell(cell);
            _firstPlayerInitCell = cell;
        }

        private void SpawnInitUnit(int x, int y, PoolType type)
        {
            GameGridCell cell = _gridMap.GetGridCell(x, y);
            GridUnit unit = SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(type));
            unit.OnInit(cell);
            _islandDic[cell.Data.gridSurface.IslandID].AddInitUnitToIsland(unit, type, cell);
        }

        private void CreateGridMap()
        {
            Vector2Int size = _textGridData.GetSize();
            gridSizeX = size.x;
            gridSizeY = size.y;
            _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE, Tf.position,
                () => new GameGridCell(), GridPlane.XZ);
        }

        private void SpawnGridSurfaceToGrid()
        {
            string[] surfaceData = _textGridData.SurfaceData.Split('\n');
            _gridSurfaceMap = new GridSurface.GridSurface[surfaceData.Length, surfaceData[0].Split(' ').Length];
            for (int x = 0; x < gridSizeX; x++)
            {
                string[] surfaceDataSplit = surfaceData[x].Split(' ');
                if (surfaceDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(surfaceDataSplit[y], out int cell)) continue;
                    if (!Enum.IsDefined(typeof(PoolType), cell)) continue;
                    GridSurface.GridSurface gridSurface = DataManager.Ins.GetGridSurface((PoolType)cell);
                    if (gridSurface is null) continue;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    gridCell.SetSurface(
                        SimplePool.Spawn<GridSurface.GridSurface>(gridSurface,
                            new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity));
                    _gridSurfaceMap[x, y] = gridCell.Data.gridSurface;
                }
            }
        }

        private void SpawnGridUnitToGrid()
        {
            string[] unitData = _textGridData.UnitData.Split('\n');
            // Remove the first line of unitData
            unitData = unitData.Skip(1).ToArray();
            for (int x = 0; x < gridSizeX; x++)
            {
                string[] unitDataSplit = unitData[x].Split(' ');
                if (unitDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(unitDataSplit[y], out int cell)) continue;
                    if (!Enum.IsDefined(typeof(PoolType), cell)) continue;
                    if ((PoolType)cell is PoolType.Player)
                        SpawnPlayerUnit(x, y);
                    else
                        SpawnInitUnit(x, y, (PoolType)cell);
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
                _islandDic[islandID].AddGridCell(_gridMap.GetGridCell(x, y));
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
    }

    public class Island
    {
        private readonly List<GameGridCell> _gridCells = new();

        private readonly HashSet<GridUnit> _gridUnits = new();

        private readonly Dictionary<GameGridCell, PoolType> _initGridUnitDic = new();
        private readonly int _islandID;

        public Island(int islandID)
        {
            _islandID = islandID;
        }

        public GameGridCell FirstPlayerStepCell { get; private set; }

        public void SetFirstPlayerStepCell(GameGridCell cell)
        {
            FirstPlayerStepCell ??= cell;
        }

        public void AddGridCell(GameGridCell cell)
        {
            _gridCells.Add(cell);
        }

        public void AddInitUnitToIsland(GridUnit unit, PoolType type, GameGridCell cell)
        {
            _gridUnits.Add(unit);
            _initGridUnitDic.Add(cell, type);
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            if (_gridUnits.Contains(unit)) return;
            _gridUnits.Add(unit);
        }

        public void ClearIsland()
        {
            for (int i = 0; i < _gridCells.Count; i++)
            {
                GameGridCell cell = _gridCells[i];
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

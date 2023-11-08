using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.DynamicUnit;
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
        private int gridSizeX;
        private int gridSizeY;
        
        private readonly Dictionary<int, Island> _islandDic = new();
        private Grid<GameGridCell, GameGridCellData> _gridMap;
        private GridSurfaceBase[,] _gridSurfaceMap;
        private PlayerUnit _pUnit;
        private GameGridCell _firstPlayerInitCell;
        private TextGridData _textGridData;
        
        // Test
        [SerializeField] private Transform currentPCellViewer;

        public void SetPlayerDirection(Direction direction)
        {
            _pUnit.direction = direction;
        }
        //
        
        
        public void MoveCellViewer(GameGridCell cell)
        {
            currentPCellViewer.DOMove(cell.WorldPos + Vector3.up * 1.25f, 0.05f).SetEase(Ease.Linear);
        }

        public void ChangeCellViewer()
        {
            // set active false if is active now, and vice versa
            currentPCellViewer.gameObject.SetActive(!currentPCellViewer.gameObject.activeSelf);
        }
        
        private void Start()
        {
            // TEST
            levelIndex = PlayerPrefs.GetInt(Constants.LEVEL_INDEX, 0);
            OnInit();
        }

        private void OnInit()
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
        }

        public void OnLose()
        {
            // Show lose screen
        }
        
        public void OnRestart()
        {
            ResetAllIsland();
            _pUnit.OnDespawn();
            _pUnit = SimplePool.Spawn<PlayerUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player));
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
        
        public GameGridCell GetNeighbourCell(GameGridCell cell, Direction direction, int distance = 1)
        {
            Vector2Int cellPos = cell.GetCellPosition();
            Vector2Int dir = Constants.dirVector[direction];
            Vector2Int neighbourPos = cellPos + dir * distance;
            return _gridMap.GetGridCell(neighbourPos.x, neighbourPos.y);
            // return _gridMap.GetGridCell(x, y);
        }
        
        public void SetFirstPlayerStepOnIsland(GameGridCell cell)
        {
            _islandDic[_pUnit.islandID].SetFirstPlayerStepCell(cell);
        }
        
        public void AddNewUnitToIsland(GridUnit.GridUnit unit)
        {
            if (!_islandDic.ContainsKey(unit.islandID)) return;
            _islandDic[unit.islandID].AddNewUnitToIsland(unit);
        }
        
        public void ResetIslandPlayerOn()
        {
            if (!_islandDic.ContainsKey(_pUnit.islandID)) return;
            _islandDic[_pUnit.islandID].ResetIsland();
            _pUnit.OnDespawn();
            _pUnit = SimplePool.Spawn<PlayerUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player));
            _pUnit.OnInit(_islandDic[_pUnit.islandID].FirstPlayerStepCell);
            currentPCellViewer.position = _islandDic[_pUnit.islandID].FirstPlayerStepCell.WorldPos + Vector3.up * 1.25f;
        }
        
        private void ResetAllIsland()
        {
            for (int i = 0; i < _islandDic.Count; i++)
            {
                _islandDic[i].ResetIsland();
            }
        }
        
        private void OnDespawnLevel()
        {
            // Despawn all groundUnit
            for (int index0 = 0; index0 < _gridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < _gridSurfaceMap.GetLength(1); index1++)
            {
                GridSurfaceBase gridSurface = _gridSurfaceMap[index0, index1];
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
            _pUnit = SimplePool.Spawn<PlayerUnit>(
                DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player));
            _pUnit.OnInit(cell);
            currentPCellViewer.position = cell.WorldPos + Vector3.up * 1.25f;
            _islandDic[cell.Data.gridSurface.IslandID].SetFirstPlayerStepCell(cell);
            _firstPlayerInitCell = cell;
        }
        
        private void SpawnInitUnit<T>(int x, int y, T type)
        {
            GameGridCell cell = _gridMap.GetGridCell(x, y);
            switch (type)
            {
                case GridUnitDynamicType dT:
                {
                    GridUnitDynamic unit = SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(dT));
                    unit.OnInit(cell);
                    _islandDic[cell.Data.gridSurface.IslandID].AddInitUnitToIsland(unit, dT, cell);
                    break;
                }
                case GridUnitStaticType sT:
                {
                    GridUnitStatic unit = SimplePool.Spawn<GridUnitStatic>(DataManager.Ins.GetGridUnitStatic(sT));
                    unit.OnInit(cell);
                    _islandDic[cell.Data.gridSurface.IslandID].AddInitUnitToIsland(unit, sT, cell);
                    break;
                }
            }
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
            _gridSurfaceMap = new GridSurfaceBase[surfaceData.Length, surfaceData[0].Split(' ').Length];
            for (int x = 0; x < gridSizeX; x++)
            {
                string[] surfaceDataSplit = surfaceData[x].Split(' ');
                if (surfaceDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(surfaceDataSplit[y], out int cell)) continue;
                    if (!Enum.IsDefined(typeof(GridSurfaceType), cell)) continue;
                    GridSurfaceBase gridSurface = DataManager.Ins.GetGridSurface((GridSurfaceType)cell);
                    if (gridSurface is null) continue;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    gridCell.SetSurface(
                        SimplePool.Spawn<GridSurfaceBase>(gridSurface,
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
                    if (Enum.IsDefined(typeof(GridUnitDynamicType), cell))
                    {
                        if ((GridUnitDynamicType)cell is GridUnitDynamicType.Player)
                            SpawnPlayerUnit(x, y);
                        else
                            SpawnInitUnit(x, y, (GridUnitDynamicType)cell);
                    }
                    else if (Enum.IsDefined(typeof(GridUnitStaticType), cell))
                    {
                        SpawnInitUnit(x, y, (GridUnitStaticType)cell);
                    }
                }
            }
        }
        
        private void AddIslandIdToSurface()
        {
            int currentIslandID = 0;
            for (int y = 0; y < _gridSurfaceMap.GetLength(1); y++)
            for (int x = 0; x < _gridSurfaceMap.GetLength(0); x++)
                if (IsGridSurfaceHadIsland(x, y, out GridSurfaceBase gridSurface))
                {
                    FloodFillIslandID(gridSurface, x, y, currentIslandID);
                    currentIslandID++;
                }

            return;

            void FloodFillIslandID(GridSurfaceBase gridSurface, int x, int y, int islandID)
            {
                gridSurface.IslandID = islandID;
                _islandDic.TryAdd(islandID, new Island(islandID));
                _islandDic[islandID].AddGridCell(_gridMap.GetGridCell(x, y));
                if (IsGridSurfaceHadIsland(x - 1, y, out GridSurfaceBase leftGridSurface))
                    FloodFillIslandID(leftGridSurface, x - 1, y, islandID);
                if (IsGridSurfaceHadIsland(x + 1, y, out GridSurfaceBase rightGridSurface))
                    FloodFillIslandID(rightGridSurface, x + 1, y, islandID);
                if (IsGridSurfaceHadIsland(x, y - 1, out GridSurfaceBase downGridSurface))
                    FloodFillIslandID(downGridSurface, x, y - 1, islandID);
                if (IsGridSurfaceHadIsland(x, y + 1, out GridSurfaceBase upGridSurface))
                    FloodFillIslandID(upGridSurface, x, y + 1, islandID);
            }

            bool IsGridSurfaceHadIsland(int x, int y, out GridSurfaceBase gridSurface)
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
        private int _islandID;
        private readonly List<GameGridCell> _gridCells = new();

        private readonly HashSet<GridUnit.GridUnit> _gridUnits = new();

        // private readonly Dictionary<GridUnit.GridUnit, GameGridCell> _initGridUnitDic = new();
        private readonly Dictionary<GameGridCell, GridUnitDynamicType> _initGridDynamicDic = new();
        private readonly Dictionary<GameGridCell, GridUnitStaticType> _initGridStaticDic = new();
        private GameGridCell _firstPlayerStepCell;

        public Island(int islandID)
        {
            _islandID = islandID;
        }

        public GameGridCell FirstPlayerStepCell => _firstPlayerStepCell;

        public void SetFirstPlayerStepCell(GameGridCell cell)
        {
            _firstPlayerStepCell ??= cell;
        }

        public void AddGridCell(GameGridCell cell)
        {
            _gridCells.Add(cell);
        }

        public void AddInitUnitToIsland(GridUnitDynamic unit, GridUnitDynamicType type, GameGridCell cell)
        {
            _gridUnits.Add(unit);
            _initGridDynamicDic.Add(cell, type);
        }

        public void AddInitUnitToIsland(GridUnitStatic unit, GridUnitStaticType type, GameGridCell cell)
        {
            _gridUnits.Add(unit);
            _initGridStaticDic.Add(cell, type);
        }

        public void AddNewUnitToIsland(GridUnit.GridUnit unit)
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
            foreach (GridUnit.GridUnit unit in _gridUnits.Where(unit => unit.gameObject.activeSelf))
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
            foreach (KeyValuePair<GameGridCell, GridUnitDynamicType> pair in _initGridDynamicDic)
            {
                GridUnitDynamic unit =
                    SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }

            foreach (KeyValuePair<GameGridCell, GridUnitStaticType> pair in _initGridStaticDic)
            {
                GridUnitStatic unit = SimplePool.Spawn<GridUnitStatic>(DataManager.Ins.GetGridUnitStatic(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }
        }
    }
}

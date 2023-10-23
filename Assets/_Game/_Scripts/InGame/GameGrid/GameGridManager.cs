using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Managers;
using _Game.Utilities.Grid;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.DynamicUnit;
using GameGridEnum;
using MapEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game.GameGrid
{
    public class GameGridManager : Singleton<GameGridManager>
    {
        [SerializeField] private int gridSizeX;
        [SerializeField] private int gridSizeY;
        private readonly int _mapIndex = 0; // Will be use the saved data later
        private Grid<GameGridCell, GameGridCellData>.DebugGrid _debugGrid;
        private Grid<GameGridCell, GameGridCellData> _gridMap;
        private GridSurfaceBase[,] _gridSurfaceMap;
        private readonly Dictionary<int, Island> islandDic = new();
        // Test Init GridUnit
        private PlayerUnit _pUnit;
        private TextGridData _textGridData;
        
        // TODO: Learning tilemap in 3D, then try to create a scene to create map and save it as text file
    

        // TESTING
        private void Start()
        {
            OnInit();
        }

        private void Update()
        {
            // Reload this scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            }
        }

        private void OnInit()
        {
            _textGridData = GameGridDataHandler.CreateGridData(_mapIndex);
            CreateGridMap2();
            Debug.Log(_textGridData.UnitData);
            // TODO: Create ScriptableObject to store GridSurface and GridUnit prefab
            // --> Complete 1/2: GroundSurface prefab done, need GridUnit prefab
            // TODO: Spawn GridSurface from Data, and set GridSurface to GridCell with the same position
            // --> Complete 1/2: try to change to spawn by Pooling instead of Instantiate later
            SpawnGridSurfaceToGrid2();
            AddIslandIdToSurface();
            // TODO: Spawn GridUnit from Data, and set GridUnit to GridCell with the same position
            SpawnGridUnitToGrid2();
            // --> No Complete: need GridUnit prefab and data
            // TestInitGridUnit();
        }

        
        
        
        public void ResetIsland()
        {
            if (!islandDic.ContainsKey(_pUnit.islandID)) return;
            islandDic[_pUnit.islandID].ResetIsland(_pUnit);
        }

        private void SpawnPlayerUnit(int x, int y)
        {
            GameGridCell cell = _gridMap.GetGridCell(x, y);
            _pUnit = SimplePool.Spawn<PlayerUnit>(
                DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player));
            _pUnit.OnInit(cell);
            islandDic[cell.GetData().gridSurface.IslandID].SetFirstPlayerStepCell(cell);
        }

        public void SetFirstPlayerStepOnIsland(GameGridCell cell)
        {
            islandDic[_pUnit.islandID].SetFirstPlayerStepCell(cell);
        }

        public void AddNewUnitToIsland(GridUnit.GridUnit unit)
        {
            if (!islandDic.ContainsKey(unit.islandID)) return;
            islandDic[unit.islandID].AddNewUnitToIsland(unit);
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
                    // islandDic[cell.GetData().gridSurface.IslandID].AddInitUnitToIsland(unit);
                    islandDic[cell.GetData().gridSurface.IslandID].AddInitUnitToIsland(unit, dT, cell);
                    break;
                }
                case GridUnitStaticType sT:
                {
                    GridUnitStatic unit = SimplePool.Spawn<GridUnitStatic>(DataManager.Ins.GetGridUnitStatic(sT));
                    unit.OnInit(cell);
                    // islandDic[cell.GetData().gridSurface.IslandID].AddInitUnitToIsland(unit);
                    islandDic[cell.GetData().gridSurface.IslandID].AddInitUnitToIsland(unit, sT, cell);
                    break;
                }
            }
        }
        
        private void TestInitGridUnit()
        {
            SpawnPlayerUnit(6,7);
            SpawnInitUnit(5,5, GridUnitDynamicType.ChumpHigh);
            SpawnInitUnit(3,3, GridUnitDynamicType.ChumpShort);
            SpawnInitUnit(7,8, GridUnitDynamicType.ChumpShort);
            SpawnInitUnit(8,8, GridUnitStaticType.TreeShort);
            SpawnInitUnit(8,7, GridUnitDynamicType.ChumpHigh);
            SpawnInitUnit(3,4, GridUnitStaticType.TreeHigh);
            SpawnInitUnit(5,3, GridUnitStaticType.RockHigh);
        }


        private void CreateGridMap2()
        {
            Vector2Int size = _textGridData.GetSize();
            gridSizeX = size.x;
            gridSizeY = size.y;
            _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE, Tf.position,
                () => new GameGridCell(), GridPlane.XZ);
            _debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
            _debugGrid.DrawGrid(_gridMap);
        }

        private void SpawnGridSurfaceToGrid2()
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
                    _gridSurfaceMap[x, y] = gridCell.GetData().gridSurface;
                }
            }
        }

        private void SpawnGridUnitToGrid2()
        {
            string[] unitData = _textGridData.UnitData.Split('\n');
            // Remove the first line of unitData
            unitData = unitData.Skip(1).ToArray();
            for (int x = 0; x < gridSizeY; x++)
            {
                string[] unitDataSplit = unitData[x].Split(' ');
                if (unitDataSplit.Length != gridSizeY) continue;
                for (int y = 0; y < gridSizeY; y++)
                {
                    if (!int.TryParse(unitDataSplit[y], out int cell)) continue;
                    if (Enum.IsDefined(typeof(GridUnitDynamicType), cell))
                    {
                        if ((GridUnitDynamicType)cell is GridUnitDynamicType.Player)
                        {
                            SpawnPlayerUnit(x,y);
                        }
                        else
                        {
                            SpawnInitUnit(x,y, (GridUnitDynamicType) cell);
                        }
                        // GridUnitDynamic gridUnitDynamic = DataManager.Ins.GetGridUnitDynamic((GridUnitDynamicType)cell);
                        // if (gridUnitDynamic is null) continue;
                        // GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                        // GridUnitDynamic unit = SimplePool.Spawn<GridUnitDynamic>(gridUnitDynamic);
                        // unit.OnInit(gridCell);
                        // if (unit is PlayerUnit playerUnit) _pUnit = playerUnit;
                    }
                    else if (Enum.IsDefined(typeof(GridUnitStaticType), cell))
                    {
                        SpawnInitUnit(x,y,(GridUnitStaticType) cell);
                        // GridUnitStatic gridUnitStatic = DataManager.Ins.GetGridUnitStatic((GridUnitStaticType)cell);
                        // if (gridUnitStatic is null) continue;
                        // GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                        // SimplePool.Spawn<GridUnitStatic>(gridUnitStatic).OnInit(gridCell);
                    }
                }
            }
        }

        private void CreateGridMap()
        {
            _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE, Tf.position,
                () => new GameGridCell(), GridPlane.XZ);
            _debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
            _debugGrid.DrawGrid(_gridMap);
        }
        
        private void SpawnGridSurfaceToGrid()
        {
            string[] lines = _textGridData.SurfaceData.Split('\n');
            _gridSurfaceMap = new GridSurfaceBase[lines.Length, lines[0].Split(' ').Length];
            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y];
                // Split line into cells with type int
                string[] cells = line.Split(' ');
                for (int x = 0; x < cells.Length; x++)
                {
                    // Check if cells[x] can convert to int
                    if (!int.TryParse(cells[x], out int cell)) continue;
                    if (!Enum.IsDefined(typeof(GridSurfaceType), cell)) continue;
                    // Convert cell to GridSurfaceType and try get GridSurface prefab from Data
                    GridSurfaceBase gridSurface = DataManager.Ins.GetGridSurface((GridSurfaceType)cell);
                    if (gridSurface is null) continue;
                    // Instantiate and Set GridSurface to GridCell
                    // TODO: Change to Pooling and set Height based on HeightMap
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    gridCell.SetSurface(
                        SimplePool.Spawn<GridSurfaceBase>(gridSurface,
                            new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity));
                    _gridSurfaceMap[x, y] = gridCell.GetData().gridSurface;
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
                if (!islandDic.ContainsKey(islandID)) islandDic.Add(islandID, new Island(islandID));
                islandDic[islandID].AddGridCell(_gridMap.GetGridCell(x, y));
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

        private void SpawnGridUnitToGrid()
        {

        }

        public GameGridCell GetNeighbourCell(GameGridCell cell, Vector2Int direction, int distance = 1)
        {
            return null;
        }

        public GameGridCell GetCell(Vector2Int position)
        {
            return _gridMap.GetGridCell(position.x, position.y);
        }

        public GameGridCell GetNeighbourCell(GameGridCell cell, Direction direction, int distance = 1)
        {
            Vector2Int cellPos = cell.GetCellPosition();
            Vector2Int dir = Constants.dirVector[direction];
            Vector2Int neighbourPos = cellPos + dir * distance;
            return _gridMap.GetGridCell(neighbourPos.x, neighbourPos.y);
            // return _gridMap.GetGridCell(x, y);
        }
    }

    public class Island {
        private readonly List<GameGridCell> _gridCells = new();
        private GameGridCell _firstPlayerStepCell;
        // private readonly Dictionary<GridUnit.GridUnit, GameGridCell> _initGridUnitDic = new();
        private readonly Dictionary<GameGridCell, GridUnitDynamicType> _initGridDynamicDic = new();
        private readonly Dictionary<GameGridCell, GridUnitStaticType> _initGridStaticDic = new();
        private readonly HashSet<GridUnit.GridUnit> _gridUnits = new();
        
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

        public void ResetIsland(PlayerUnit playerUnit)
        {
            // clear all unit in each cells
            for (int i = 0; i < _gridCells.Count; i++)
            {
                GameGridCell cell = _gridCells[i];
                cell.ClearGridUnit();
            }
            // Despawn all unit that spawn when played
            Debug.Log("Number of added unit: " + _gridUnits.Count);
            foreach (GridUnit.GridUnit unit in _gridUnits.Where(unit => unit.gameObject.activeSelf))
            {
                unit.OnDespawn();
            }
            _gridUnits.Clear();
            foreach (KeyValuePair<GameGridCell, GridUnitDynamicType> pair in _initGridDynamicDic)
            {
                GridUnitDynamic unit = SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }
            foreach (KeyValuePair<GameGridCell, GridUnitStaticType> pair in _initGridStaticDic)
            {
                GridUnitStatic unit = SimplePool.Spawn<GridUnitStatic>(DataManager.Ins.GetGridUnitStatic(pair.Value));
                unit.OnInit(pair.Key);
                AddNewUnitToIsland(unit);
            }
            playerUnit.OnDespawn();
            playerUnit = SimplePool.Spawn<PlayerUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player));
            playerUnit.OnInit(_firstPlayerStepCell);
        }
        
        public Island(int islandID)
        {
        }
        
        
    }
}

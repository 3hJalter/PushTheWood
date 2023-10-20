using System;
using System.Collections.Generic;
using _Game.Managers;
using _Game.Utilities.Grid;
using _Game.DesignPattern;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.DynamicUnit;
using _Game.GameGrid.GridUnit.StaticUnit;
using GameGridEnum;
using MapEnum;
using UnityEngine;

namespace _Game.GameGrid
{
    public class GameGridManager : Singleton<GameGridManager>
    {
        [SerializeField] private int gridSizeX;
        [SerializeField] private int gridSizeY;
        private readonly Dictionary<int, List<GameGridCell>> _islandDic = new();
        private readonly int _mapIndex = 0; // Will be use the saved data later
        private Grid<GameGridCell, GameGridCellData>.DebugGrid _debugGrid;
        private Grid<GameGridCell, GameGridCellData> _gridMap;

        private GridSurfaceBase[,] _gridSurfaceMap;

        // Test Init GridUnit
        private GridUnitDynamic _gridUnit;
        private TextGridData _textGridData;

        // TODO: Learning tilemap in 3D, then try to create a scene to create map and save it as text file


        // TESTING
        private void Start()
        {
            OnInit();
        }

        private void OnInit()
        {
            CreateGridMap();
            _textGridData = GameGridDataHandler.CreateGridData(_mapIndex);
            Debug.Log(_textGridData.UnitData);
            // TODO: Create ScriptableObject to store GridSurface and GridUnit prefab
            // --> Complete 1/2: GroundSurface prefab done, need GridUnit prefab
            // TODO: Spawn GridSurface from Data, and set GridSurface to GridCell with the same position
            // --> Complete 1/2: try to change to spawn by Pooling instead of Instantiate later
            SpawnGridSurfaceToGrid();
            AddIslandIdToSurface();
            // TODO: Spawn GridUnit from Data, and set GridUnit to GridCell with the same position
            SpawnGridUnitToGrid();
            // --> No Complete: need GridUnit prefab and data
            TestInitGridUnit();
        }

        private void TestInitGridUnit()
        {
            GameGridCell cell = _gridMap.GetGridCell(5, 5);
            _gridUnit = SimplePool.Spawn<GridUnitDynamic>(
                DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpHigh));
            _gridUnit.OnInit(cell);
            cell = _gridMap.GetGridCell(3, 3);
            SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpShort))
                .OnInit(cell);
            cell = _gridMap.GetGridCell(6, 7);
            // SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpHigh)).OnInit(cell);
            SimplePool.Spawn<PlayerUnit>(
                DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Player)).OnInit(cell);
            cell = _gridMap.GetGridCell(8, 8);
            SimplePool.Spawn<TreeUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.TreeShort)).OnInit(cell);
            cell = _gridMap.GetGridCell(8, 7);
            SimplePool.Spawn<GridUnitDynamic>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpHigh))
                .OnInit(cell);
            cell = _gridMap.GetGridCell(3, 4);
            SimplePool.Spawn<TreeUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.TreeHigh)).OnInit(cell);

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
                if (!_islandDic.ContainsKey(islandID))
                    _islandDic.Add(islandID, new List<GameGridCell>());
                _islandDic[islandID].Add(_gridMap.GetGridCell(x, y));
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
}

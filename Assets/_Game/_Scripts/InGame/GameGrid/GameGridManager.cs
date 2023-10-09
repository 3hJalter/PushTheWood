using System;
using System.Collections.Generic;
using _Game._Scripts.DesignPattern;
using _Game._Scripts.Managers;
using _Game._Scripts.Utilities.Grid;
using _Game.GameGrid.GridSurface;
using GameGridEnum;
using MapEnum;
using UnityEngine;

namespace _Game.GameGrid
{
    public class GameGridManager : Singleton<GameGridManager>
    {
        private const int CELL_SIZE = 2;
        [SerializeField] private int gridSizeX;
        [SerializeField] private int gridSizeY;
        private readonly Dictionary<int, List<GameGridCell>> _islandDic = new();
        private readonly int _mapIndex = 0; // Will be use the saved data later
        private Grid<GameGridCell, GameGridCellData>.DebugGrid _debugGrid;
        private Grid<GameGridCell, GameGridCellData> _gridMap;
        private GridSurfaceBase[,] _gridSurfaceMap;
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
            // --> No Complete: need GridUnit prefab and data
        }

        private void CreateGridMap()
        {
            _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, CELL_SIZE, Tf.position,
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
                    GridSurfaceType gridSurfaceType = (GridSurfaceType)cell;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    GridSurfaceBase gridSurface = DataManager.Ins.GetGridSurface(gridSurfaceType);
                    if (gridSurface == null) continue;
                    // Instantiate and Set GridSurface to GridCell
                    gridCell.SetSurface(Instantiate(gridSurface, new Vector3(gridCell.WorldX, 0, gridCell.WorldY),
                        Quaternion.identity));
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
        }

        private bool IsGridSurfaceHadIsland(int x, int y, out GridSurfaceBase gridSurface)
        {
            gridSurface = null;
            int rows = _gridSurfaceMap.GetLength(0);
            int cols = _gridSurfaceMap.GetLength(1);
            if (x < 0 || x >= rows || y < 0 || y >= cols) return false;
            gridSurface = _gridSurfaceMap[x, y];
            if (gridSurface == null) return false;
            if (gridSurface.SurfaceType == GridSurfaceType.Water) return false;
            return gridSurface.IslandID < 0;
        }

        private void FloodFillIslandID(GridSurfaceBase gridSurface, int x, int y, int islandID)
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

        public GameGridCell GetNeighbourCell(GameGridCell cell, Direction direction)
        {
            Vector2Int cellPos = cell.GetCellPosition();
            int x = cellPos.x;
            int y = cellPos.y;
            switch (direction)
            {
                case Direction.Left:
                    x -= 1;
                    if (x < 0) return null;
                    break;
                case Direction.Down:
                    y -= 1;
                    if (y < 0) return null;
                    break;
                case Direction.Right:
                    x += 1;
                    if (x >= gridSizeX) return null;
                    break;
                case Direction.Up:
                    y += 1;
                    if (y >= gridSizeY) return null;
                    break;
                case Direction.None:
                default:
                    return null;
            }

            return _gridMap.GetGridCell(x, y);
        }
    }
}

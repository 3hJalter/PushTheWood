using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.Managers;
using _Game.UIs.Screen;
using _Game.Utilities;
using _Game.Utilities.Grid;
using GameGridEnum;
using HControls;
using MapEnum;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game._Scripts.Managers
{
    public class ChooseLevelManager : Singleton<ChooseLevelManager>
    {
        [SerializeField] private int currentLevelIndex;
        private Level _currentLevel;
        private Level _nextLevel;
        private Level _previousLevel;
        

        [SerializeField] private int initializeX = 0;
        [SerializeField] private float moveSpeed = 20f;
        
        private Direction _previousDirection = Direction.None;
        private void Update()
        {
            Direction direction = HInputManager.GetDirectionInput();
            switch (direction)
            {
                case Direction.None:
                    _previousDirection = Direction.None;
                    return;
                case Direction.Left or Direction.Right:
                    return;
            }
            // else move the CameraTargetPosZ
            if (_previousDirection is Direction.None)
            {
                if (direction is Direction.Forward)
                {
                    SeePreviousLevel();
                }
                else if (direction is Direction.Back)
                {
                    SeeNextLevel();
                }
            }
            _previousDirection = direction;
        }

        private void OnEnable()
        {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<ChooseLevelScreen>();
            OnFirstSpawnLevel(currentLevelIndex);
        }

        [ContextMenu("See Next Level")]
        public void SeeNextLevel()
        {
            int nextLevelIndex = currentLevelIndex + 1;
            if (nextLevelIndex >= DataManager.Ins.CountLevel) return;
            currentLevelIndex += 1;
            // Despawn previous level
            if (_previousLevel is not null)
            {
                _previousLevel.OnDeSpawnLevel();
                _previousLevel = null;
            }

            // Previous level become current level
            _previousLevel = _currentLevel;
            // Current level become next level
            _currentLevel = _nextLevel;
            // Spawn next level
            Vector3 nextLevelPos = new(0, 0, _currentLevel.GetMaxZPos() + 5f);
            _nextLevel = nextLevelIndex + 1 < DataManager.Ins.CountLevel
                ? new Level(nextLevelIndex + 1, nextLevelPos, false, initializeX)
                : null;
            // Change camera target position to center of first island of current level
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f);
        }

        [ContextMenu("See Previous Level")]
        public void SeePreviousLevel()
        {
            int previousLevelIndex = currentLevelIndex - 1;
            if (previousLevelIndex < 0) return;
            currentLevelIndex -= 1;
            // Despawn next level
            if (_nextLevel is not null)
            {
                _nextLevel.OnDeSpawnLevel();
                _nextLevel = null;
            }

            // Next level become current level
            _nextLevel = _currentLevel;
            // Current level become previous level
            _currentLevel = _previousLevel;
            // Spawn previous level
            Vector3 previousLevelPos = new(0, 0, _currentLevel.GetMinZPos() - 5f);
            _previousLevel = previousLevelIndex - 1 >= 0
                ? new Level(previousLevelIndex - 1, previousLevelPos, true, initializeX)
                : null;
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f, 1.5f);
        }

        private void OnFirstSpawnLevel(int index)
        {
            _currentLevel = new Level(index);
            CameraManager.Ins.ChangeCameraTargetPosition(_currentLevel.GetIsland(0).GetCenterIslandPos() + Vector3.up * 5f, 1.5f);
            initializeX = _currentLevel.GridSizeX;
            
            if (index - 1 >= 0)
            {
                
                Vector3 previousLevelPos = new(0, 0, _currentLevel.GetMinZPos() - 5f);
                _previousLevel = new Level(index - 1, previousLevelPos, true, initializeX);
            }

            if (index + 1 < DataManager.Ins.CountLevel)
            {
                Vector3 nextLevelPos = new(0, 0, _currentLevel.GetMaxZPos() + 5f);
                _nextLevel = new Level(index + 1, nextLevelPos, false, initializeX);
            }
            
           
        }
    }

    public class Level
    {
        private readonly Dictionary<int, Island> _islandDic = new();
        private readonly TextGridData _textGridData;
        private readonly int gridSizeX;

        public int GridSizeX => gridSizeX;

        private readonly int gridSizeY;

        private Grid<GameGridCell, GameGridCellData> _gridMap;

        // private GameGridCell _firstPlayerInitCell;
        private GridSurface[,] _gridSurfaceMap;
        // private Player player;

        public Island GetIsland(int islandID)
        {
            return _islandDic[islandID];
        }
        
        public Level(int index, Vector3 originPosition = default, bool reduceZPosOffset = false, int fistXSizeInit = 0)
        {
            Index = index;
            _textGridData = GameGridDataHandler.CreateGridData(index);
            gridSizeX = _textGridData.GetSize().x;
            gridSizeY = _textGridData.GetSize().y;
            DevLog.Log(DevId.Hoang, $"Island Id: {index}, SizeX: {gridSizeX}, SizeY: {gridSizeY}, Origin Position: {originPosition}");
            if (fistXSizeInit != 0) originPosition += Vector3.right * (fistXSizeInit - gridSizeX) * Constants.CELL_SIZE / 2;
            DevLog.Log(DevId.Hoang, $"Island Id: {index}, New Origin Position: {originPosition}");
            if (reduceZPosOffset) originPosition -= Vector3.forward * (gridSizeY * Constants.CELL_SIZE);
            _gridMap = new Grid<GameGridCell, GameGridCellData>(gridSizeX, gridSizeY, Constants.CELL_SIZE,
                originPosition,
                () => new GameGridCell(), GridPlane.XZ);
            SpawnGridSurfaceToGrid();
            AddIslandIdToSurface();
            SpawnPosAndRotGridUnitToGrid();
            // DEBUG: Spawn an cube at cell (0,0)
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = _gridMap.GetGridCell(0, 0).WorldPos;
            // DEBUG: Spawn an cube at cell (0, last Z pos)
            GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position =
                _gridMap.GetGridCell(0, gridSizeY - 1).WorldPos;
        }

        public int Index { get; }

        public void OnDeSpawnLevel()
        {
            // Despawn all groundUnit
            for (int index0 = 0; index0 < _gridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < _gridSurfaceMap.GetLength(1); index1++)
            {
                GridSurface gridSurface = _gridSurfaceMap[index0, index1];
                if (gridSurface is not null) gridSurface.OnDespawn();
            }

            // Despawn all unit in each island
            for (int i = 0; i < _islandDic.Count; i++)
            {
                Island island = _islandDic[i];
                island.ClearIsland();
            }

            // Set player to null
            // player.OnDespawn();
            // player = null;
            // _firstPlayerInitCell = null;
            // Clear all _islandDic data
            _islandDic.Clear();
            // Clear all _gridSurfaceMap data
            _gridSurfaceMap = null;
            // Clear all _gridMap data
            _gridMap = null;
        }
        
        public Vector3 GetCenterPos(float offsetY = 5f)
        {
            float centerX = (_gridMap.GetGridCell(0, 0).WorldX + _gridMap.GetGridCell(gridSizeX - 1, 0).WorldX) / 2;
            float centerZ = (_gridMap.GetGridCell(0, 0).WorldY + _gridMap.GetGridCell(0, gridSizeY - 1).WorldY) / 2;
            return new Vector3(centerX, offsetY, centerZ);
        }

        public float GetMaxZPos()
        {
            return _gridMap.GetGridCell(0, gridSizeY - 1).WorldY;
        }

        public float GetMinZPos()
        {
            return _gridMap.GetGridCell(0, 0).WorldY;
        }

        private void SpawnGridSurfaceToGrid()
        {
            string[] surfaceData = _textGridData.SurfaceData.Split('\n');
            string[] surfaceRotationDirectionData = _textGridData.SurfaceRotationDirectionData.Split('\n');
            surfaceRotationDirectionData = surfaceRotationDirectionData.Skip(1).ToArray();
            string[] surfaceMaterialData = _textGridData.SurfaceMaterialData.Split('\n');
            surfaceMaterialData = surfaceMaterialData.Skip(1).ToArray();
            _gridSurfaceMap = new GridSurface[surfaceData.Length, surfaceData[0].Split(' ').Length];
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
                    GridSurface gridSurface = DataManager.Ins.GetGridSurface((PoolType)cell);
                    if (gridSurface is null) return;
                    GameGridCell gridCell = _gridMap.GetGridCell(x, y);
                    GridSurface surfaceClone = SimplePool.Spawn<GridSurface>(gridSurface,
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

        private void AddIslandIdToSurface()
        {
            int currentIslandID = 0;
            for (int y = 0; y < _gridSurfaceMap.GetLength(1); y++)
            for (int x = 0; x < _gridSurfaceMap.GetLength(0); x++)
                if (IsGridSurfaceHadIsland(x, y, out GridSurface gridSurface))
                {
                    FloodFillIslandID(gridSurface, x, y, currentIslandID);
                    currentIslandID++;
                }

            return;

            void FloodFillIslandID(GridSurface gridSurface, int x, int y, int islandID)
            {
                gridSurface.IslandID = islandID;
                _islandDic.TryAdd(islandID, new Island(islandID));
                _islandDic[islandID].AddGridCell(_gridMap.GetGridCell(x, y));
                if (IsGridSurfaceHadIsland(x - 1, y, out GridSurface leftGridSurface))
                    FloodFillIslandID(leftGridSurface, x - 1, y, islandID);
                if (IsGridSurfaceHadIsland(x + 1, y, out GridSurface rightGridSurface))
                    FloodFillIslandID(rightGridSurface, x + 1, y, islandID);
                if (IsGridSurfaceHadIsland(x, y - 1, out GridSurface downGridSurface))
                    FloodFillIslandID(downGridSurface, x, y - 1, islandID);
                if (IsGridSurfaceHadIsland(x, y + 1, out GridSurface upGridSurface))
                    FloodFillIslandID(upGridSurface, x, y + 1, islandID);
            }

            bool IsGridSurfaceHadIsland(int x, int y, out GridSurface gridSurface)
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

        private void SpawnPosAndRotGridUnitToGrid()
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
                    if ((PoolType)unitCell is not PoolType.Player)
                        SpawnInitUnit(x, y, (PoolType)unitCell, (Direction)directionCell);
                }
            }

            return;

            void SpawnInitUnit(int x, int y, PoolType type, Direction direction)
            {
                GameGridCell cell = _gridMap.GetGridCell(x, y);
                GridUnit unit = SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(type));
                // unit.OnInit(cell, HeightLevel.One, true, direction);
                unit.OnSetPositionAndRotation(PredictUnitPos(), direction);
                unit.islandID = cell.Data.gridSurface.IslandID;
                if (cell.Data.gridSurface == null) return;
                _islandDic[cell.Data.gridSurface.IslandID].AddInitUnitToIsland(unit, type, cell);
                return;

                Vector3 PredictUnitPos()
                {
                    float offsetY = (float)HeightLevel.One / 2 * Constants.CELL_SIZE;
                    if (unit.UnitTypeY == UnitTypeY.Down) offsetY -= unit.yOffsetOnDown;
                    return cell.WorldPos + Vector3.up * offsetY;
                }
            }
        }
    }
}

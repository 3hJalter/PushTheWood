using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Data;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.GridSurface;
using _Game.GameGrid.Unit;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities.Grid;
using GameGridEnum;
using JetBrains.Annotations;
using MapEnum;
using UnityEngine;

namespace _Game._Scripts.InGame
{
    public class Level
    {
        #region data

        private bool _isInit; // Check if all unit is init in this Level
        
        // Init Data
        private readonly TextGridData _textGridData;

        // Map
        private Grid<GameGridCell, GameGridCellData> _gridMap;

        private readonly int gridSizeY;

        // Surface & Unit
        private GridSurface[,] _gridSurfaceMap;

        private readonly List<LevelUnitData> _unitDataList = new();

        // Island (Each island has some surfaces and units)
        private readonly Dictionary<int, Island> _islandDic = new();
        public Dictionary<int, Island> Islands => _islandDic;

        // Some other data
        public GameGridCell firstPlayerInitCell;
        public Direction firstPlayerDirection;
        
        // Get Data
        public int GridSizeX { get; }

        public int Index { get; }
        
        public bool IsInit => _isInit;
        
        public Grid<GameGridCell, GameGridCellData> GridMap => _gridMap;

        #endregion
        
        #region constructor

        public Level(int index, Transform parent = null)
        {
            _isInit = false;
            // Set GridMap
            Index = index;
            // Get data
            _textGridData = GameGridDataHandler.CreateGridData(index);
            GridSizeX = _textGridData.GetSize().x;
            gridSizeY = _textGridData.GetSize().y;
            // Create Grid Map
            CreateGridMap();
            // Spawn Grid Surface
            SpawnGridSurfaceToGrid();
            // Spawn Units (Not Init)
            OnSpawnUnits();
            // if isInit -> AddIsland & InitUnit
            if (parent is null) return;
            // Set parent
            SetParent(parent);
        }

        #endregion
        
        #region public function

        public void OnInitLevelSurfaceAndUnit()
        {
            AddIslandIdToSurface();
            for (int i = 0; i < _unitDataList.Count; i++) OnInitUnit(_unitDataList[i]);
            _isInit = true;
        }
        
        public Island GetIsland(int islandID)
        {
            return _islandDic[islandID];
        }

        public Vector3 GetCenterPos()
        {
            float centerX = (_gridMap.GetGridCell(0, 0).WorldX + _gridMap.GetGridCell(GridSizeX - 1, 0).WorldX) / 2;
            float centerZ = (_gridMap.GetGridCell(0, 0).WorldY + _gridMap.GetGridCell(0, gridSizeY - 1).WorldY) / 2;
            return new Vector3(centerX, 0, centerZ);
        }

        public float GetMaxZPos()
        {
            return _gridMap.GetGridCell(0, gridSizeY - 1).WorldY;
        }

        public float GetMinZPos()
        {
            return _gridMap.GetGridCell(0, 0).WorldY;
        }
        
        public void AddNewUnitToIsland(GridUnit unit)
        {
            if (!_islandDic.ContainsKey(unit.islandID)) return;
            _islandDic[unit.islandID].AddNewUnitToIsland(unit);
        }
        
        public GameGridCell GetCellWorldPos(Vector3 position)
        {
            return GridMap.GetGridCell(position);
        }
        
        public GameGridCell GetCell(Vector2Int position)
        {
            return GridMap.GetGridCell(position.x, position.y);
        }
        
        public void SetFirstPlayerStepOnIsland(GameGridCell cell)
        {
            _islandDic[LevelManager.Ins.player.islandID].SetFirstPlayerStepCell(cell);
            CameraManager.Ins.ChangeCameraTargetPosition(_islandDic[LevelManager.Ins.player.islandID].GetCenterIslandPos());
        }
        
        public void ResetIslandPlayerOn()
        {
            if (!_islandDic.ContainsKey(LevelManager.Ins.player.islandID)) return;
            _islandDic[LevelManager.Ins.player.islandID].ResetIsland();
            LevelManager.Ins.player.OnDespawn();
            LevelManager.Ins.player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            LevelManager.Ins.player.OnInit(_islandDic[LevelManager.Ins.player.islandID].FirstPlayerStepCell);
        }
        
        public void ResetAllIsland()
        {
            for (int i = 0; i < _islandDic.Count; i++) _islandDic[i].ResetIsland();
        }
        
        public void OnDeSpawnLevel()
        {
            // Despawn all groundUnit
            for (int index0 = 0; index0 < _gridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < _gridSurfaceMap.GetLength(1); index1++)
            {
                GridSurface gridSurface = _gridSurfaceMap[index0, index1];
                if (gridSurface is not null) gridSurface.OnDespawn();
            }
            
            if (_isInit)
            {
                // Despawn all unit in each island
                for (int i = 0; i < _islandDic.Count; i++)
                {
                    Island island = _islandDic[i];
                    island.ClearIsland();
                }
                // Set player to null
                LevelManager.Ins.player.OnDespawn();
                LevelManager.Ins.player = null;
            }
            else
            {
                for (int i = 0; i < _unitDataList.Count; i++)
                {
                    LevelUnitData data = _unitDataList[i];
                    data.unit.OnDespawn();
                }
            }
            
            firstPlayerInitCell = null;
            // Clear all _islandDic data
            _islandDic.Clear();
            // Clear all _gridSurfaceMap data
            _gridSurfaceMap = null;
            // Clear all _gridMap data
            _gridMap = null;
            // Clear all _unitDataList data
            _unitDataList.Clear();
            _isInit = false;
        }

        #endregion

        #region private function

        private void SetParent(Transform parent)
        {
            // set all gridSurface tp parent
            for (int index0 = 0; index0 < _gridSurfaceMap.GetLength(0); index0++)
            {
                for (int index1 = 0; index1 < _gridSurfaceMap.GetLength(1); index1++)
                {
                    GridSurface gridSurface = _gridSurfaceMap[index0, index1];
                    if (gridSurface is null) continue;
                    gridSurface.Tf.SetParent(parent);
                }
            }
            // set all gridUnit to parent
            for (int i = 0; i < _unitDataList.Count; i++)
            {
                LevelUnitData data = _unitDataList[i];
                data.unit.Tf.SetParent(parent);
            }
        }
        
        private void CreateGridMap()
        {
            string gridPositionData = _textGridData.GridPositionData;
            string[] vector2Data = gridPositionData.Split(' ');
            float xData = float.Parse(vector2Data[0]);
            float zData = float.Parse(vector2Data[1]);
            Vector3 originPos = new Vector3(xData, 0, zData);
            _gridMap = new Grid<GameGridCell, GameGridCellData>(GridSizeX, gridSizeY, Constants.CELL_SIZE,
                originPos, () => new GameGridCell(), GridPlane.XZ);
        }
        
        private void SpawnGridSurfaceToGrid()
        {
            string[] surfaceData = _textGridData.SurfaceData.Split('\n');
            surfaceData = surfaceData.Skip(1).ToArray();
            string[] surfaceRotationDirectionData = _textGridData.SurfaceRotationDirectionData.Split('\n');
            surfaceRotationDirectionData = surfaceRotationDirectionData.Skip(1).ToArray();
            string[] surfaceMaterialData = _textGridData.SurfaceMaterialData.Split('\n');
            surfaceMaterialData = surfaceMaterialData.Skip(1).ToArray();
            _gridSurfaceMap = new GridSurface[surfaceData.Length, surfaceData[0].Split(' ').Length];
            for (int x = 0; x < GridSizeX; x++)
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
                    surfaceClone.OnInit((Direction)directionSurface,
                        (MaterialEnum)materialSurface);
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

        private void OnSpawnUnits()
        {
            string[] unitData = _textGridData.UnitData.Split('\n');
            // Remove the first line of unitData
            unitData = unitData.Skip(1).ToArray();
            string[] unitRotationDirectionData = _textGridData.UnitRotationDirectionData.Split('\n');
            unitRotationDirectionData = unitRotationDirectionData.Skip(1).ToArray();
            for (int x = 0; x < GridSizeX; x++)
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
                        SpawnUnit(x, y, (PoolType)unitCell, (Direction)directionCell);
                    else
                    {
                        firstPlayerInitCell = _gridMap.GetGridCell(x, y);
                        firstPlayerDirection = (Direction) directionCell;
                    }
                        
                }
            }

            return;

            void SpawnUnit(int x, int y, PoolType type, Direction direction)
            {
                GameGridCell cell = _gridMap.GetGridCell(x, y);
                GridUnit unit = SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(type));
                _unitDataList.Add(new LevelUnitData
                {
                    mainCellIn = cell,
                    startHeightIn = HeightLevel.One,
                    directionIn = direction,
                    unitType = type,
                    unit = unit
                });
                unit.OnSetPositionAndRotation(PredictUnitPos(), direction);
                return;

                Vector3 PredictUnitPos()
                {
                    float offsetY = (float)HeightLevel.One / 2 * Constants.CELL_SIZE;
                    if (unit.UnitTypeY == UnitTypeY.Down) offsetY -= unit.yOffsetOnDown;
                    return cell.WorldPos + Vector3.up * offsetY;
                }
            }
        }

        public void OnInitPlayerToLevel()
        {
            if (LevelManager.Ins.player is not null) LevelManager.Ins.player.OnDespawn();
            LevelManager.Ins.player = SimplePool.Spawn<Player>(
                DataManager.Ins.GetGridUnit(PoolType.Player));
            LevelManager.Ins.player.OnInit(firstPlayerInitCell, HeightLevel.One, true, firstPlayerDirection);
            _islandDic[firstPlayerInitCell.Data.gridSurface.IslandID].SetFirstPlayerStepCell(firstPlayerInitCell);
        }
        
        private void OnInitUnit(LevelUnitData data)
        {
            data.unit.OnInit(data.mainCellIn, data.startHeightIn, true, data.directionIn, true);
            if (data.mainCellIn.Data.gridSurface == null) return;
            _islandDic[data.mainCellIn.Data.gridSurface.IslandID].AddInitUnitToIsland(data.unit, data.unitType, data.mainCellIn);
        }

        #endregion
        
        private struct LevelUnitData
        {
            public GameGridCell mainCellIn;
            public HeightLevel startHeightIn;
            public Direction directionIn;
            public PoolType unitType;
            public GridUnit unit;
        }

    }
}

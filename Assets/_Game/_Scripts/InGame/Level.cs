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
using DG.Tweening;
using GameGridEnum;
using MapEnum;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game._Scripts.InGame
{
    public class Level
    {
        #region constructor

        public Level(int index, Transform parent = null)
        {
            IsInit = false;
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
            OnSpawnShadowUnit();
            // if isInit -> AddIsland & InitUnit
            if (parent is null) return;
            // Set parent
            SetParent(parent);
        }

        #endregion

        public struct LevelUnitData
        {
            public GameGridCell mainCellIn;
            public HeightLevel startHeightIn;
            public Direction directionIn;
            public PoolType unitType;
            public GridUnit unit;
        }

        #region data

        // Init Data
        private readonly TextGridData _textGridData;

        // Map

        private readonly int gridSizeY;

        // Surface & Unit

        // Island (Each island has some surfaces and units)
        public Dictionary<int, Island> Islands { get; } = new();

        // Some other data
        public GameGridCell firstPlayerInitCell;
        public Direction firstPlayerDirection;

        // Get Data
        public int GridSizeX { get; }

        public int Index { get; }

        public bool IsInit { get; private set; }

        public GridSurface[,] GridSurfaceMap { get; private set; }

        public List<LevelUnitData> UnitDataList { get; } = new();

        public List<GridUnit> ShadowUnitList { get; } = new();

        public Grid<GameGridCell, GameGridCellData> GridMap { get; private set; }

        #endregion

        #region public function

        public void OnInitLevelSurfaceAndUnit()
        {
            AddIslandIdToSurface();
            for (int i = 0; i < UnitDataList.Count; i++) OnInitUnit(UnitDataList[i]);
            IsInit = true;
        }

        public Island GetIsland(int islandID)
        {
            return Islands[islandID];
        }

        public Vector3 GetCenterPos()
        {
            float centerX = (GridMap.GetGridCell(0, 0).WorldX + GridMap.GetGridCell(GridSizeX - 1, 0).WorldX) / 2;
            float centerZ = (GridMap.GetGridCell(0, 0).WorldY + GridMap.GetGridCell(0, gridSizeY - 1).WorldY) / 2;
            return new Vector3(centerX, 0, centerZ);
        }

        public float GetMaxZPos()
        {
            return GridMap.GetGridCell(0, gridSizeY - 1).WorldY;
        }

        public float GetMinZPos()
        {
            return GridMap.GetGridCell(0, 0).WorldY;
        }

        public void AddNewUnitToIsland(GridUnit unit)
        {
            if (!Islands.ContainsKey(unit.islandID)) return;
            Islands[unit.islandID].AddNewUnitToIsland(unit);
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
            Islands[LevelManager.Ins.player.islandID].SetFirstPlayerStepCell(cell);
            CameraManager.Ins.ChangeCameraTargetPosition(Islands[LevelManager.Ins.player.islandID].centerIslandPos);
        }

        public void ResetIslandPlayerOn()
        {
            if (!Islands.ContainsKey(LevelManager.Ins.player.islandID)) return;
            GridMap.Reset();
            LevelManager.Ins.IsConstructingLevel = true;
            Islands[LevelManager.Ins.player.islandID].ResetIsland();
            LevelManager.Ins.player.OnDespawn();
            LevelManager.Ins.player = SimplePool.Spawn<Player>(DataManager.Ins.GetGridUnit(PoolType.Player));
            LevelManager.Ins.player.OnInit(Islands[LevelManager.Ins.player.islandID].FirstPlayerStepCell);
            GridMap.CompleteObjectInit();
            LevelManager.Ins.IsConstructingLevel = false;
            LevelManager.Ins.ResetGameState();
        }

        public void ResetAllIsland()
        {
            for (int i = 0; i < Islands.Count; i++) Islands[i].ResetIsland();
        }

        public void OnDeSpawnLevel()
        {
            // Despawn all groundUnit
            for (int index0 = 0; index0 < GridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < GridSurfaceMap.GetLength(1); index1++)
            {
                GridSurface gridSurface = GridSurfaceMap[index0, index1];
                if (gridSurface is not null) gridSurface.OnDespawn();
            }

            for (int i = 0; i < ShadowUnitList.Count; i++) Object.Destroy(ShadowUnitList[i].gameObject);

            if (IsInit)
            {
                // Despawn all unit in each island
                for (int i = 0; i < Islands.Count; i++)
                {
                    Island island = Islands[i];
                    island.ClearIsland();
                }

                // Set player to null
                LevelManager.Ins.player.OnDespawn();
                LevelManager.Ins.player = null;
            }
            else
            {
                for (int i = 0; i < UnitDataList.Count; i++)
                {
                    LevelUnitData data = UnitDataList[i];
                    data.unit.OnDespawn();
                }
            }

            firstPlayerInitCell = null;
            // Clear all _islandDic data
            Islands.Clear();
            // Clear all _gridSurfaceMap data
            GridSurfaceMap = null;
            // Clear all _gridMap data
            GridMap = null;
            // Clear all _unitDataList data
            UnitDataList.Clear();
            IsInit = false;
        }

        private readonly Tween _tweenShadowUnitList;

        public void ChangeShadowUnitAlpha(bool isHide)
        {
            if (ShadowUnitList.Count == 0) return;
            // Kill the previous tween if they are running
            _tweenShadowUnitList?.Kill();
            float currentAlphaTransparency = ShadowUnitList[0].GetAlphaTransparency();

            if (isHide)
            {
                Tween tween = DOVirtual.Float(currentAlphaTransparency, 0, currentAlphaTransparency,
                        value => ShadowUnitList[0].SetAlphaTransparency(value))
                    .OnComplete(() => ShadowUnitList[0].gameObject.SetActive(false));
            }
            else
            {
                ShadowUnitList[0].gameObject.SetActive(true);
                Tween tween = DOVirtual.Float(currentAlphaTransparency, 0.5f, 0.5f - currentAlphaTransparency,
                    value => ShadowUnitList[0].SetAlphaTransparency(value));
            }

        }

        #endregion

        #region private function

        private void SetParent(Transform parent)
        {
            // set all gridSurface tp parent
            for (int index0 = 0; index0 < GridSurfaceMap.GetLength(0); index0++)
            for (int index1 = 0; index1 < GridSurfaceMap.GetLength(1); index1++)
            {
                GridSurface gridSurface = GridSurfaceMap[index0, index1];
                if (gridSurface is null) continue;
                gridSurface.Tf.SetParent(parent);
            }

            // set all gridUnit to parent
            for (int i = 0; i < UnitDataList.Count; i++)
            {
                LevelUnitData data = UnitDataList[i];
                data.unit.Tf.SetParent(parent);
            }
        }

        private void CreateGridMap()
        {
            GridMap = new Grid<GameGridCell, GameGridCellData>(GridSizeX, gridSizeY, Constants.CELL_SIZE,
                default, () => new GameGridCell(), GridPlane.XZ);
        }

        private void SpawnGridSurfaceToGrid()
        {
            string[] surfaceData = _textGridData.SurfaceData.Split('\n');
            string[] surfaceRotationDirectionData = _textGridData.SurfaceRotationDirectionData.Split('\n');
            surfaceRotationDirectionData = surfaceRotationDirectionData.Skip(1).ToArray();
            string[] surfaceMaterialData = _textGridData.SurfaceMaterialData.Split('\n');
            surfaceMaterialData = surfaceMaterialData.Skip(1).ToArray();
            GridSurfaceMap = new GridSurface[surfaceData.Length, surfaceData[0].Split(' ').Length];
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
                    GameGridCell gridCell = GridMap.GetGridCell(x, y);
                    GridSurface surfaceClone = SimplePool.Spawn<GridSurface>(gridSurface,
                        new Vector3(gridCell.WorldX, 0, gridCell.WorldY), Quaternion.identity);
                    gridCell.SetSurface(surfaceClone);
                    GridSurfaceMap[x, y] = gridCell.Data.gridSurface;

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
            for (int y = 0; y < GridSurfaceMap.GetLength(1); y++)
            for (int x = 0; x < GridSurfaceMap.GetLength(0); x++)
                if (IsGridSurfaceHadIsland(x, y, out GridSurface gridSurface))
                {
                    FloodFillIslandID(gridSurface, x, y, currentIslandID);
                    Islands[currentIslandID].SetIslandPos();
                    currentIslandID++;
                }

            return;

            void FloodFillIslandID(GridSurface gridSurface, int x, int y, int islandID)
            {
                gridSurface.IslandID = islandID;
                Islands.TryAdd(islandID, new Island(islandID));
                Islands[islandID].AddGridCell(GridMap.GetGridCell(x, y));
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
                int rows = GridSurfaceMap.GetLength(0);
                int cols = GridSurfaceMap.GetLength(1);
                if (x < 0 || x >= rows || y < 0 || y >= cols) return false;
                gridSurface = GridSurfaceMap[x, y];
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
                    {
                        SpawnUnit(x, y, (PoolType)unitCell, (Direction)directionCell);
                    }
                    else
                    {
                        if (LevelManager.Ins.player != null) LevelManager.Ins.player.OnDespawn();
                        firstPlayerInitCell = GridMap.GetGridCell(x, y);
                        firstPlayerDirection = (Direction)directionCell;
                        LevelManager.Ins.player = (Player) SpawnUnit(x, y, (PoolType)unitCell, (Direction)directionCell);
                    }

                }
            }

            return;

            GridUnit SpawnUnit(int x, int y, PoolType type, Direction direction)
            {
                GameGridCell cell = GridMap.GetGridCell(x, y);
                GridUnit unit = SimplePool.Spawn<GridUnit>(DataManager.Ins.GetGridUnit(type));
                UnitDataList.Add(new LevelUnitData
                {
                    mainCellIn = cell,
                    startHeightIn = HeightLevel.One,
                    directionIn = direction,
                    unitType = type,
                    unit = unit
                });
                unit.OnSetPositionAndRotation(PredictUnitPos(), direction);
                return unit;

                Vector3 PredictUnitPos()
                {
                    float offsetY = (float)HeightLevel.One / 2 * Constants.CELL_SIZE;
                    if (unit.UnitTypeY == UnitTypeY.Down) offsetY -= unit.yOffsetOnDown;
                    return cell.WorldPos + Vector3.up * offsetY;
                }
            }
        }

        private void OnSpawnShadowUnit()
        {
            // shadow unit has format x y z xAngle yAngle zAngle unitType
            // each shadow unit is split by '\n'
            string[] shadowUnitData = _textGridData.ShadowUnitData.Split('\n').Skip(1).ToArray();
            for (int i = 0; i < shadowUnitData.Length; i++)
            {
                string[] shadowUnitDataSplit = shadowUnitData[i].Split(' ');
                if (shadowUnitDataSplit.Length != 7) continue;
                if (!int.TryParse(shadowUnitDataSplit[6], out int unitCell)) continue;
                if (!Enum.IsDefined(typeof(PoolType), unitCell)) continue;
                PoolType type = (PoolType)unitCell;
                Vector3 position = new(float.Parse(shadowUnitDataSplit[0]), float.Parse(shadowUnitDataSplit[1]),
                    float.Parse(shadowUnitDataSplit[2]));
                Vector3 eulerAngle = new(float.Parse(shadowUnitDataSplit[3]), float.Parse(shadowUnitDataSplit[4]),
                    float.Parse(shadowUnitDataSplit[5]));
                GridUnit unit = Object.Instantiate(DataManager.Ins.GetGridUnit(type));
                unit.Tf.position = position;
                unit.Tf.eulerAngles = eulerAngle;
                unit.ChangeMaterial(DataManager.Ins.GetTransparentMaterial());
                unit.SetAlphaTransparency(0);
                ShadowUnitList.Add(unit);
                unit.gameObject.SetActive(false);
            }
        }

        public void OnInitPlayerToLevel()
        {
            LevelManager.Ins.player.OnInit(firstPlayerInitCell, HeightLevel.One, true, firstPlayerDirection);
            Islands[firstPlayerInitCell.Data.gridSurface.IslandID].SetFirstPlayerStepCell(firstPlayerInitCell);
        }

        private void OnInitUnit(LevelUnitData data)
        {
            data.unit.ResetData();
            data.unit.OnInit(data.mainCellIn, data.startHeightIn, true, data.directionIn, true);
            if (data.mainCellIn.Data.gridSurface == null) return;
            Islands[data.mainCellIn.Data.gridSurface.IslandID]
                .AddInitUnitToIsland(data.unit, data.unitType, data.mainCellIn);
        }

        #endregion
    }
}

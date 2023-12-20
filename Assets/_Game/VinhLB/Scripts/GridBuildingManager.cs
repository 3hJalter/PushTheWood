using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Managers;
using _Game.Utilities.Grid;
using GameGridEnum;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VinhLB
{
    public class GridBuildingManager : Singleton<GridBuildingManager>
    {
        private const string HOME_LEVEL_NAME = "L0";

        [Header("References")] [SerializeField]
        private BuildingUnitDatabaseSO _buildingUnitDatabaseSo;

        [Header("Settings")] [SerializeField] private bool _testing;

        [SerializeField] private bool _useKeyboard;

        [SerializeField] private bool _snapping;

        [SerializeField] private LayerMask _placeableLayerMask;

        private int _currentBuildingUnitDataId;
        private Direction _currentDirection;

        private Grid<GameGridCell, GameGridCellData> _homeGrid;
        private bool _isAnyChanged;

        private Vector3 _lastMousePosition;

        public bool Snapping => _snapping;
        public bool IsOnBuildingMode { get; private set; }

        public BuildingUnitData CurrentBuildingUnitData { get; private set; }

        private void Start()
        {
            if (_testing) OnInit();
        }

        private void Update()
        {
            HandleControls();
        }

        public event Action OnBuildingModeChanged;
        public event Action OnSelectedChanged;

        public void OnInit()
        {
            IsOnBuildingMode = false;
            _currentBuildingUnitDataId = -1;
            _currentDirection = Direction.Back;

            if (_testing)
            {
                LevelManager.Ins.OnInit();
                _homeGrid = LevelManager.Ins.GridMap;

                Grid<GameGridCell, GameGridCellData>.DebugGrid debugGrid = new();
                debugGrid.DrawGrid(_homeGrid, true);

                // CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
                CameraManager.Ins.ChangeCamera(ECameraType.InGameCamera);
            }
            else
            {
                _homeGrid = LevelManager.Ins.GridMap;
            }
        }

        public List<BuildingUnitData> GetPlacedObjectDataList()
        {
            return _buildingUnitDatabaseSo.BuildingUnitDataList;
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            return _homeGrid.GetWorldPosition(_lastMousePosition);
        }

        public Vector3 GetRotationOffset()
        {
            Vector2Int rotationOffset = BuildingUnitData.GetRotationOffset(CurrentBuildingUnitData.Width,
                CurrentBuildingUnitData.Height, _currentDirection);

            return new Vector3(rotationOffset.x, 0f, rotationOffset.y) * _homeGrid.CellSize;
        }

        public Quaternion GetPlacedObjectRotation()
        {
            return Quaternion.Euler(0, BuildingUnitData.GetRotationAngle(_currentDirection), 0);
        }

        public void ToggleBuildMode()
        {
            IsOnBuildingMode = !IsOnBuildingMode;

            if (IsOnBuildingMode)
            {
                if (_homeGrid != LevelManager.Ins.GridMap) _homeGrid = LevelManager.Ins.GridMap;
                _isAnyChanged = false;
                Utilities.TryGetCenterScreenPosition(out _lastMousePosition, _placeableLayerMask);
            }
            else
            {
                if (_isAnyChanged) GridMapFileUtilities.Save(HOME_LEVEL_NAME);
            }

            ChangeCurrentObjectDataId(-1);

            OnBuildingModeChanged?.Invoke();
        }

        public void ChangeCurrentObjectDataId(int id)
        {
            _currentBuildingUnitDataId = id;
            CurrentBuildingUnitData = _buildingUnitDatabaseSo.BuildingUnitDataList
                .FirstOrDefault(data => data.Id == _currentBuildingUnitDataId);

            _currentDirection = Direction.Back;

            OnSelectedChanged?.Invoke();
        }

        public void ChangePlaceDirection()
        {
            _currentDirection = BuildingUnitData.GetNextDirection(_currentDirection);
        }

        public void PlaceBuilding()
        {
            if (CanBuild())
            {
                GameGridCell gameGridCell = _homeGrid.GetGridCell(_lastMousePosition);
                BuildingUnit unit = SimplePool.Spawn<BuildingUnit>(
                    DataManager.Ins.GetGridUnit(CurrentBuildingUnitData.PoolType));
                unit.OnInit(gameGridCell, HeightLevel.One, true, _currentDirection);

                _isAnyChanged = true;
            }
        }

        public void DeleteBuilding()
        {
            GameGridCell gameGridCell = _homeGrid.GetGridCell(_lastMousePosition);
            if (gameGridCell != null && gameGridCell.HasBuilding())
            {
                gameGridCell.DestroyGridUnits();

                GridUnit unit = gameGridCell.GetGridUnitAtHeight(HeightLevel.One);
                List<GameGridCell> neighborCellList = unit.cellInUnits;
                for (int i = 0; i < neighborCellList.Count; i++) neighborCellList[i].ClearGridUnit();

                _isAnyChanged = true;
            }
        }

        public bool CanBuild()
        {
            if (CurrentBuildingUnitData == null) return false;

            bool canBuild = true;
            GameGridCell gameGridCell = _homeGrid.GetGridCell(_lastMousePosition);
            List<GameGridCell> neighborCellList = gameGridCell.GetCellsInsideUnit(_homeGrid,
                CurrentBuildingUnitData.Width, CurrentBuildingUnitData.Height, _currentDirection);
            for (int i = 0; i < neighborCellList.Count; i++)
                if (neighborCellList[i] == null ||
                    !neighborCellList[i].CanBuild(CurrentBuildingUnitData.BelowSurfaceType))
                {
                    canBuild = false;

                    break;
                }

            if (canBuild && CurrentBuildingUnitData.CheckAdjacentCells)
            {
                List<GameGridCell> adjacentCellList = gameGridCell.GetAdjacentCells(_homeGrid,
                    CurrentBuildingUnitData.Width, CurrentBuildingUnitData.Height, _currentDirection);
                canBuild = adjacentCellList.Count(cell =>
                               cell.Data.gridSurface is not null &&
                               cell.Data.gridSurface.SurfaceType == CurrentBuildingUnitData.AdjacentSurfaceType) >=
                           CurrentBuildingUnitData.MinAdjacentCells;
            }

            return canBuild;
        }

        private void HandleControls()
        {
            if (Input.GetKeyDown(KeyCode.B)) ToggleBuildMode();

            if (!IsOnBuildingMode) return;

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                Utilities.TryGetMouseWorldPosition(out _lastMousePosition, _placeableLayerMask);

            if (!_useKeyboard) return;

            if (Input.GetKeyDown(KeyCode.F)) PlaceBuilding();

            if (Input.GetKeyDown(KeyCode.D)) DeleteBuilding();

            if (Input.GetKeyDown(KeyCode.R)) ChangePlaceDirection();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                int newPlacedObjectDataId =
                    (_currentBuildingUnitDataId + 1) % _buildingUnitDatabaseSo.BuildingUnitDataList.Count;
                ChangeCurrentObjectDataId(newPlacedObjectDataId);
            }

            if (Input.GetKeyDown(KeyCode.S)) GridMapFileUtilities.Save(HOME_LEVEL_NAME);
        }
    }
}

using _Game.Camera;
using _Game.DesignPattern;
using _Game.GameGrid;
using _Game.Utilities.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class GridBuildingManager : Singleton<GridBuildingManager>
    {
        public event System.Action OnSelectedChanged;

        [Header("References")]
        [SerializeField]
        private PlacedObjectDatabaseSO _placedObjectDatabaseSO;

        [Header("Settings")]
        [SerializeField]
        private bool _debugging;
        [SerializeField]
        private bool _snapping;
        [SerializeField]
        private int _homeGridWidth;
        [SerializeField]
        private int _homeGridHeight;
        [SerializeField]
        private float _homeGridCellSize;

        private Grid<GameGridCell, GameGridCellData> _homeGrid;

        private bool _isOnBuildMode;
        private int _currentPlacedObjectDataIndex;
        private Direction _currentDirection;
        private Vector3 _lastMousePosition;

        public bool Snapping => _snapping;
        public bool IsOnBuildMode => _isOnBuildMode;

        public PlacedObjectData CurrentPlacedObjectData
        {
            get
            {
                if (_currentPlacedObjectDataIndex >= 0 &&
                    _currentPlacedObjectDataIndex < _placedObjectDatabaseSO.BuildingObjectDataList.Count)
                {
                    return _placedObjectDatabaseSO.BuildingObjectDataList[_currentPlacedObjectDataIndex];
                }

                return null;
            }
        }


        private void Awake()
        {
            //Vector3 originPosition = transform.position + new Vector3(-_homeGridWidth, 0, -_homeGridHeight) * _homeGridCellSize * 0.5f;
            //_homeGrid = new Grid<GridPlacedObject, GameGridCellData>(_homeGridWidth, _homeGridHeight, _homeGridCellSize, originPosition,
            //    () => new GridPlacedObject(), MapEnum.GridPlane.XZ);
        }

        private void Start()
        {
            _isOnBuildMode = false;
            _currentPlacedObjectDataIndex = -1;
            _currentDirection = Direction.Back;

            LevelManager.Ins.OnInit();
            _homeGrid = LevelManager.Ins.GridMap;

            if (_debugging)
            {
                var debugGrid = new Grid<GameGridCell, GameGridCellData>.DebugGrid();
                debugGrid.DrawGrid(_homeGrid, true);
            }

            CameraFollow.Ins.ChangeCamera(ECameraType.InGameCamera);
        }

        private void Update()
        {
            HandleControls();
        }

        public void ToggleBuildMode()
        {
            _isOnBuildMode = !_isOnBuildMode;

            if (_isOnBuildMode)
            {
                Utilities.TryGetCenterScreenPosition(out _lastMousePosition);
            }

            _currentPlacedObjectDataIndex = -1;

            OnSelectedChanged?.Invoke();
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            Vector2Int rotationOffset = CurrentPlacedObjectData.GetRotationOffset(_currentDirection);
            Vector3 buildingObjectPosition = _homeGrid.GetWorldPosition(_lastMousePosition) +
                new Vector3(rotationOffset.x, 0, rotationOffset.y) * _homeGrid.CellSize;

            return buildingObjectPosition;
        }

        public Quaternion GetPlacedObjectRotation()
        {
            return Quaternion.Euler(0, PlacedObjectData.GetRotationAngle(_currentDirection), 0);
        }

        public bool CanBuild()
        {
            bool canBuild = true;
            (int i, int j) = _homeGrid.GetGridPosition(_lastMousePosition);
            List<Vector2Int> gridPositionList = CurrentPlacedObjectData.GetGridPositionList(new Vector2Int(i, j), _currentDirection);
            for (int p = 0; p < gridPositionList.Count; p++)
            {
                GameGridCell gameGridCell = _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y);
                if (gameGridCell == null || !gameGridCell.Data.CanBuild())
                {
                    canBuild = false;

                    break;
                }
            }

            return canBuild;
        }

        private void HandleControls()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBuildMode();
            }

            if (!_isOnBuildMode)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Utilities.TryGetMouseWorldPosition(out _lastMousePosition);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                (int i, int j) = _homeGrid.GetGridPosition(_lastMousePosition);

                if (CanBuild())
                {
                    Vector2Int rotationOffset = CurrentPlacedObjectData.GetRotationOffset(_currentDirection);
                    Vector3 buildingObjectPosition = _homeGrid.GetWorldPosition(i, j) +
                        new Vector3(rotationOffset.x, 0, rotationOffset.y) * _homeGrid.CellSize;
                    PlacedObject placedObject = PlacedObject.Create(CurrentPlacedObjectData,
                        buildingObjectPosition, new Vector2Int(i, j), _currentDirection);
                    placedObject.OnInit(_homeGrid.GetGridCell(i, j), GameGridEnum.HeightLevel.One, true,
                        new Vector3(rotationOffset.x, 0, rotationOffset.y) * _homeGrid.CellSize * 0.5f);

                    List<Vector2Int> gridPositionList = CurrentPlacedObjectData.GetGridPositionList(new Vector2Int(i, j), _currentDirection);
                    for (int p = 0; p < gridPositionList.Count; p++)
                    {
                        GameGridCell gameGridCell = _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y);
                        gameGridCell.Data.PlacedObject = placedObject;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (Utilities.TryGetMouseWorldPosition(out Vector3 mousePosition))
                {
                    GameGridCell gameGridCell = _homeGrid.GetGridCell(mousePosition);
                    if (gameGridCell != null && gameGridCell.Data.PlacedObject != null)
                    {
                        gameGridCell.Data.PlacedObject.DestroySelf();

                        List<Vector2Int> gridPositionList = gameGridCell.Data.PlacedObject.GetGridPositionList();

                        for (int p = 0; p < gridPositionList.Count; p++)
                        {
                            GameGridCell ggc = _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y);
                            if (ggc is GridPlacedObject gpo)
                            {
                                gpo.SetPlacedObject(null);
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentDirection = PlacedObjectData.GetNextDirection(_currentDirection);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _currentPlacedObjectDataIndex = (_currentPlacedObjectDataIndex + 1) % _placedObjectDatabaseSO.BuildingObjectDataList.Count;

                _currentDirection = Direction.Back;

                OnSelectedChanged?.Invoke();
            }
        }
    }

    public class GridPlacedObject : GameGridCell
    {
        private PlacedObject _placedObject;

        public PlacedObject PlacedObject => _placedObject;

        public void SetPlacedObject(PlacedObject placedObject)
        {
            _placedObject = placedObject;
        }

        public bool CanBuild()
        {
            return _placedObject == null;
        }
    }
}

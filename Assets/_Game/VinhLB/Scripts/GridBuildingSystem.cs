using _Game.daivq.Utilities;
using _Game.GameGrid;
using _Game.GameGrid.Unit;
using _Game.Utilities.Grid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class GridBuildingSystem : MonoSingleton<GridBuildingSystem>
    {
        public event System.Action OnSelectedChanged;

        [Header("References")]
        [SerializeField]
        private PlacedObjectDatabaseSO _placedObjectDatabaseSO;

        [Header("Settings")]
        [SerializeField]
        private bool _snapping;
        [SerializeField]
        private int _homeGridWidth;
        [SerializeField]
        private int _homeGridHeight;
        [SerializeField]
        private float _homeGridCellSize;

        private Grid<GridPlacedObject, GameGridCellData> _homeGrid;

        private bool _isOnBuildMode;
        private int _currentPlacedObjectDataIndex;
        private Direction _currentDirection;

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


        protected override void Awake()
        {
            base.Awake();

            Vector3 originPosition = transform.position + new Vector3(-_homeGridWidth, 0, -_homeGridHeight) * _homeGridCellSize * 0.5f;
            _homeGrid = new Grid<GridPlacedObject, GameGridCellData>(_homeGridWidth, _homeGridHeight, _homeGridCellSize, originPosition,
                () => new GridPlacedObject(), MapEnum.GridPlane.XZ);
        }

        private void Start()
        {
            _isOnBuildMode = false;
            _currentPlacedObjectDataIndex = -1;
            _currentDirection = Direction.Back;

            var debugGrid = new Grid<GridPlacedObject, GameGridCellData>.DebugGrid();
            debugGrid.DrawGrid(_homeGrid, true);
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
                _currentPlacedObjectDataIndex = 0;
            }
            else
            {
                _currentPlacedObjectDataIndex = -1;
            }

            OnSelectedChanged?.Invoke();
        }

        public Vector3 GetMouseWorldSnappedPosition()
        {
            if (Utilities.TryGetMouseWorldPosition(out Vector3 mousePosition))
            {
                Vector2Int rotationOffset = CurrentPlacedObjectData.GetRotationOffset(_currentDirection);
                Vector3 buildingObjectPosition = _homeGrid.GetWorldPosition(mousePosition) +
                    new Vector3(rotationOffset.x, 0, rotationOffset.y) * _homeGrid.CellSize;

                return buildingObjectPosition;
            }

            return Vector3.zero;
        }

        public Quaternion GetPlacedObjectRotation()
        {
            return Quaternion.Euler(0, PlacedObjectData.GetRotationAngle(_currentDirection), 0);
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

            if (Input.GetMouseButtonDown(0))
            {
                if (Utilities.TryGetMouseWorldPosition(out Vector3 mousePosition))
                {
                    (int i, int j) = _homeGrid.GetGridPosition(mousePosition);

                    List<Vector2Int> gridPositionList = CurrentPlacedObjectData.GetGridPositionList(new Vector2Int(i, j), _currentDirection);

                    bool canBuild = true;
                    for (int p = 0; p < gridPositionList.Count; p++)
                    {
                        GridPlacedObject gridPlacedObject = _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y);
                        if (gridPlacedObject == null || !gridPlacedObject.CanBuild())
                        {
                            canBuild = false;

                            break;
                        }
                    }

                    if (canBuild)
                    {
                        Vector2Int rotationOffset = CurrentPlacedObjectData.GetRotationOffset(_currentDirection);
                        Vector3 buildingObjectPosition = _homeGrid.GetWorldPosition(i, j) +
                            new Vector3(rotationOffset.x, 0, rotationOffset.y) * _homeGrid.CellSize;
                        PlacedObject placedObject = PlacedObject.Create(CurrentPlacedObjectData,
                            buildingObjectPosition, new Vector2Int(i, j), _currentDirection);
                        placedObject.OnInit(_homeGrid.GetGridCell(i, j), GameGridEnum.HeightLevel.One);

                        for (int p = 0; p < gridPositionList.Count; p++)
                        {
                            _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y).SetPlacedObject(placedObject);
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (Utilities.TryGetMouseWorldPosition(out Vector3 mousePosition))
                {
                    GridPlacedObject gridPlacedObject = _homeGrid.GetGridCell(mousePosition);
                    if (gridPlacedObject != null && gridPlacedObject.PlacedObject != null)
                    {
                        gridPlacedObject.PlacedObject.DestroySelf();

                        List<Vector2Int> gridPositionList = gridPlacedObject.PlacedObject.GetGridPositionList();

                        for (int p = 0; p < gridPositionList.Count; p++)
                        {
                            _homeGrid.GetGridCell(gridPositionList[p].x, gridPositionList[p].y).SetPlacedObject(null);
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

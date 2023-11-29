using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class GridBuildingSystem : HMonoBehaviour
    {
        [SerializeField]
        private BuildingObjectData[] _buildingObjectDataArray;

        private Grid<BuildingGridObject> _buildingGrid;

        private Direction _currentDirection;
        private BuildingObjectData _buildingObjectData;

        private void Awake()
        {
            _buildingGrid = new Grid<BuildingGridObject>(3, 3, 2f, Tf.position,
                (grid, i, j) => new BuildingGridObject(grid, i, j), GridPlaneType.XZ);

            _currentDirection = Direction.Back;
            _buildingObjectData = _buildingObjectDataArray[0];
        }

        private void Start()
        {
            _buildingGrid.DrawGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 pointerPosition = hit.point;
                    _buildingGrid.GetGridLocations(pointerPosition, out int x, out int z);
                    //Debug.Log($"({i}, {j})");

                    List<Vector2Int> gridPositionList = _buildingObjectData.GetGridPositionList(new Vector2Int(x, z), _currentDirection);

                    bool canBuild = true;
                    for (int p = 0; p < gridPositionList.Count; p++)
                    {
                        BuildingGridObject buildingGridObject = _buildingGrid.GetValue(gridPositionList[p].x, gridPositionList[p].y);
                        if (buildingGridObject == null || !buildingGridObject.CanBuild())
                        {
                            canBuild = false;

                            break;
                        }
                    }

                    if (canBuild)
                    {
                        Vector2Int rotationOffset = _buildingObjectData.GetRotationOffset(_currentDirection);
                        Vector3 buildingObjectPosition = _buildingGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0.0f, rotationOffset.y);
                        Quaternion placedObjectRotation = Quaternion.Euler(0.0f, _buildingObjectData.GetRotationAngle(_currentDirection), 0.0f);
                        Transform objectTransform = Instantiate(_buildingObjectData.Prefab, buildingObjectPosition, placedObjectRotation);

                        for (int p = 0; p < gridPositionList.Count; p++)
                        {
                            _buildingGrid.GetValue(gridPositionList[p].x, gridPositionList[p].y).SetTransform(objectTransform);
                        }
                    }
                    else
                    {

                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentDirection = BuildingObjectData.GetNextDirection(_currentDirection);
            }
        }
    }

    public class BuildingGridObject
    {
        private Grid<BuildingGridObject> _grid;
        private int _x;
        private int _z;
        private Transform _transform;

        public BuildingGridObject(Grid<BuildingGridObject> grid, int x, int z)
        {
            _grid = grid;
            _x = x;
            _z = z;
        }

        public void SetTransform(Transform transform)
        {
            _transform = transform;
        }

        public bool CanBuild()
        {
            return _transform == null;
        }
    }
}

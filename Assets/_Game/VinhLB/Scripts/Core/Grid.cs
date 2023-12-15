using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VinhLB
{
    public class Grid<T>
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _originPosition;
        private GridPlaneType _planeType;
        private T[,] _gridArray;
        private TextMeshPro[,] _debugTextArray;
        private Transform _debugTextParentTransform;

        public float CellSize => _cellSize;

        public Grid(int width, int height, float cellSize,
            Vector3 originPosition = default, Func<Grid<T>, int, int, T> createFunc = null, GridPlaneType planeType = GridPlaneType.XY)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;
            _planeType = planeType;

            _gridArray = new T[_width, _height];
            if (createFunc != null)
            {
                for (int i = 0; i < _gridArray.GetLength(0); i++)
                {
                    for (int j = 0; j < _gridArray.GetLength(1); j++)
                    {
                        _gridArray[i, j] = createFunc(this, i, j);
                    }
                }
            }

            _debugTextArray = new TextMeshPro[_width, _height];
            _debugTextParentTransform = new GameObject("GridDebugTexts").GetComponent<Transform>();
        }

        ~Grid()
        {
            UnityEngine.Object.Destroy(_debugTextParentTransform.gameObject);
        }

        public T GetCellValue(int i, int j)
        {
            if (i >= 0 && j >= 0 && i < _width && j < _height)
            {
                return _gridArray[i, j];
            }
            else
            {
                return default;
            }
        }

        public T GetCellValue(Vector3 worldPosition)
        {
            GetGridPosition(worldPosition, out int i, out int j);

            return GetCellValue(i, j);
        }

        public void SetCellValue(int i, int j, T value)
        {
            if (i >= 0 && j >= 0 && i < _width && j < _height)
            {
                _gridArray[i, j] = value;
            }
        }

        public void SetCellValue(Vector3 worldPosition, T value)
        {
            GetGridPosition(worldPosition, out int i, out int j);

            SetCellValue(i, j, value);
        }

        public Vector3 GetGridWorldPosition(int i, int j)
        {
            Vector3 position;
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    position = new Vector3(i, j, 0);
                    break;
                case GridPlaneType.YZ:
                    position = new Vector3(0, i, j);
                    break;
                case GridPlaneType.XZ:
                    position = new Vector3(i, 0, j);
                    break;
                default:
                    position = Vector3.zero;
                    break;
            }

            return position * _cellSize + _originPosition;
        }

        public Vector3 GetGridWorldPosition(Vector3 worldPosition)
        {
            GetGridPosition(worldPosition, out int i, out int j);

            return GetGridWorldPosition(i, j);
        }

        public void GetGridPosition(Vector3 worldPosition, out int i, out int j)
        {
            Vector3 actualPosition = worldPosition - _originPosition;
            int intX = Mathf.FloorToInt(actualPosition.x / _cellSize);
            int intY = Mathf.FloorToInt(actualPosition.y / _cellSize);
            int intZ = Mathf.FloorToInt(actualPosition.z / _cellSize);
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    i = intX;
                    j = intY;
                    break;
                case GridPlaneType.YZ:
                    i = intY;
                    j = intZ;
                    break;
                case GridPlaneType.XZ:
                    i = intX;
                    j = intZ;
                    break;
                default:
                    i = 0;
                    j = 0;
                    break;
            }
        }

        public void DrawGrid()
        {
            for (int i = 0; i < _gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _gridArray.GetLength(1); j++)
                {
                    Vector3 textPosition = GetGridWorldPosition(i, j) + GetOffset(_cellSize * 0.5f);
                    _debugTextArray[i, j] = Utilities.CreateWorldText($"({i}, {j})",
                        null, textPosition, Vector2.one * _cellSize, 2f * _cellSize, Color.white, TextAlignmentOptions.Center);
                    _debugTextArray[i, j].transform.forward = GetGridPlaneNormal();

                    Debug.DrawLine(GetGridWorldPosition(i, j), GetGridWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetGridWorldPosition(i, j), GetGridWorldPosition(i + 1, j), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetGridWorldPosition(0, _height), GetGridWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetGridWorldPosition(_width, 0), GetGridWorldPosition(_width, _height), Color.white, 100f);
        }

        private Vector3 GetOffset(float value)
        {
            Vector3 offset;
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    offset = new Vector3(value, value, 0);
                    break;
                case GridPlaneType.YZ:
                    offset = new Vector3(0, value, value);
                    break;
                case GridPlaneType.XZ:
                    offset = new Vector3(value, 0, value);
                    break;
                default:
                    offset = Vector3.zero;
                    break;
            }

            return offset;
        }

        private Vector3 GetGridPlaneNormal()
        {
            Vector3 normal;
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    normal = Vector3.forward;
                    break;
                case GridPlaneType.YZ:
                    normal = Vector3.right;
                    break;
                case GridPlaneType.XZ:
                    normal = Vector3.down;
                    break;
                default:
                    normal = Vector3.zero;
                    break;
            }

            return normal;
        }
    }

    public enum GridPlaneType
    {
        XY = 0,
        YZ = 1,
        XZ = 2
    }
}

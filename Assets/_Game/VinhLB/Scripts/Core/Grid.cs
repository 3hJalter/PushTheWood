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
        }

        public T this[int i, int j]
        {
            get
            {
                return GetValue(i, j);
            }
            set
            {
                SetValue(i, j, value);
            }
        }

        public T GetValue(int i, int j)
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

        public void SetValue(int i, int j, T value)
        {
            if (i >= 0 && j >= 0 && i < _width && j < _height)
            {
                _gridArray[i, j] = value;
            }
        }

        public void SetValue(Vector3 worldPosition, T value)
        {
            GetGridLocations(worldPosition, out int i, out int j);
            SetValue(i, j, value);
        }

        public Vector3 GetWorldPosition(int i, int j)
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

        public void GetGridLocations(Vector3 worldPosition, out int i, out int j)
        {
            Vector3 actualPosition = worldPosition - _originPosition;
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    i = Mathf.FloorToInt(actualPosition.x / _cellSize);
                    j = Mathf.FloorToInt(actualPosition.y / _cellSize);
                    break;
                case GridPlaneType.YZ:
                    i = Mathf.FloorToInt(actualPosition.y / _cellSize);
                    j = Mathf.FloorToInt(actualPosition.z / _cellSize);
                    break;
                case GridPlaneType.XZ:
                    i = Mathf.FloorToInt(actualPosition.x / _cellSize);
                    j = Mathf.FloorToInt(actualPosition.z / _cellSize);
                    break;
                default:
                    i = 0;
                    j = 0;
                    break;
            }
        }

        public void DrawGrid()
        {
            TextMeshPro[,] debugTextArray = new TextMeshPro[_width, _height];
            for (int i = 0; i < _gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _gridArray.GetLength(1); j++)
                {
                    Vector3 textPosition = GetWorldPosition(i, j) + GetOffset(_cellSize * 0.5f);
                    debugTextArray[i, j] = Utilities.CreateWorldText($"({i}, {j})",
                        null, textPosition, Vector2.one * _cellSize, 2f * _cellSize, Color.white, TextAlignmentOptions.Center);
                    debugTextArray[i, j].transform.forward = -GetNormal();

                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);
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

        private Vector3 GetNormal()
        {
            Vector3 normal;
            switch (_planeType)
            {
                case GridPlaneType.XY:
                    normal = Vector3.Cross(Vector3.right, Vector3.up).normalized;
                    break;
                case GridPlaneType.YZ:
                    normal = Vector3.Cross(Vector3.up, Vector3.forward).normalized;
                    break;
                case GridPlaneType.XZ:
                    normal = Vector3.Cross(Vector3.forward, Vector3.right).normalized;
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

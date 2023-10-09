using CodeMonkey.Utils;
using TMPro;
using UnityEngine;

namespace _Game.Utilities.HGrid
{
    public class Grid
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly int [,] _gridArray;
        private readonly TextMesh[,] debugTextArray;
        
        public Grid(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridArray = new int[width, height];
            debugTextArray = new TextMesh[width, height];
            
            for (int i = 0; i < _gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _gridArray.GetLength(1); j++)
                {
                    debugTextArray[i,j] = UtilsClass.CreateWorldText(GetGridTextValue(i,j), null, 
                        GetWorldPosition(i, j) + new Vector3(_cellSize, _cellSize) * .5f, 
                        15, UnityEngine.Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(i, j), 
                        GetWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(i, j), 
                        GetWorldPosition(i + 1, j), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, _height), 
                GetWorldPosition(_width, _height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(_width, 0), 
                GetWorldPosition(_width, _height), Color.white, 100f);
            SetValue(0, 0, 56);
        }
        
        private Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * _cellSize;
        }

        private void SetValue(int x, int y, int value)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height) return;
            _gridArray[x, y] = value;
            debugTextArray[x, y].text = GetGridTextValue(x, y);
        }
        
        private string GetGridTextValue(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height) return "";
            return $" '{x},{y}' : {_gridArray[x, y]}";
        }
        
        private void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            y = Mathf.FloorToInt(worldPosition.y / _cellSize);
        }
    }
}

using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Utilities.Grid
{
    public class Grid<T, D> where T : GridCell<D>
    {
        private readonly TextMesh[,] debugTextArray;
        private readonly T[,] gridArray;
        private readonly Vector3 originPosition;

        public Grid(int width, int height, float cellSize, Vector3 originPosition = default,
            Func<GridCell<D>> ConstructorCell = null, Constants.Plane planeType = Constants.Plane.XY)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            this.originPosition = originPosition;
            PlaneType = planeType;

            gridArray = new T[width, height];
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = (T)ConstructorCell();
                gridArray[x, y].SetCellPosition(x, y);
                gridArray[x, y].Size = cellSize;
                gridArray[x, y].PlaneType = planeType;
                switch (planeType)
                {
                    case Constants.Plane.XY:
                        gridArray[x, y].UpdateWorldPosition(originPosition.x, originPosition.y);
                        break;
                    case Constants.Plane.XZ:
                        gridArray[x, y].UpdateWorldPosition(originPosition.x, originPosition.z);
                        break;
                    case Constants.Plane.YZ:
                        gridArray[x, y].UpdateWorldPosition(originPosition.y, originPosition.z);
                        break;
                }

                gridArray[x, y]._OnValueChange += OnGridCellValueChange;
            }


        }

        public float CellSize { get; }

        public int Width { get; }

        public int Height { get; }

        public Constants.Plane PlaneType { get; }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return GetUnitVector3(x, y) * CellSize + originPosition;
        }

        public (int, int) GetGridPosition(Vector3 worldPosition)
        {
            Vector3 realPos = worldPosition - originPosition;
            switch (PlaneType)
            {
                case Constants.Plane.XY:
                    return (Mathf.FloorToInt(realPos.x / CellSize), Mathf.FloorToInt(realPos.y / CellSize));
                case Constants.Plane.XZ:
                    return (Mathf.FloorToInt(realPos.x / CellSize), Mathf.FloorToInt(realPos.z / CellSize));
                case Constants.Plane.YZ:
                    return (Mathf.FloorToInt(realPos.y / CellSize), Mathf.FloorToInt(realPos.z / CellSize));
            }

            return default;
        }

        public void SetGridCell(int x, int y, T value)
        {
            if (value == null) return;
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                gridArray[x, y]._OnValueChange -= OnGridCellValueChange;
                gridArray[x, y] = value;
                gridArray[x, y]._OnValueChange += OnGridCellValueChange;
            }
        }

        public void SetGridCell(Vector3 position, T value)
        {
            int x, y;
            (x, y) = GetGridPosition(position);
            SetGridCell(x, y, value);
        }

        public T GetGridCell(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height) return gridArray[x, y];
            return default;
        }

        public T GetGridCell(Vector3 worldPosition)
        {
            int x, y;
            (x, y) = GetGridPosition(worldPosition);
            if (x >= 0 && y >= 0 && x < Width && y < Height) return gridArray[x, y];
            return default;
        }

        private void OnGridCellValueChange(int x, int y)
        {
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }

        private Vector3 GetUnitVector3(float val1, float val2)
        {
            switch (PlaneType)
            {
                case Constants.Plane.XY:
                    return new Vector3(val1, val2, 0);
                case Constants.Plane.XZ:
                    return new Vector3(val1, 0, val2);
                case Constants.Plane.YZ:
                    return new Vector3(0, val1, val2);
            }

            return default;
        }


        #region VISIT CLASS

        public abstract class PathfindingAlgorithm
        {
            protected Grid<T, D> grid;
            public abstract List<T> FindPath(int startX, int startY, int endX, int endY, Grid<T, D> grid);
        }

        public class DebugGrid
        {
            private readonly Vector2 UNWALKABLE_UV = Vector2.zero;
            private readonly Vector2 WALKABLE_UV = new(9f / 334, 0);

            public virtual void DrawGrid(Grid<T, D> grid, bool isPositionShow = false)
            {
                for (int x = 0; x < grid.gridArray.GetLength(0); x++)
                for (int y = 0; y < grid.gridArray.GetLength(1); y++)
                {
                    if (isPositionShow)
                        grid.debugTextArray[x, y] = GridUtilities.CreateWorldText(grid.gridArray[x, y].ToString(), null
                            , grid.GetWorldPosition(x, y) + new Vector3(grid.CellSize / 2, grid.CellSize / 2), 20,
                            Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1), Color.white, 100f,
                        true);
                    Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y), Color.white, 100f,
                        true);
                }

                Debug.DrawLine(grid.GetWorldPosition(0, grid.Height), grid.GetWorldPosition(grid.Width, grid.Height),
                    Color.white, 100f);
                Debug.DrawLine(grid.GetWorldPosition(grid.Width, 0), grid.GetWorldPosition(grid.Width, grid.Height),
                    Color.white, 100f);
            }

            public virtual void UpdateVisualMap(Grid<NodeCell, int> grid, Mesh mesh)
            {
                GridUtilities.CreateEmptyMeshArray(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uv,
                    out int[] triangles);
                for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                {
                    int index = x * grid.Height + y;
                    Vector3 quadSize = new Vector3(1, 1) * grid.CellSize;
                    NodeCell cell = grid.GetGridCell(x, y);
                    if (cell.IsWalkable)
                        GridUtilities.AddToMeshArray(vertices, uv, triangles, index, cell.WorldPos, 0f, quadSize,
                            WALKABLE_UV, WALKABLE_UV);
                    else
                        GridUtilities.AddToMeshArray(vertices, uv, triangles, index, cell.WorldPos, 0f, quadSize,
                            UNWALKABLE_UV, UNWALKABLE_UV);
                }

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;
            }

            public virtual void UpdateVisualMap(Grid<GameCell, GameCellData> grid, Mesh mesh)
            {
                GridUtilities.CreateEmptyMeshArray(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uv,
                    out int[] triangles);
                for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                {
                    int index = x * grid.Height + y;

                    Vector3 quadSize = default;
                    switch (grid.PlaneType)
                    {
                        case Constants.Plane.XY:
                            quadSize = new Vector3(1, 1) * grid.CellSize;
                            break;
                        case Constants.Plane.XZ:
                            quadSize = new Vector3(1, 0, 1) * grid.CellSize;
                            break;
                        case Constants.Plane.YZ:
                            quadSize = new Vector3(0, 1, 1) * grid.CellSize;
                            break;
                    }

                    GameCell cell = grid.GetGridCell(x, y);
                    if (!cell.IsBlockingRollingTree)
                        GridUtilities.AddToMeshArray(vertices, uv, triangles, index, cell.WorldPos, 0f, quadSize,
                            WALKABLE_UV, WALKABLE_UV);
                    else
                        GridUtilities.AddToMeshArray(vertices, uv, triangles, index, cell.WorldPos, 0f, quadSize,
                            UNWALKABLE_UV, UNWALKABLE_UV);
                }

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;
            }

            public virtual void DrawPath(List<T> path)
            {
                if (path != null)
                    for (int i = 0; i < path.Count - 1; i++)
                        Debug.DrawLine(new Vector3(path[i].WorldX, path[i].WorldY),
                            new Vector3(path[i + 1].WorldX, path[i + 1].WorldY), Color.cyan, 5f);
            }
        }

        #endregion
    }
}

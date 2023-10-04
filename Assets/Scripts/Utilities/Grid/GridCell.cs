using System;
using UnityEngine;

namespace Utilities.Grid
{
    public class GridCell<T>
    {
        protected const int MIN = 0;
        protected const int MAX = 100;
        public Action<int, int> _OnValueChange;
        public Constants.Plane PlaneType;
        protected float size;
        protected T value;
        protected Vector3 worldPos;
        protected float worldX;
        protected float worldY;

        protected int x;
        protected int y;

        public GridCell()
        {
        }

        public GridCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public GridCell(GridCell<T> copy)
        {
            x = copy.x;
            y = copy.y;
            size = copy.size;
            worldX = copy.worldX;
            worldY = copy.worldY;
            worldPos = copy.worldPos;
            PlaneType = copy.PlaneType;
        }

        public int X => x;
        public int Y => y;
        public float WorldX => worldX;
        public float WorldY => worldY;
        public Vector3 WorldPos => worldPos;
        public T Value => value;

        public float Size
        {
            get => size;
            set => size = value;
        }

        public void SetCellValue(T value)
        {
            this.value = value;
            _OnValueChange?.Invoke(x, y);
        }


        public void SetCellPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void UpdateWorldPosition(float originX, float originY)
        {
            worldX = originX + (x + 0.5f) * size;
            worldY = originY + (y + 0.5f) * size;

            switch (PlaneType)
            {
                case Constants.Plane.XY:
                    worldPos.Set(worldX, worldY, 0);
                    break;
                case Constants.Plane.XZ:
                    worldPos.Set(worldX, 0, worldY);
                    break;
                case Constants.Plane.YZ:
                    worldPos.Set(0, worldX, worldY);
                    break;
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}

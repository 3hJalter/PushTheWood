using System;
using UnityEngine;
using Plane = MapEnum.Plane;

namespace DesignPattern.Grid
{
    public class GridCell<T>
    {
        protected const int MIN = 0;
        protected const int MAX = 100;
        public Action<int, int> OnValueChange;
        public Plane PlaneType;
        protected T value;
        private Vector3 worldPos;
        private float worldX;
        private float worldY;

        protected int x;
        protected int y;

        protected GridCell()
        {
        }

        public GridCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        protected GridCell(GridCell<T> copy)
        {
            x = copy.x;
            y = copy.y;
            Size = copy.Size;
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

        public float Size { get; set; }

        public void SetCellValue(T valueIn)
        {
            value = valueIn;
            OnValueChange?.Invoke(x, y);
        }


        public void SetCellPosition(int xIn, int yIn)
        {
            x = xIn;
            y = yIn;
        }

        public void UpdateWorldPosition(float originX, float originY)
        {
            worldX = originX + (x + 0.5f) * Size;
            worldY = originY + (y + 0.5f) * Size;

            switch (PlaneType)
            {
                case Plane.XY:
                    worldPos.Set(worldX, worldY, 0);
                    break;
                case Plane.XZ:
                    worldPos.Set(worldX, 0, worldY);
                    break;
                case Plane.YZ:
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

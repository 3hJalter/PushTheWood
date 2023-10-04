using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.AI
{
    public class GridCell<T>
    {
        public Action<int, int> _OnValueChange;
        protected const int MIN = 0;
        protected const int MAX = 100;

        protected int x;
        protected int y;
        protected float size;
        protected float worldX;
        protected float worldY;
        protected Vector3 worldPos = new Vector3();
        protected T value;
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
        public CONSTANTS.PLANE PlaneType;

        public GridCell() { }
        public GridCell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public GridCell(GridCell<T> copy)
        {
            this.x = copy.x;
            this.y = copy.y;
            this.size = copy.size;
            this.worldX = copy.worldX;
            this.worldY = copy.worldY;
            this.worldPos = copy.worldPos;
            this.PlaneType = copy.PlaneType;
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
                case CONSTANTS.PLANE.XY:
                    worldPos.Set(worldX, worldY, 0);
                    break;
                case CONSTANTS.PLANE.XZ:
                    worldPos.Set(worldX, 0, worldY);
                    break;
                case CONSTANTS.PLANE.YZ:
                    worldPos.Set( 0, worldX, worldY);
                    break;
            }
        }
        public override string ToString()
        {
            return value.ToString();
        }

    }
}
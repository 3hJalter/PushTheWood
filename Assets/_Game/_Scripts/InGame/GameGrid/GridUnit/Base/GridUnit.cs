using System;
using System.Collections;
using System.Collections.Generic;
using _Game._Scripts.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.Base
{
    public abstract class GridUnit : GameUnit
    {
        // Skin
        [SerializeField] protected Transform skin;

        // Unit Height
        [SerializeField] protected HeightLevel startHeight;

        [SerializeField] protected HeightLevel endHeight;

        // Unit Size
        [SerializeField] protected Vector3Int size;

        // Unit Cell
        protected readonly List<GameGridCell> cellInUnits = new();
        protected GameGridCell mainCell;
        public HeightLevel StartHeight => startHeight;
        public HeightLevel EndHeight => endHeight;

        public Vector3 GetMainCellWorldPos()
        {
            return mainCell.WorldPos;
        }

        public Vector3Int GetSize()
        {
            return size;
        }

        public void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One)
        {
            startHeight = startHeightIn;
            endHeight = startHeightIn + size.y - 1;
            mainCell = mainCellIn;
            cellInUnits.Add(mainCell);
            // Add all neighbour X from mainCell
            AddXCell(mainCell, size.x);
            // Add all neighbour Z from all cells in cellInUnits
            for (int i = cellInUnits.Count - 1; i >= 0; i--) AddZCell(cellInUnits[i], size.z);
            // Set gridUnitDic of cellInUnits with heightLevel in heightLevel list
            for (int i = 0; i < cellInUnits.Count; i++) cellInUnits[i].AddGridUnit(startHeight, endHeight, this);
            // Add offset Height to Position
            Vector3 offsetY = new(0, (int)startHeight * Constants.CELL_SIZE, 0);
            Tf.position = mainCell.WorldPos + offsetY;
        }

        public virtual void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
        }

        private void AddZCell(GameGridCell cell, int sizeZ)
        {
            for (int i = 1; i < sizeZ; i++)
            {
                GameGridCell neighbourZ = GameGridManager.Ins.GetNeighbourCell(cell, Direction.Forward, i);
                if (neighbourZ == null) continue;
                cellInUnits.Add(neighbourZ);
                Debug.Log("Add neighbourZ: " + neighbourZ.WorldPos + " to cellInUnits: " + name);
            }
        }

        private void AddXCell(GameGridCell cell, int sizeX)
        {
            for (int i = 1; i < sizeX; i++)
            {
                GameGridCell neighbourX = GameGridManager.Ins.GetNeighbourCell(cell, Direction.Right, i);
                if (neighbourX == null) continue;
                cellInUnits.Add(neighbourX);
                Debug.Log("Add neighbourX: " + neighbourX.WorldPos + " to cellInUnits: " + name);
            }
        }
    }
}

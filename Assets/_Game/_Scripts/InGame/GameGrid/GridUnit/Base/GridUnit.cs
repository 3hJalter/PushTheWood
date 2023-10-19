using System;
using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public abstract class GridUnit : GameUnit
    {
        [SerializeField] protected Transform skin;
        [SerializeField] protected Vector3Int size;
        [SerializeField] protected HeightLevel startHeight = HeightLevel.One;
        [SerializeField] protected float yOffsetOnDown = 0.5f;
        [SerializeField] protected UnitState unitState = UnitState.Up;
        protected readonly List<GameGridCell> cellInUnits = new();
        private UnitInitData _unitInitData;

        [SerializeField] protected bool isMinusHalfSizeY;
        [SerializeField] protected HeightLevel endHeight;
        protected int islandID = -1;
        protected GameGridCell mainCell;
        protected UnitState nextUnitState;
            

        public GameGridCell MainCell => mainCell;
        public HeightLevel StartHeight => startHeight;
        public HeightLevel EndHeight => endHeight;

        private void Awake()
        {
            SaveInitData(size, unitState, skin);
        }

        public virtual void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One)
        {
            GetInitData();
            startHeight = startHeightIn;
            endHeight = startHeightIn + (size.y - 1) * 2;
            if (!isMinusHalfSizeY && unitState == UnitState.Up) endHeight += 1;
            mainCell = mainCellIn;
            cellInUnits.Add(mainCell);
            // Add all neighbour X Z from mainCell
            AddXCell(mainCell, size.x);
            for (int i = cellInUnits.Count - 1; i >= 0; i--) AddZCell(cellInUnits[i], size.z);
            // Set gridUnitDic of cellInUnits with heightLevel in heightLevel list
            for (int i = 0; i < cellInUnits.Count; i++) cellInUnits[i].AddGridUnit(startHeight, endHeight, this);
            // Add offset Height to Position
            // Vector3 offsetY = new(0, (int) startHeight * Constants.CELL_SIZE, 0);
            Tf.position = mainCell.WorldPos + Vector3.up * (float)startHeight / 2 * Constants.CELL_SIZE;
            if (unitState is UnitState.Down) Tf.position -= Vector3.up * yOffsetOnDown;
        }

        protected Vector3 UnitYPos(HeightLevel startHeightI)
        {
            float offsetY1 = (float)startHeight / 2 * Constants.CELL_SIZE;
            float offsetY2 = float.MaxValue;
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GridUnit unit = cellInUnits[i].GetGridUnitAtHeight(startHeightI - 1);
                if (unit != null && unit.unitState is UnitState.Down && unit.yOffsetOnDown < offsetY2)
                    offsetY2 = unit.yOffsetOnDown;
            }

            if (Math.Abs(offsetY2 - float.MaxValue) < 0.1) offsetY2 = 0f;
            return new Vector3(0, offsetY1 - offsetY2, 0);
        }

        public Vector3 GetMainCellWorldPos()
        {
            return mainCell.WorldPos;
        }

        public Vector3Int GetSize()
        {
            return size;
        }

        protected virtual void OnDespawn()
        {
            SimplePool.Despawn(this);
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

        private void SaveInitData(Vector3Int sizeI, UnitState unitStateI, Transform skinI)
        {
            _unitInitData = new UnitInitData(size, unitState, skin.localPosition, skin.localRotation);
        }

        private void GetInitData()
        {
            size = _unitInitData.Size;
            unitState = _unitInitData.UnitState;
            skin.localPosition = _unitInitData.LocalSkinPos;
            skin.localRotation = _unitInitData.LocalSkinRot;
        }
    }

    public enum UnitState
    {
        Up = 0,
        Down = 1
    }

    public class UnitInitData
    {
        public UnitInitData(Vector3Int size, UnitState unitState, Vector3 localSkinPos, Quaternion localSkinRot)
        {
            Size = size;
            UnitState = unitState;
            LocalSkinPos = localSkinPos;
            LocalSkinRot = localSkinRot;
        }

        public Vector3Int Size { get; }
        public UnitState UnitState { get; }
        public Vector3 LocalSkinPos { get; }
        public Quaternion LocalSkinRot { get; }
    }
}

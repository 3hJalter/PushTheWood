using System.Collections.Generic;
using _Game.DesignPattern;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using VinhLB;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnit : GameUnit
    {
        [SerializeField]
        protected Transform skin;
        [SerializeField]
        protected Vector3Int size;

        public Vector3Int Size => size;

        [SerializeField]
        protected bool isMinusHalfSizeY;
        [SerializeField]
        protected HeightLevel startHeight = HeightLevel.One;
        [SerializeField]
        protected HeightLevel endHeight;
        [SerializeField]
        protected float yOffsetOnDown = 0.5f;
        [SerializeField]
        protected UnitState unitState = UnitState.Up;
        [SerializeField]
        protected UnitType unitType = UnitType.Both;
        public int islandID = -1;

        [SerializeField]
        protected Direction skinRotationDirection = Direction.None;
        [SerializeField]
        protected Direction lastPushedDirection = Direction.None;
        public readonly List<GameGridCell> cellInUnits = new();
        private UnitInitData _unitInitData;

        protected GameGridCell mainCell;
        protected UnitState nextUnitState;

        public UnitType UnitType
        {
            get => unitType;
            protected set => unitType = value;
        }

        public GameGridCell MainCell => mainCell;
        public HeightLevel StartHeight => startHeight;
        public HeightLevel EndHeight => endHeight;
        public Direction SkinRotationDirection => skinRotationDirection;

        protected HeightLevel BelowStartHeight => startHeight - Constants.BELOW_HEIGHT;
        protected HeightLevel UpperEndHeight => endHeight + Constants.UPPER_HEIGHT;

        private void Awake()
        {
            SaveInitData(size, unitState, skin);
        }

        private void SaveInitData(Vector3Int sizeI, UnitState unitStateI, Transform skinI)
        {
            _unitInitData = new UnitInitData(sizeI, unitStateI, skinI.localPosition, skinI.localRotation);
        }

        private void GetInitData()
        {
            size = _unitInitData.Size;
            unitState = _unitInitData.UnitState;
            skin.localPosition = _unitInitData.LocalSkinPos;
            skin.localRotation = _unitInitData.LocalSkinRot;
        }

        public virtual void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None)
        {
            if (isUseInitData) GetInitData();
            nextUnitState = unitState;
            islandID = mainCellIn.IslandID;
            SetHeight(startHeightIn);
            AddCell(mainCellIn, skinDirection);
            // Add offset Height to Position
            Tf.position = mainCell.WorldPos + Vector3.up * (float)startHeight / 2 * Constants.CELL_SIZE;
            if (unitState is UnitState.Down) Tf.position -= Vector3.up * yOffsetOnDown;

            skinRotationDirection = skinDirection;
            skin.rotation = Quaternion.Euler(0f, BuildingUnitData.GetRotationAngle(skinRotationDirection), 0f);

            Vector2Int rotationOffset = BuildingUnitData.GetRotationOffset(size.x, size.z, skinRotationDirection);
            Vector3 posOffset = new Vector3(rotationOffset.x, 0, rotationOffset.y) * Constants.CELL_SIZE;
            skin.position += posOffset;
        }

        protected void SetHeight(HeightLevel startHeightIn)
        {
            startHeight = startHeightIn;
            endHeight = CalculateEndHeight(startHeightIn, size);
        }

        public HeightLevel CalculateEndHeight(HeightLevel startHeightIn, Vector3Int sizeIn)
        {
            HeightLevel endHeightOut = startHeightIn + (sizeIn.y - 1) * 2;
            if (!isMinusHalfSizeY && nextUnitState == UnitState.Up) endHeightOut += 1;
            return endHeightOut;
        }

        private void AddCell(GameGridCell mainCellIn, Direction skinDirection)
        {
            mainCell = mainCellIn;
            cellInUnits.Add(mainCell);
            switch (skinDirection)
            {
                default:
                case Direction.Back:
                case Direction.Forward:
                    AddXCell(mainCell, size.x);
                    for (int i = cellInUnits.Count - 1; i >= 0; i--) AddZCell(cellInUnits[i], size.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    AddXCell(mainCell, size.z);
                    for (int i = cellInUnits.Count - 1; i >= 0; i--) AddZCell(cellInUnits[i], size.x);
                    break;
            }
            // Add this unit to cells
            for (int i = 0; i < cellInUnits.Count; i++) cellInUnits[i].AddGridUnit(startHeight, endHeight, this);
        }

        public Vector3 GetMainCellWorldPos()
        {
            return mainCell.WorldPos;
        }

        public Vector3Int GetSize()
        {
            return size;
        }


        public virtual void OnDespawn()
        {
            Tf.DOKill(true);
            for (int i = 0; i < cellInUnits.Count; i++) cellInUnits[i].RemoveGridUnit(this);
            cellInUnits.Clear();
            mainCell = null;
            SimplePool.Despawn(this);
        }

        public virtual void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            lastPushedDirection = direction;
        }

        public GridUnit GetBelowUnit()
        {
            if (mainCell is not null) return mainCell.GetGridUnitAtHeight(BelowStartHeight);
            // Get cell from this position
            Vector3 position = Tf.position;
            GameGridCell cell =
                LevelManager.Ins.GetCell(new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z)));
            return cell?.GetGridUnitAtHeight(BelowStartHeight);
        }

        protected GridUnit GetAboveUnit()
        {
            return mainCell.GetGridUnitAtHeight(UpperEndHeight);
        }

        private void AddZCell(GameGridCell cell, int sizeZ)
        {
            for (int i = 1; i < sizeZ; i++)
            {
                //GameGridCell neighbourZ = GridUnitFunc.GetNeighbourCell(grid, cell, Direction.Forward, i);
                GameGridCell neighbourZ = LevelManager.Ins.GetNeighbourCell(cell, Direction.Forward, i);
                if (neighbourZ == null) continue;
                cellInUnits.Add(neighbourZ);
            }
        }

        private void AddXCell(GameGridCell cell, int sizeX)
        {
            for (int i = 1; i < sizeX; i++)
            {
                //GameGridCell neighbourX = GridUnitFunc.GetNeighbourCell(grid, cell, Direction.Right, i);
                GameGridCell neighbourX = LevelManager.Ins.GetNeighbourCell(cell, Direction.Right, i);
                if (neighbourX == null) continue;
                cellInUnits.Add(neighbourX);
            }
        }
    }

    public enum UnitState
    {
        Up = 0,
        Down = 1
    }

    public enum UnitType
    {
        None = -1,
        Horizontal = 0,
        Vertical = 1,
        Both = 2
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
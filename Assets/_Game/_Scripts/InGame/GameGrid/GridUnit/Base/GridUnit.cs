using System.Collections.Generic;
using _Game.DesignPattern;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnit : GameUnit
    {
        [SerializeField] protected Transform skin; // Model location
        [SerializeField] protected Vector3Int size; // Size of this unit
        [SerializeField] protected bool isMinusHalfSizeY; // Use when the init size need to be x.5 (ex: 1.5, 2.5, 3.5)
        [SerializeField] protected HeightLevel startHeight = HeightLevel.One; // Serialize for test
        [SerializeField] protected HeightLevel endHeight; // Serialize for test
        [SerializeField] protected float yOffsetOnDown = 0.5f; // Offset when unit is down

        public int islandID = -1; // The island that this unit is on
        public readonly List<GameGridCell> cellInUnits = new(); // All cells that this unit is on

        private UnitInitData _unitInitData; // Save init data for despawn and spawn

        protected Direction lastPushedDirection = Direction.None; // The last direction that this unit is pushed
        protected GameGridCell mainCell; // The main cell that this unit is on
        protected UnitState nextUnitState; // The next state of this unit
        [SerializeField] protected UnitState unitState = UnitState.Up; // The current state of this unit (Up or Down), Serialize for test
        [SerializeField] protected UnitType unitType = UnitType.Both; // The type of this unit (Horizontal, Vertical, Both or None). Serialize for test


        public Vector3Int Size => size;

        public UnitType UnitType
        {
            get => unitType;
            protected set => unitType = value;
        }

        public GameGridCell MainCell => mainCell;
        public HeightLevel StartHeight => startHeight;
        public HeightLevel EndHeight => endHeight;

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
            bool isUseInitData = true)
        {
            if (isUseInitData) GetInitData();
            nextUnitState = unitState;
            islandID = mainCellIn.IslandID;
            SetHeight(startHeightIn);
            AddCell(mainCellIn);
            // Add offset Height to Position
            Tf.position = mainCell.WorldPos + Vector3.up * (float)startHeight / 2 * Constants.CELL_SIZE;
            if (unitState is UnitState.Down) Tf.position -= Vector3.up * yOffsetOnDown;

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

        private void AddCell(GameGridCell mainCellIn)
        {
            mainCell = mainCellIn;
            cellInUnits.Add(mainCell);
            AddXCell(mainCell, size.x);
            for (int i = cellInUnits.Count - 1; i >= 0; i--) AddZCell(cellInUnits[i], size.z);
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
                GameGridCell neighbourZ = LevelManager.Ins.GetNeighbourCell(cell, Direction.Forward, i);
                if (neighbourZ == null) continue;
                cellInUnits.Add(neighbourZ);
            }
        }

        private void AddXCell(GameGridCell cell, int sizeX)
        {
            for (int i = 1; i < sizeX; i++)
            {
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

using System;
using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using _Game.DesignPattern.ConditionRule;
using _Game.DesignPattern.StateMachine;
using _Game.Utilities.Grid;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;
using UnityEngine.Serialization;
using VinhLB;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnit : GameUnit, IOriginator
    {
        // Model of Unit
        [SerializeField] public Transform skin; // Model location

        // Size of this unit, the X and Z equal to the size of the main cell, the Y equal to height level
        [SerializeField] protected Vector3Int size;

        // Use when the init size Y need to be x.5 when Up (ex: 1.5, 2.5, 3.5)
        [SerializeField] protected bool isMinusHalfSizeY;

        // Height level of this unit
        [SerializeField] protected HeightLevel startHeight = HeightLevel.One; // Serialize for test

        [SerializeField] protected HeightLevel endHeight; // Serialize for test

        // Offset when unit is down
        [SerializeField] public float yOffsetOnDown = 0.5f;

        // The island that this unit is on
        public int islandID = -1;

        // The current state of this unit (Up or Down)
        [FormerlySerializedAs("unitState")] [SerializeField]
        protected UnitTypeY unitTypeY = UnitTypeY.Up; // Serialize for test

        // The type of this unit (Horizontal, Vertical, Both or None)
        [FormerlySerializedAs("unitType")] [SerializeField]
        protected UnitTypeXZ unitTypeXZ = UnitTypeXZ.Both; // Serialize for test

        [SerializeField] protected Direction skinRotationDirection = Direction.None;

        [SerializeField] public readonly HashSet<GridUnit> belowUnits = new();

        public readonly List<GameGridCell> cellInUnits = new();

        // Position data when this unit enter a cell
        public readonly EnterCellPosData EnterPosData = new();

        // All neighbor units of this unit
        [SerializeField] protected readonly HashSet<GridUnit> neighborUnits = new();
        [SerializeField] protected readonly HashSet<GridUnit> upperUnits = new();

        // Save init data on first Initialize
        private UnitInitData _unitInitData;

        // The last direction that this unit is pushed
        protected Direction lastPushedDirection = Direction.None;

        // The main cell that this unit is on

        protected GameGridCell mainCell;

        public Vector3Int Size
        {
            get => size;
            set => size = value;
        }

        public UnitTypeXZ UnitTypeXZ
        {
            get => unitTypeXZ;
            set => unitTypeXZ = value;
        }

        public UnitTypeY UnitTypeY
        {
            get => unitTypeY;
            set => unitTypeY = value;
        }

        public GameGridCell MainCell => mainCell;

        public HeightLevel StartHeight
        {
            get => startHeight;
            set => startHeight = value;
        }

        public Direction SkinRotationDirection => skinRotationDirection;

        public HeightLevel EndHeight
        {
            get => endHeight;
            set => endHeight = value;
        }

        public HeightLevel BelowStartHeight => startHeight - Constants.BELOW_HEIGHT;
        public HeightLevel UpperEndHeight => endHeight + Constants.UPPER_HEIGHT;

        protected virtual void Awake()
        {
            SaveInitData(size, unitTypeY, skin);
        }

        public virtual void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true, Direction skinDirection = Direction.None, bool hasSetPosAndRos = false)
        {
            if (isUseInitData) GetInitData();
            islandID = mainCellIn.IslandID;
            SetHeight(startHeightIn);
            SetEnterCellData(Direction.None, mainCellIn, unitTypeY);
            OnEnterCells(mainCellIn, InitCell(mainCellIn, skinDirection));
            // Set position
            if (!hasSetPosAndRos) OnSetPositionAndRotation(EnterPosData.finalPos, skinDirection);
        }

        public void OnSetPositionAndRotation(Vector3 position, Direction skinDirection)
        {
            Tf.position = position;

            skinRotationDirection = skinDirection;
            skin.rotation = Quaternion.Euler(0f, BuildingUnitData.GetRotationAngle(skinRotationDirection), 0f);

            Vector2Int rotationOffset = BuildingUnitData.GetRotationOffset(size.x, size.z, skinRotationDirection);
            Vector3 posOffset = new Vector3(rotationOffset.x, 0, rotationOffset.y) * Constants.CELL_SIZE;
            skin.position += posOffset;
        }
        
        public virtual bool IsCurrentStateIs(StateEnum stateEnum)
        {
            return false;
        }

        public HeightLevel CalculateEndHeight(HeightLevel startHeightIn, Vector3Int sizeIn)
        {
            HeightLevel endHeightOut = startHeightIn + (sizeIn.y - 1) * 2;
            if (!isMinusHalfSizeY && unitTypeY == UnitTypeY.Up) endHeightOut += 1;
            return endHeightOut;
        }

        public virtual void OnDespawn()
        {
            Tf.DOKill(true);
            OnOutCells();
            this.Despawn();
        }

        public virtual void OnPush(Direction direction, ConditionData conditionData = null)
        {

        }
        
        public virtual void OnBePushed(Direction direction = Direction.None, GridUnit pushUnit = null)
        {
            lastPushedDirection = direction;
        }

        public void OnOutCells()
        {
            RemoveUnitFromCell();
            OnOutTrigger();
            ClearNeighbor();
            cellInUnits.Clear();
            mainCell = null;
        }

        public Vector3 GetUnitWorldPos(GameGridCell cell = null)
        {
            cell ??= mainCell;
            float offsetY = (float)startHeight / 2 * Constants.CELL_SIZE;
            if (unitTypeY == UnitTypeY.Down) offsetY -= yOffsetOnDown;
            return cell.WorldPos + Vector3.up * offsetY;
        }

        public void SetEnterCellData(Direction direction, GameGridCell enterMainCell, UnitTypeY nextUnitTypeY,
            bool hasInitialOffset = true, List<GameGridCell> enterNextCells = null)
        {
            Vector3 initialPos = GetUnitWorldPos(enterMainCell);
            HeightLevel enterStartHeight = enterNextCells is null
                ? GetEnterStartHeight(enterMainCell)
                : GetEnterStartHeight(enterNextCells);
            Vector3 finalPos = PredictUnitPos();
            EnterPosData.SetEnterPosData(direction, enterStartHeight, initialPos, finalPos, yOffsetOnDown,
                hasInitialOffset);
            return;

            Vector3 PredictUnitPos()
            {
                float offsetY = (float)enterStartHeight / 2 * Constants.CELL_SIZE;
                if (nextUnitTypeY == UnitTypeY.Down) offsetY -= yOffsetOnDown;
                return enterMainCell.WorldPos + Vector3.up * offsetY;
            }
        }

        public void OnEnterCells(GameGridCell enterMainCell, List<GameGridCell> enterNextCells = null)
        {
            SetHeight(EnterPosData.startHeight);
            InitCellsToUnit(enterMainCell, enterNextCells);
            SetNeighbor(LevelManager.Ins.CurrentLevel.GridMap);
            return;

            void InitCellsToUnit(GameGridCell enterMainCellIn, List<GameGridCell> enterCells = null)
            {
                mainCell = enterMainCellIn;
                // Add all nextCells to cellInUnits
                if (enterCells is not null)
                    for (int i = 0; i < enterCells.Count; i++)
                        AddCell(enterCells[i]);
                else AddCell(enterMainCellIn);
            }
        }

        private void ClearNeighbor()
        {
            foreach (GridUnit unit in neighborUnits) unit.neighborUnits.Remove(this);
            neighborUnits.Clear();
            foreach (GridUnit unit in upperUnits) unit.belowUnits.Remove(this);
            upperUnits.Clear();
            foreach (GridUnit unit in belowUnits) unit.upperUnits.Remove(this);
            belowUnits.Clear();
        }

        private void OnOutTrigger()
        {
            foreach (GridUnit unit in neighborUnits) unit.OnOutTriggerNeighbor(this);
            foreach (GridUnit unit in belowUnits) unit.OnOutTriggerUpper(this);
            foreach (GridUnit unit in upperUnits) unit.OnOutTriggerBelow(this);
        }

        protected virtual void OnOutTriggerNeighbor(GridUnit triggerUnit)
        {
            neighborUnits.Remove(triggerUnit);
        }

        protected virtual void OnOutTriggerBelow(GridUnit triggerUnit)
        {
            belowUnits.Remove(triggerUnit);
            // TODO: Falling Logic 
        }

        protected virtual void OnOutTriggerUpper(GridUnit triggerUnit)
        {
            upperUnits.Remove(triggerUnit);
        }

        private void SetNeighbor(Grid<GameGridCell, GameGridCellData> map)
        {
            // In direction
            neighborUnits.UnionWith(this.GetAllNeighborUnits(Direction.Left, map));
            neighborUnits.UnionWith(this.GetAllNeighborUnits(Direction.Right, map));
            neighborUnits.UnionWith(this.GetAllNeighborUnits(Direction.Forward, map));
            neighborUnits.UnionWith(this.GetAllNeighborUnits(Direction.Back, map));
            foreach (GridUnit unit in neighborUnits) unit.neighborUnits.Add(this);
            // Upper and Below
            upperUnits.UnionWith(this.GetAboveUnits());
            foreach (GridUnit unit in upperUnits) unit.belowUnits.Add(this);
            belowUnits.UnionWith(this.GetBelowUnits());
            foreach (GridUnit unit in belowUnits) unit.upperUnits.Add(this);
        }

        public void OnEnterTrigger(GridUnit gridUnit)
        {
            foreach (GridUnit unit in neighborUnits.ToList()) unit.OnEnterTriggerNeighbor(gridUnit);
            foreach (GridUnit unit in belowUnits.ToList()) unit.OnEnterTriggerUpper(gridUnit);
            foreach (GridUnit unit in upperUnits.ToList()) unit.OnEnterTriggerBelow(gridUnit);
        }

        protected virtual void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {

        }

        protected virtual void OnEnterTriggerBelow(GridUnit triggerUnit)
        {

        }

        protected virtual void OnEnterTriggerUpper(GridUnit triggerUnit)
        {

        }

        private void SetHeight(HeightLevel startHeightIn)
        {
            startHeight = startHeightIn;
            endHeight = CalculateEndHeight(startHeightIn, size);
        }

        private void SaveInitData(Vector3Int sizeI, UnitTypeY unitTypeYi, Transform skinI)
        {
            _unitInitData = new UnitInitData(sizeI, unitTypeYi, skinI.localPosition, skinI.localRotation);
        }

        private void GetInitData()
        {
            size = _unitInitData.Size;
            unitTypeY = _unitInitData.UnitTypeY;
            skin.localPosition = _unitInitData.LocalSkinPos;
            skin.localRotation = _unitInitData.LocalSkinRot;
        }

        private List<GameGridCell> InitCell(GameGridCell mainCellIn, Direction skinDirection)
        {
            List<GameGridCell> initCells = new();
            mainCell = mainCellIn;
            initCells.Add(mainCell);
            switch (skinDirection)
            {
                default:
                case Direction.Back:
                case Direction.Forward:
                    InitXCell(mainCell, size.x);
                    for (int i = initCells.Count - 1; i >= 0; i--) InitZCell(initCells[i], size.z);
                    break;
                case Direction.Left:
                case Direction.Right:
                    InitZCell(mainCell, size.z);
                    for (int i = initCells.Count - 1; i >= 0; i--) InitXCell(initCells[i], size.x);
                    break;
            }

            // Add this unit to cells
            // for (int i = 0; i < initCells.Count; i++) initCells[i].AddGridUnit(startHeight, endHeight, this);
            return initCells;

            void InitXCell(GameGridCell cell, int sizeX)
            {
                for (int i = 1; i < sizeX; i++)
                {
                    GameGridCell neighbourX = cell.GetNeighborCell(Direction.Right, i);
                    if (neighbourX == null || initCells.Contains(neighbourX)) continue;
                    initCells.Add(neighbourX);
                }
            }

            void InitZCell(GameGridCell cell, int sizeZ)
            {
                for (int i = 1; i < sizeZ; i++)
                {
                    GameGridCell neighbourZ = cell.GetNeighborCell(Direction.Forward, i);
                    if (neighbourZ == null || initCells.Contains(neighbourZ)) continue;
                    initCells.Add(neighbourZ);
                }
            }
        }

        private HeightLevel GetEnterStartHeight(GameGridCell enterCell)
        {
            HeightLevel enterStartHeight = Constants.MIN_HEIGHT;
            HeightLevel initHeight = Constants.DirFirstHeightOfSurface[enterCell.SurfaceType];
            if (initHeight > enterStartHeight) enterStartHeight = initHeight;
            for (HeightLevel heightLevel = initHeight;
                 heightLevel <= BelowStartHeight;
                 heightLevel++)
            {
                if (enterCell.GetGridUnitAtHeight(heightLevel) is null) continue;
                if (heightLevel + 1 > enterStartHeight) enterStartHeight = heightLevel + 1;
            }

            return enterStartHeight;
        }

        public HeightLevel GetEnterStartHeight(List<GameGridCell> enterCells)
        {
            HeightLevel enterStartHeight = Constants.MIN_HEIGHT;
            foreach (GameGridCell cell in enterCells)
            {
                HeightLevel initHeight = Constants.DirFirstHeightOfSurface[cell.SurfaceType];
                if (initHeight > enterStartHeight) enterStartHeight = initHeight;
                for (HeightLevel heightLevel = initHeight;
                     heightLevel <= BelowStartHeight;
                     heightLevel++)
                {
                    if (cell.GetGridUnitAtHeight(heightLevel) is null) continue;
                    if (heightLevel + 1 > enterStartHeight) enterStartHeight = heightLevel + 1;
                }
            }

            return enterStartHeight;
        }


        public void RemoveUnitFromCell()
        {
            for (int i = cellInUnits.Count - 1; i >= 0; i--)
                cellInUnits[i].RemoveGridUnit(this);
        }

        private void AddCell(GameGridCell cell)
        {
            cellInUnits.Add(cell);
            cell.AddGridUnit(this);
        }

        #region SAVING DATA
        public virtual IMemento Save()
        {
            return new UnitMemento(this, Tf.position, skin.rotation, startHeight, endHeight
                , unitTypeY, unitTypeXZ,
                belowUnits.Count > 0 ? belowUnits : null,
                neighborUnits.Count > 0 ? neighborUnits : null,
                upperUnits.Count > 0 ? upperUnits : null,
                mainCell, cellInUnits, islandID);
        }
        public struct UnitMemento : IMemento
        {
            GridUnit main;
            #region DATA
            Vector3 position;
            Quaternion rotation;
            HeightLevel startHeight;
            HeightLevel endHeight;
            UnitTypeY unitTypeY;
            UnitTypeXZ unitTypeXZ;
            GridUnit[] belowsUnits;
            GridUnit[] neighborUnits;
            GridUnit[] upperUnits;
            GameGridCell mainCell;
            GameGridCell[] cellInUnits;
            int islandID;

            #endregion
            public UnitMemento(GridUnit main, params object[] data)
            {
                this.main = main;
                position = (Vector3)data[0];
                rotation = (Quaternion)data[1];
                startHeight = (HeightLevel)data[2];
                endHeight = (HeightLevel)data[3];
                unitTypeY = (UnitTypeY)data[4];
                unitTypeXZ = (UnitTypeXZ)data[5];
                if (data[6] != null) belowsUnits = ((HashSet<GridUnit>)data[6]).ToArray();
                else belowsUnits = null;

                if (data[7] != null) neighborUnits = ((HashSet<GridUnit>)data[7]).ToArray();
                else neighborUnits = null;

                if (data[8] != null) upperUnits = ((HashSet<GridUnit>)data[8]).ToArray();
                else upperUnits = null;

                mainCell = (GameGridCell)data[9];
                if (data[10] != null) cellInUnits = ((List<GameGridCell>)data[10]).ToArray();
                else cellInUnits = null;
                islandID = (int)data[11];
            }
            public void Restore()
            {
                main.Tf.position = position;
                main.skin.rotation = rotation;
                main.startHeight = startHeight;
                main.endHeight = endHeight;
                main.unitTypeY = unitTypeY;
                main.unitTypeXZ = unitTypeXZ;

                if (belowsUnits != null)
                {
                    main.belowUnits.Clear();
                    foreach (GridUnit unit in belowsUnits)
                    {
                        main.belowUnits.Add(unit);
                    }
                }

                if (neighborUnits != null)
                {
                    main.neighborUnits.Clear();
                    foreach (GridUnit unit in neighborUnits)
                    {
                        main.neighborUnits.Add(unit);
                    }
                }

                if (upperUnits != null)
                {
                    main.upperUnits.Clear();
                    foreach (GridUnit unit in upperUnits)
                    {
                        main.upperUnits.Add(unit);
                    }
                }

                main.mainCell = mainCell;
                if (cellInUnits != null)
                {
                    main.cellInUnits.Clear();
                    foreach (GameGridCell cell in cellInUnits)
                    {
                        main.cellInUnits.Add(cell);
                    }
                }
            }
        }
        #endregion
    }

    public enum UnitTypeY
    {
        Up = 0,
        Down = 1
    }

    public enum UnitTypeXZ
    {
        None = -1,
        Horizontal = 0,
        Vertical = 1,
        Both = 2
    }

    public class UnitInitData
    {
        public UnitInitData(Vector3Int size, UnitTypeY unitTypeY, Vector3 localSkinPos, Quaternion localSkinRot)
        {
            Size = size;
            UnitTypeY = unitTypeY;
            LocalSkinPos = localSkinPos;
            LocalSkinRot = localSkinRot;
        }

        public Vector3Int Size { get; }
        public UnitTypeY UnitTypeY { get; }
        public Vector3 LocalSkinPos { get; }
        public Quaternion LocalSkinRot { get; }
    }

    public class EnterCellPosData
    {
        public Vector3 finalPos; // consider falling
        public Vector3 initialPos; // not consider falling
        public bool isFalling;
        public HeightLevel startHeight;

        public void SetEnterPosData(Direction direction, HeightLevel startHeightIn, Vector3 initialPosIn,
            Vector3 finalPosIn, float yOffsetDown, bool hasInitialOffset = true)
        {
            startHeight = startHeightIn;
            initialPos = initialPosIn;
            finalPos = finalPosIn;
            isFalling = Math.Abs(finalPosIn.y - initialPosIn.y) > yOffsetDown + 0.01;
            if (!isFalling) initialPos = finalPos;
            if (!isFalling || !hasInitialOffset) return; // make falling before go to the center of next cell
            Vector3Int dirVector3 = Constants.DirVector3[direction];
            initialPos -= new Vector3(dirVector3.x, 0, dirVector3.z) * Constants.CELL_SIZE / 2;
        }
    }
}

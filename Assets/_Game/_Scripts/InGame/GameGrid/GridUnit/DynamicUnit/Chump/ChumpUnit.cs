using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.DesignPattern;
using _Game.Managers;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public abstract class ChumpUnit : GridUnitDynamic, IChumpUnit
    {
        [SerializeField] protected ChumpType chumpType;
        [SerializeField] protected bool isOnWater;
        protected ChumpType nextChumpType;

        public ChumpType ChumpType
        {
            get => chumpType;
            private set => chumpType = value;
        }

        public virtual void OnGetNextStateAndType(Direction direction)
        {
            nextUnitState = unitState;
            nextChumpType = chumpType;
            if (unitState == UnitState.Up)
            {
                nextUnitState = UnitState.Down;
                nextChumpType = direction is Direction.Left or Direction.Right
                    ? ChumpType.Horizontal
                    : ChumpType.Vertical;
            }
            // Override handle case when state is down

        }

        public void OnPushChump(Direction direction)
        {
            OnGetNextStateAndType(direction);
            if (nextUnitState == UnitState.Up) OnPushChumpUp(direction);
            else OnPushChumpDown(direction);
        }

        public virtual void OnPushChumpUp(Direction direction)
        {
        }

        public virtual void OnPushChumpDown(Direction direction)
        {
        }

        //
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            chumpType = ChumpType.None;
        }

        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            // base.OnInteract(direction, interactUnit);
            if (isInAction) return;
            OnPushChump(direction);
        }

        private bool CanSpawnRaftAndWaterChump(out List<GameGridCell> createRaftCells,
            out List<GameGridCell> createChumpShortCells,
            out HashSet<ChumpUnit> waterChumps)
        {
            createRaftCells = new List<GameGridCell>();
            createChumpShortCells = new List<GameGridCell>();
            waterChumps = new HashSet<ChumpUnit>();
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                if (cell.GetSurfaceType() is not GridSurfaceType.Water) return false;
                GridUnit unit = cell.GetGridUnitAtHeight(BelowStartHeight);
                switch (unit)
                {
                    case RaftUnit:
                        return false;
                    case null:
                        createChumpShortCells.Add(cell);
                        continue;
                    case ChumpUnit waterChump:
                        if (waterChump.ChumpType != nextChumpType) return false;
                        waterChumps.Add(waterChump);
                        createRaftCells.Add(cell);
                        break;
                }
            }
            // Loop through all waterChumps to check if they has larger size than this, is yes, get the cell that this chump doesn't have, add it to createChumpShortCells
            // May has a bugs here
            foreach (GameGridCell cell in from waterChump in waterChumps
                     where waterChump.size.x > size.x || waterChump.size.z > size.z
                     select waterChump.cellInUnits.Find(t => !cellInUnits.Contains(t)))
            {
                if (cell is null) return false;
                createChumpShortCells.Add(cell);
            }
            return true;
        }

        private void SpawnRaftAndWaterChump(IReadOnlyList<GameGridCell> createRaftCells,
            IReadOnlyList<GameGridCell> createChumpShortCells, HashSet<ChumpUnit> waterChumps)
        {
            // TODO: Lock isInAction and Animation for this function
            for (int i = 0; i < createRaftCells.Count; i++)
            {
                ChumpType type = ChumpType == ChumpType.None ? waterChumps.First().ChumpType : ChumpType;
                SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Raft))
                    .OnInit(createRaftCells[i], type);
            }

            for (int i = 0; i < createChumpShortCells.Count; i++)
                SpawnWaterChumpShort(createChumpShortCells[i], nextChumpType);
            foreach (ChumpUnit chump in waterChumps) chump.OnDespawn();
            OnDespawn();
            Debug.Log("Create Raft");
        }

        private void SpawnWaterChumpShort(GameGridCell spawnCell, ChumpType createdChumpType)
        {
            ChumpUnit chumpUnit =
                SimplePool.Spawn<ChumpUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpShort));
            chumpUnit.unitState = UnitState.Down;
            chumpUnit.OnInit(spawnCell, Constants.dirFirstHeightOfSurface[GridSurfaceType.Water], false);
            chumpUnit.ChumpType = createdChumpType;
            chumpUnit.skin.localRotation =
                Quaternion.Euler(chumpUnit.ChumpType is ChumpType.Horizontal
                    ? Constants.horizontalSkinRotation
                    : Constants.verticalSkinRotation);
            chumpUnit.isOnWater = true;
        }

        protected void OnFallAtWaterSurface()
        {
            if (mainCell.GetSurfaceType() is not GridSurfaceType.Water) return;
            if (startHeight == Constants.dirFirstHeightOfSurface[GridSurfaceType.Water])
                isOnWater = true;
            else if (startHeight == Constants.dirFirstHeightOfSurface[GridSurfaceType.Water] + Constants.UPPER_HEIGHT &&
                     CanSpawnRaftAndWaterChump(out List<GameGridCell> createRaftCells,
                         out List<GameGridCell> createChumpShortCells,
                         out HashSet<ChumpUnit> waterChumps))
                SpawnRaftAndWaterChump(createRaftCells, createChumpShortCells, waterChumps);
            else
                Debug.Log("Has Raft Below");
        }

        // SPAGHETTI CODE
        protected override void OnNotFall()
        {
            base.OnNotFall();
            if (cellInUnits.Any(t => t.GetSurfaceType() is not GridSurfaceType.Water)) return;
            if (startHeight == Constants.dirFirstHeightOfSurface[GridSurfaceType.Water] + Constants.UPPER_HEIGHT &&
                CanSpawnRaftAndWaterChump(out List<GameGridCell> createRaftCells,
                    out List<GameGridCell> createChumpShortCells,
                    out HashSet<ChumpUnit> waterChumps))
                SpawnRaftAndWaterChump(createRaftCells, createChumpShortCells, waterChumps);
            else
                Debug.Log("Has Raft below on NotFall");
        }

        protected void MoveChump(Direction direction)
        {
            if (!CanMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            isInAction = true;
            OnOutCurrentCells();
            Vector3 newPosition = GetUnitNextWorldPos(nextMainCell);
            Tf.DOMove(newPosition, Constants.MOVING_TIME).SetEase(Ease.Linear).OnComplete(() =>
            {
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, OnFallAtWaterSurface);
            });
        }

        protected void RollChump(Direction direction)
        {
            if (!CanRotateMove(direction, out Vector3Int sizeAfterRotate,
                    out HeightLevel endHeightAfterRotate, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            isInAction = true;
            StartCoroutine(Roll(direction, OnRollComplete));
            // RollTween(direction, OnRollComplete);
            return;

            void OnRollComplete()
            {
                size = sizeAfterRotate;
                endHeight = endHeightAfterRotate;
                // TODO: Take all GridUnitBase in cellInUnits which above this and handle them later
                Vector3 skinOffset = nextMainCell.WorldPos - mainCell.WorldPos;
                skin.position -= skinOffset;
                OnOutCurrentCells();
                Tf.position = GetUnitNextWorldPos(nextMainCell);
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, OnFallAtWaterSurface);
                // TODO: Handle the above of old cellUnits
                if (unitState == nextUnitState && !isInAction && gameObject.activeSelf) OnPushChump(direction);
                unitState = nextUnitState;
                chumpType = nextChumpType;
            }
        }

        protected IEnumerator Roll(Direction direction, Action callback)
        {
            anchor.ChangeAnchorPos(this, direction);
            Vector3 axis = Vector3.Cross(Vector3.up, Constants.dirVector3[direction]);
            for (int i = 0; i < 90 / 5; i++)
            {
                skin.RotateAround(anchor.Tf.position, axis, 5);
                yield return new WaitForSeconds(0.01f);
            }

            callback?.Invoke();
        }
    }

    public enum ChumpType
    {
        None = -1,
        Horizontal = 0,
        Vertical = 1
    }
}

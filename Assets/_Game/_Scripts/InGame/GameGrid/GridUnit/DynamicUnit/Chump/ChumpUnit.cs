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
        public virtual void OnGetNextStateAndType(Direction direction)
        {
            nextUnitState = unitState;
            nextUnitType = unitType;
            if (unitState != UnitState.Up) return;
            nextUnitState = UnitState.Down;
            nextUnitType = direction is Direction.Left or Direction.Right
                ? UnitType.Horizontal
                : UnitType.Vertical;
            // Override handle case when state is down
        }

        protected virtual void SpawnRaftPrefab(UnitType type)
        { }

        private void SpawnShortRaftPrefab(GameGridCell cellInit, UnitType type)
        {
            RaftUnit raft = SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Raft));
            raft.OnInit(cellInit, type);
            raft.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(raft);
        }

        public void OnPushChump(Direction direction)
        {
            if (_isOnWater)
            {
                // Bugs on not move (check null ground)
                OnMoveChumpWater(direction);
                return;
            }
            OnGetNextStateAndType(direction);
            if (nextUnitState == UnitState.Up) OnPushChumpUp(direction);
            else OnPushChumpDown(direction);
        }

        private void OnMoveChumpWater(Direction direction)
        {
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }
            if (nextCells.Any(cell => cell.SurfaceType is GridSurfaceType.Ground))
            {
                return;
            }
            isInAction = true;
            Vector3 newPosition = GetUnitNextWorldPos(nextMainCell);
            Tf.DOMove(newPosition, Constants.MOVING_TIME).SetEase(Ease.Linear).OnComplete(() =>
            {
                HashSet<GridUnit> aboveUnits = new();
                for (int i = 0; i < cellInUnits.Count; i++)
                {
                    GameGridCell cell = cellInUnits[i];
                    for (HeightLevel j = endHeight + 1; j <= cell.GetMaxHeight(); j++)
                    {
                        GridUnit unit = cell.GetGridUnitAtHeight(j);
                        if (unit is null) continue;
                        if (unit is GridUnitDynamic) aboveUnits.Add(unit);
                    }
                }
                // make all aboveUnits fall
                OnOutCurrentCells();
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, AfterChumpFall);
                foreach (GridUnit unit in aboveUnits)
                {
                    if (unit is GridUnitDynamic dynamicUnit && dynamicUnit.CanFall(out int numHeightDown)) dynamicUnit.OnFall(numHeightDown, AfterChumpFall);
                }
                if (unitState == nextUnitState && !isInAction && gameObject.activeSelf) OnMoveChumpWater(direction);
            });
        }
        
        public virtual void OnPushChumpUp(Direction direction)
        {
        }

        public virtual void OnPushChumpDown(Direction direction)
        {
        }

        //
        [SerializeField] private bool _isOnWater;
        
        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One,
            bool isUseInitData = true)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData);
            unitType = UnitType.None;
            _isOnWater = mainCellIn.SurfaceType is GridSurfaceType.Water;
        }

        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            base.OnInteract(direction, interactUnit);
            if (isInAction) return;
            OnPushChump(direction);
        }

        private bool CanSpawnRaftAndWaterChump(out List<GameGridCell> createShortRaftCells,
            out List<GameGridCell> createChumpShortCells,
            out HashSet<ChumpUnit> waterChumps)
        {
            createShortRaftCells = new List<GameGridCell>();
            createChumpShortCells = new List<GameGridCell>();
            waterChumps = new HashSet<ChumpUnit>();
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                if (cell.SurfaceType is not GridSurfaceType.Water) return false;
                GridUnit unit = cell.GetGridUnitAtHeight(BelowStartHeight);
                switch (unit)
                {
                    case RaftUnit:
                        return false;
                    case null:
                        createChumpShortCells.Add(cell);
                        continue;
                    case ChumpUnit waterChump:
                        if (waterChump.UnitType != nextUnitType && nextUnitType != UnitType.None) return false;
                        waterChumps.Add(waterChump);
                        createShortRaftCells.Add(cell);
                        break;
                }
            }
            
            // If count of createRaftCells equal to count of cellInUnits:
            // => clear the createRaftCells to tell next function spawn the raft which has same size with it at mainCell
            // ELse the next function spawn rafts which has size 1x1
            if (createShortRaftCells.Count == cellInUnits.Count) createShortRaftCells.Clear();
            
            // Loop through all waterChumps to check if they has unit that not be in cellInUnits, if has, add to createChumpShortCells

            foreach (ChumpUnit chump in waterChumps)
            {
                for (int i = 0; i < chump.cellInUnits.Count; i++)
                {
                    GameGridCell cell = chump.cellInUnits[i];
                    if (cell.SurfaceType is not GridSurfaceType.Water) return false;
                    if (cellInUnits.Contains(cell)) continue;
                    createChumpShortCells.Add(cell);
                }
            }

            return true;
        }

        private void SpawnRaftAndWaterChump(IReadOnlyList<GameGridCell> createRaftCells,
            IReadOnlyList<GameGridCell> createChumpShortCells, HashSet<ChumpUnit> waterChumps)
        {
            UnitType spawnType = nextUnitType == UnitType.None ? waterChumps.First().UnitType : UnitType;
            // if createRaftCells is cleared, it means that the raft will be spawn at mainCell
            if (createRaftCells.Count == 0) SpawnRaftPrefab(spawnType);
            for (int i = 0; i < createRaftCells.Count; i++)
            {
                SpawnShortRaftPrefab(createRaftCells[i], spawnType);
            }

            for (int i = 0; i < createChumpShortCells.Count; i++)
                SpawnWaterChumpShort(createChumpShortCells[i], spawnType);
            foreach (ChumpUnit chump in waterChumps) chump.OnDespawn();
            OnDespawn();
            Debug.Log("Create Raft");
        }

        private void SpawnWaterChumpShort(GameGridCell spawnCell, UnitType createdUnitType)
        {
            ChumpUnit chumpUnit =
                SimplePool.Spawn<ChumpUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.ChumpShort));
            chumpUnit.unitState = UnitState.Down;
            chumpUnit.OnInit(spawnCell, Constants.dirFirstHeightOfSurface[GridSurfaceType.Water], false);
            chumpUnit.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(chumpUnit);
            chumpUnit.UnitType = createdUnitType;
            chumpUnit.skin.localRotation =
                Quaternion.Euler(chumpUnit.UnitType is UnitType.Horizontal
                    ? Constants.horizontalSkinRotation
                    : Constants.verticalSkinRotation);
        }

        protected void AfterChumpFall()
        {
            if (mainCell.SurfaceType is not GridSurfaceType.Water)
            {
                if (unitState is not UnitState.Down) return;
                if ((unitType is UnitType.Horizontal && lastPushedDirection is Direction.Back or Direction.Forward)
                    || (unitType is UnitType.Vertical && lastPushedDirection is Direction.Left or Direction.Right))
                    OnPushChump(lastPushedDirection);
                return;
            }
            if (startHeight == Constants.dirFirstHeightOfSurface[GridSurfaceType.Water])
            {
                _isOnWater = true;
                if (nextUnitType is UnitType.None)
                {
                    // Temporary only need to handle this case with short chump, so just remove the unit at the endHeight
                    // TODO: Change later because it will be make bugs when extend with object can rotate like chumpShort but has larger size
                    mainCell.RemoveGridUnitAtHeight(endHeight);
                    // Change the chump to down and rotate skin
                    // May be it will be conflict with the Roll or Move chump Function because of running when complete a tween, which make this function call after the two before function done
                    size = GridUnitFunc.RotateSize(lastPushedDirection, size);
                    endHeight = startHeight + (size.y - 1) * 2;
                    unitType = lastPushedDirection is Direction.Left or Direction.Right
                        ? UnitType.Horizontal
                        : UnitType.Vertical;
                    if (!isMinusHalfSizeY && unitState == UnitState.Up) endHeight += 1;
                    unitState = UnitState.Down;
                    skin.localRotation =
                        Quaternion.Euler(unitType is UnitType.Horizontal
                            ? Constants.horizontalSkinRotation
                            : Constants.verticalSkinRotation);
                    // minus position offsetY
                    Tf.position -= Vector3.up * yOffsetOnDown;
                }
            }
            else if (startHeight == Constants.dirFirstHeightOfSurface[GridSurfaceType.Water] + Constants.UPPER_HEIGHT &&
                     CanSpawnRaftAndWaterChump(out List<GameGridCell> createRaftCells,
                         out List<GameGridCell> createChumpShortCells,
                         out HashSet<ChumpUnit> waterChumps))
            {
                SpawnRaftAndWaterChump(createRaftCells, createChumpShortCells, waterChumps);
            }
            else
            {
                Debug.Log("Has Raft Below");
            }
        }

        // SPAGHETTI CODE
        protected override void OnNotFall()
        {
            base.OnNotFall();
            if (cellInUnits.Any(t => t.SurfaceType is not GridSurfaceType.Water)) return;
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
            if (HasObstacleIfMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            isInAction = true;
            
            Vector3 newPosition = GetUnitNextWorldPos(nextMainCell);
            Tf.DOMove(newPosition, Constants.MOVING_TIME).SetEase(Ease.Linear).OnComplete(() =>
            {
                HashSet<GridUnit> aboveUnits = new();
                for (int i = 0; i < cellInUnits.Count; i++)
                {
                    GameGridCell cell = cellInUnits[i];
                    for (HeightLevel j = endHeight + 1; j <= cell.GetMaxHeight(); j++)
                    {
                        GridUnit unit = cell.GetGridUnitAtHeight(j);
                        if (unit is null) continue;
                        if (unit is GridUnitDynamic) aboveUnits.Add(unit);
                    }
                }
                // make all aboveUnits fall
                OnOutCurrentCells();
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, AfterChumpFall);
                foreach (GridUnit unit in aboveUnits)
                {
                    if (unit is GridUnitDynamic dynamicUnit && dynamicUnit.CanFall(out int numHeightDown)) dynamicUnit.OnFall(numHeightDown, AfterChumpFall);
                }
            });
        }
        
        protected void RollChump(Direction direction)
        {
            if (HasObstacleIfRotateMove(direction, out Vector3Int sizeAfterRotate,
                    out HeightLevel endHeightAfterRotate, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            isInAction = true;
            // StartCoroutine(Roll(direction, OnRollComplete));
            RollTween(direction, OnRollComplete);
            return;

            void OnRollComplete()
            {
                size = sizeAfterRotate;
                endHeight = endHeightAfterRotate;
                // TODO: Take all GridUnitBase in cellInUnits which above this and handle them later
                HashSet<GridUnit> aboveUnits = new();
                for (int i = 0; i < cellInUnits.Count; i++)
                {
                    GameGridCell cell = cellInUnits[i];
                    for (HeightLevel j = endHeight + 1; j <= cell.GetMaxHeight(); j++)
                    {
                        GridUnit unit = cell.GetGridUnitAtHeight(j);
                        if (unit is null) continue;
                        if (unit is GridUnitDynamic) aboveUnits.Add(unit);
                    }
                }
                Vector3 skinOffset = nextMainCell.WorldPos - mainCell.WorldPos;
                skin.position -= skinOffset;
                OnOutCurrentCells();
                Tf.position = GetUnitNextWorldPos(nextMainCell);
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, AfterChumpFall);
                // make all aboveUnits fall
                foreach (GridUnit unit in aboveUnits)
                {
                    if (unit is GridUnitDynamic dynamicUnit && dynamicUnit.CanFall(out int numHeightDown)) dynamicUnit.OnFall(numHeightDown, AfterChumpFall);
                }
                // TODO: Handle the above of old cellUnits
                if (unitState == nextUnitState && !isInAction && gameObject.activeSelf) OnPushChump(direction);
                unitState = nextUnitState;
                unitType = nextUnitType;
            }
        }

        private void RollTween(Direction direction, Action callback)
        {
            anchor.ChangeAnchorPos(this, direction);
            Vector3 axis = Vector3.Cross(Vector3.up, Constants.dirVector3[direction]);
            // Roll 90 degree around anchor in 0.18 second using Tween
            float lastAngle = 0;
            DOVirtual.Float(0, 90, Constants.MOVING_TIME, i =>
            {
                skin.RotateAround(anchor.Tf.position, axis, i - lastAngle);
                lastAngle = i;
            }).OnComplete(() => { callback?.Invoke(); });
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
}

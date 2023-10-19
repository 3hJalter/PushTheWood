using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.GameGrid;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.StaticUnit;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.InGame.GameGrid.GridUnit.DynamicUnit
{
    public abstract class ChumpUnit : GridUnitDynamic, IChumpUnit
    {
        protected ChumpState nextChumpState;
        protected ChumpType nextChumpType;
        protected ChumpState chumpState;
        [SerializeField] protected ChumpType chumpType;

        public ChumpType ChumpType => chumpType;


        public void OnGetNextStateAndType(Direction direction)
        {
            nextChumpState = chumpState;
            nextChumpType = chumpType;
            switch (chumpState)
            {
                case ChumpState.Up:
                    nextChumpState = ChumpState.Down;
                    nextChumpType = direction is Direction.Left or Direction.Right
                        ? ChumpType.Horizontal
                        : ChumpType.Vertical;
                    break;
                case ChumpState.Down:
                    if (chumpType == ChumpType.Horizontal)
                    {
                        if (direction is Direction.Left or Direction.Right) nextChumpState = ChumpState.Up;
                    }
                    else
                    {
                        if (direction is Direction.Forward or Direction.Back) nextChumpState = ChumpState.Up;
                    }
                    break;
            }
        }

        public virtual void OnPushChumpUp(Direction direction)
        {
        }

        public virtual void OnPushChumpDown(Direction direction)
        {
        }

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One)
        {
            base.OnInit(mainCellIn, startHeightIn);
            chumpType = ChumpType.Horizontal;
            chumpState = ChumpState.Up;
        }

        public override void OnInteract(Direction direction, _Game.GameGrid.GridUnit.GridUnit interactUnit = null)
        {
            // base.OnInteract(direction, interactUnit);
            if (isInAction) return;
            OnPushChump(direction);
        }

        protected virtual void SpawnBridge()
        {
            Debug.Log("Create Bridge");
        }

        protected virtual void SpawnRaft()
        {
            // Create Raft and animation for it
            Debug.Log("Create Raft");
        }

        protected void OnFallAtWaterSurface()
        {
            if (mainCell.GetSurfaceType() is not GridSurfaceType.Water) return;
            if (startHeight == HeightLevel.Zero)
                SpawnBridge();
            else if (startHeight == HeightLevel.One && CanSpawnRaft())
                SpawnRaft();
            else
                Debug.Log("Has Raft Below");
        }

        protected override void OnNotFall()
        {
            if (cellInUnits.Any(t => t.GetSurfaceType() is not GridSurfaceType.Water)) return;
            if (startHeight == HeightLevel.One && CanSpawnRaft())
                SpawnRaft();
            else
                Debug.Log("Has Raft below on NotFall");
        }

        private bool CanSpawnRaft()
        {
            // for (int i = 0; i < cellInUnits.Count; i++)
            // {
            //     GameGridCell cell = cellInUnits[i];
            //     if (cell.GetSurfaceType() is GridSurfaceType.Water)
            //     {
            //         if (cell.GetGridUnitAtHeight(startHeight - 1) is BridgeUnit) return false;
            //     }
            // }
            for (int i = 0; i < cellInUnits.Count; i++)
            {
                GameGridCell cell = cellInUnits[i];
                if (cell.GetSurfaceType() is GridSurfaceType.Water)
                {
                    if (cell.GetGridUnitAtHeight(startHeight - 1) is BridgeUnit bridgeUnit)
                    {
                        if ((int) bridgeUnit.BridgeType != (int) nextChumpType) continue;
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnPushChump(Direction direction)
        {
            OnGetNextStateAndType(direction);
            if (nextChumpState == ChumpState.Up) OnPushChumpUp(direction);
            else OnPushChumpDown(direction);
        }

        public ChumpState NextChumpState => nextChumpState;

        protected void MoveChump(Direction direction)
        {
            if (!CanMove(direction, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<_Game.GameGrid.GridUnit.GridUnit> nextUnits))
            {
                OnNotMove(direction, nextUnits, this);
                return;
            }

            isInAction = true;
            OnOutCurrentCells();
            Vector3 newPosition = GetUnitNextWorldPos(nextMainCell);
            Tf.DOMove(newPosition, 0.4f).SetEase(Ease.Linear).OnComplete(() =>
            {
                isInAction = false;
                OnEnterNextCells(nextMainCell, nextCells, OnFallAtWaterSurface);
            });
        }

        protected void RollChump(Direction direction)
        {
            if (!CanRotateMove(direction, out Vector3Int sizeAfterRotate,
                    out HeightLevel endHeightAfterRotate, out GameGridCell nextMainCell,
                    out HashSet<GameGridCell> nextCells, out HashSet<_Game.GameGrid.GridUnit.GridUnit> nextUnits))
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
                if (chumpState == nextChumpState && !isInAction) OnPushChump(direction);
                chumpState = nextChumpState;
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
        Horizontal = 0,
        Vertical = 1
    }

    public enum ChumpState
    {
        Up = 0,
        Down = 1
    }
}

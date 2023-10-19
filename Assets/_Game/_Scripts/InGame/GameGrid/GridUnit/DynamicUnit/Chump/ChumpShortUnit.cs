﻿using _Game.DesignPattern;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.InGame.GameGrid.GridUnit.DynamicUnit;
using _Game.Managers;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpShortUnit : ChumpUnit
    {
        protected override void SpawnBridge()
        {
            if (chumpType is ChumpType.Horizontal)
                SimplePool.Spawn<BridgeUnit>(
                        DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.BridgeShortHorizontal))
                    .OnInit(mainCell, HeightLevel.ZeroPointFive);
            else
                SimplePool.Spawn<BridgeUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.BridgeShortVertical))
                    .OnInit(mainCell, HeightLevel.ZeroPointFive);
            OnDespawn();
        }

        public override void OnGetNextStateAndType(Direction direction)
        {
            base.OnGetNextStateAndType(direction);
            if (unitState != UnitState.Down) return;
            switch (chumpType)
            {
                case ChumpType.Horizontal when direction is Direction.Left or Direction.Right:
                case ChumpType.Vertical when direction is Direction.Forward or Direction.Back:
                    nextUnitState = UnitState.Up;
                    break;
            }
        }

        public override void OnPushChumpUp(Direction direction)
        {
            RollChump(direction);
        }

        public override void OnPushChumpDown(Direction direction)
        {
            RollChump(direction);
        }

        // SPAGHETTI CODE, change later
        public override void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
        {
            OnGetNextStateAndType(direction);

            GridUnit aboveUnit = treeRootUnit.GetAboveUnit();
            if (aboveUnit != null)
            {
                aboveUnit.OnInteract(direction);
                return;
            }

            if (mainCell.GetGridUnitAtHeight(endHeight + 1) != null) return;
            isInAction = true;
            Vector3 nextPos = treeRootUnit.GetMainCellWorldPos();
            Vector3 offsetY = new(0, ((float) startHeight + 1) / 2 * Constants.CELL_SIZE, 0);
            nextPos += offsetY;
            
            StartCoroutine(Roll(direction, () =>
            {
                size = RotateSize(direction, size);
                startHeight += 1;
                endHeight = startHeight + size.y - 1;
                Vector3 skinOffset = treeRootUnit.GetMainCellWorldPos() - mainCell.WorldPos;
                skin.position -= skinOffset;
                OnOutCurrentCells();
                Tf.position = nextPos;
                OnEnterNextCells(treeRootUnit.MainCell, null, OnFallAtWaterSurface);
                unitState = nextUnitState;
                chumpType = nextChumpType;
                isInAction = false;
            }));

        }
    }
}

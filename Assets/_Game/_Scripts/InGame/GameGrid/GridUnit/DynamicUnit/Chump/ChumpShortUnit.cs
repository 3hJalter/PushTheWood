using _Game.DesignPattern;
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
                    .OnInit(mainCell, HeightLevel.Zero);
            else
                SimplePool.Spawn<BridgeUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.BridgeShortVertical))
                    .OnInit(mainCell, HeightLevel.Zero);
            OnDespawn();
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
            Vector3 offsetY = new(0, ((int)startHeight + 1) * Constants.CELL_SIZE, 0);
            nextPos += offsetY - treeRootUnit.offsetY;
            
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
                chumpState = nextChumpState;
                chumpType = nextChumpType;
                isInAction = false;
            }));

        }
    }
}

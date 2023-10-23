using _Game.DesignPattern;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.Managers;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpShortUnit : ChumpUnit, IInteractRootTreeUnit
    {
        protected override void SpawnRaftPrefab(ChumpType type)
        {
            RaftUnit raft = SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.Raft));
            raft.OnInit(mainCell, type);
            raft.islandID = islandID;
            GameGridManager.Ins.AddNewUnitToIsland(raft);
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
                    nextChumpType = ChumpType.None;
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
        public void OnInteractWithTreeRoot(Direction direction, TreeRootUnit treeRootUnit)
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
                size = GridUnitFunc.RotateSize(direction, size);
                startHeight += 1;
                endHeight = startHeight + size.y - 1;
                Vector3 skinOffset = treeRootUnit.GetMainCellWorldPos() - mainCell.WorldPos;
                skin.position -= skinOffset;
                OnOutCurrentCells();
                Tf.position = nextPos;
                OnEnterNextCells(treeRootUnit.MainCell, null, AfterChumpFall);
                unitState = nextUnitState;
                chumpType = nextChumpType;
                isInAction = false;
            }));

        }
    }
}

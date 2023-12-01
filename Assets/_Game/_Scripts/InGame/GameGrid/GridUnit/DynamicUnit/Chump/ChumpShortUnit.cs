using _Game.DesignPattern;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;

namespace _Game.GameGrid.Unit.DynamicUnit
{
    public class ChumpShortUnit : ChumpUnit, IInteractRootTreeUnit
    {
        // SPAGHETTI CODE, change later
        public void OnInteractWithTreeRoot(Direction directionIn, TreeRootUnit treeRootUnit)
        {
            if ((unitType is UnitType.Horizontal && lastPushedDirection is Direction.Back or Direction.Forward)
                || (unitType is UnitType.Vertical && lastPushedDirection is Direction.Left or Direction.Right)) return;
            GridUnit aboveUnit = treeRootUnit.GetAboveUnit();
            if (aboveUnit is not null)
            {
                aboveUnit.OnInteract(directionIn);
                return;
            }

            if (mainCell.GetGridUnitAtHeight(endHeight + 1) is not null) return;
            startHeight += 1;
            endHeight += 1;
            // OnGetNextStateAndType(directionIn);
            OnRollChump(directionIn, RuleRollingData.nextSize, RuleRollingData.nextEndHeight,
                RuleRollingData.nextMainCell, RuleRollingData.nextCells);
        }

        protected override void SpawnRaftPrefab(UnitType type)
        {
            RaftUnit raft = SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnit(PoolType.Raft));
            raft.OnInit(mainCell, type);
            raft.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(raft);
        }

        public override void OnGetNextStateAndType(Direction direction)
        {
            base.OnGetNextStateAndType(direction);
            if (unitState != UnitState.Down) return;
            switch (unitType)
            {
                case UnitType.Horizontal when direction is Direction.Left or Direction.Right:
                case UnitType.Vertical when direction is Direction.Forward or Direction.Back:
                    nextUnitState = UnitState.Up;
                    nextUnitType = UnitType.None;
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
            // OnMove(direction); // TEST RULE
        }
    }
}

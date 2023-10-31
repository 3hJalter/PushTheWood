using _Game.DesignPattern;
using _Game.Managers;
using GameGridEnum;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpHighUnit : ChumpUnit
    {
        protected override void SpawnRaftPrefab(UnitType type)
        {
            RaftUnit raft = SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.RaftLong));
            raft.OnInit(mainCell, type);
            raft.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(raft);
        }

        public override void OnPushChumpDown(Direction direction)
        {
            if (unitState == UnitState.Up)
            {
                RollChump(direction);
                return;
            }
            if (unitType == UnitType.Vertical)
            {
                switch (direction)
                {
                    case Direction.Left or Direction.Right:
                        RollChump(direction);
                        break;
                    case Direction.Forward or Direction.Back:
                        MoveChump(direction);
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case Direction.Left or Direction.Right:
                        MoveChump(direction);
                        break;
                    case Direction.Forward or Direction.Back:
                        RollChump(direction);
                        break;
                }
            }
        }
    }
}

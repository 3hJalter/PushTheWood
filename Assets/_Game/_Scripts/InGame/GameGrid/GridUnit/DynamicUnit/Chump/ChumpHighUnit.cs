using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.Managers;
using GameGridEnum;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpHighUnit : ChumpUnit
    {
        protected override void SpawnRaftPrefab(ChumpType type)
        {
            SimplePool.Spawn<RaftUnit>(DataManager.Ins.GetGridUnitDynamic(GridUnitDynamicType.RaftLong))
                .OnInit(mainCell, type);
        }

        public override void OnPushChumpDown(Direction direction)
        {
            if (unitState == UnitState.Up)
            {
                RollChump(direction);
                return;
            }
            if (chumpType == ChumpType.Vertical)
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

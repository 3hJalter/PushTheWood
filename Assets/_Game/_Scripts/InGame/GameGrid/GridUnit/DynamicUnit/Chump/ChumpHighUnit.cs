using _Game.DesignPattern;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.Managers;
using GameGridEnum;

namespace _Game.InGame.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpHighUnit : ChumpUnit
    {
        protected override void SpawnBridge()
        {
            if (chumpType is ChumpType.Horizontal)
                SimplePool.Spawn<BridgeUnit>(
                        DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.BridgeHighHorizontal))
                    .OnInit(mainCell, HeightLevel.Zero);
            else
                SimplePool.Spawn<BridgeUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.BridgeHighVertical))
                    .OnInit(mainCell, HeightLevel.Zero); 
            OnDespawn();
        }
        
        public override void OnPushChumpUp(Direction direction)
        {
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

        public override void OnPushChumpDown(Direction direction)
        {
            RollChump(direction);
        }
    }
}

﻿using _Game.DesignPattern;
using _Game.GameGrid.GridUnit;
using _Game.GameGrid.GridUnit.StaticUnit;
using _Game.Managers;
using GameGridEnum;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class ChumpHighUnit : ChumpUnit
    {
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

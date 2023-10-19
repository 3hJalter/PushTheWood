﻿using _Game.GameGrid.GridUnit;

namespace _Game.InGame.GameGrid.GridUnit.DynamicUnit
{
    public interface IChumpUnit
    {
        void OnPushChump(Direction direction);
        void OnPushChumpUp(Direction direction);
		void OnPushChumpDown(Direction direction);
		void OnGetNextStateAndType(Direction direction);
    }
}

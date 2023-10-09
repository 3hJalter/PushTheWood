using _Game.GameGrid.GridUnit.Base;
using GameGridEnum;

namespace _Game.GameGrid.GridUnit
{
    public class ChumpUnit: GridUnitDynamic
    {
        private ChumpType _chumpType;
        private ChumpState _chumpState;
    }

    public enum ChumpType
    {
        Horizontal = 0,
        Vertical = 1
    }
    
    public enum ChumpState
    {
        Up = 0,
        Down = 1,
    }
}

using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class BelowGroundUnit : GridUnitStatic
    {
        public BelowGroundUnit()
        {
            gridUnitStaticType = GridUnitStaticType.None;
            size = Vector3Int.one;
            startHeight = HeightLevel.Zero;
            endHeight = HeightLevel.ZeroPointFive;
            unitState = UnitState.Up;
            unitType = UnitType.Both;
        }
    }
}

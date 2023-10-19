using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public class GridUnitStatic : GridUnit
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;

        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;
    }
}

using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.Base
{
    public class GridUnitStatic : GridUnit
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;

        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;
    }
}

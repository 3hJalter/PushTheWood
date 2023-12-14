using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnitStatic : GridUnit
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;
        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;
    }
}

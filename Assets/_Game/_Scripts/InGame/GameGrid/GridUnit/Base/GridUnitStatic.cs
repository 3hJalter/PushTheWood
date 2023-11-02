using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit
{
    public abstract class GridUnitStatic : GridUnit
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;

        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;
        
        public PoolType? PoolType => ConvertToPoolType(gridUnitStaticType);
    }
}

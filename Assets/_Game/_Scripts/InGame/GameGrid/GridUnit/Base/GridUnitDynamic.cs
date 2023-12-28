using _Game.GameGrid.Unit.DynamicUnit.Chump;
using GameGridEnum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnitDynamic : GridUnit
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;
        [SerializeField] public Anchor anchor;       
    }
}

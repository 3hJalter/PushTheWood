using _Game.DesignPattern.StateMachine;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit
{
    public abstract class GridUnitDynamic : GridUnit
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;
        [SerializeField] public Anchor anchor;
    }
}

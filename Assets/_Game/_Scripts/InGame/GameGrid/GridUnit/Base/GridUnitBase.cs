using _Game._Scripts.DesignPattern;
using GameGridEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.GameGrid.GridUnit.Base
{
    public class GridUnitBase : GameUnit
    {
        private GameGridCell _currentCell;
        
        public virtual void OnInteract()
        {
        }
        
        public virtual void OnEnterCell(GameGridCell cell)
        {
            _currentCell = cell;
            // TODO: Move Animation and Change Position of GridUnit
            Tf.position = _currentCell.WorldPos;
            // END
        }
        
    }

    public class GridUnitStatic : GridUnitBase
    {
        [SerializeField] protected GridUnitStaticType gridUnitStaticType;
        
        public GridUnitStaticType GridUnitStaticType => gridUnitStaticType;
    }
    
    public class GridUnitDynamic : GridUnitBase
    {
        [SerializeField] protected GridUnitDynamicType gridUnitDynamicType;
        
        public GridUnitDynamicType GridUnitDynamicType => gridUnitDynamicType;
    }
}

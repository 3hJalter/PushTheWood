using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Chump;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Utilities;
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
        public abstract StateEnum CurrentStateId
        {
            get;
            set;
        }

        #region SAVING DATA
        public override IMemento Save()
        {
            IMemento save;
            if (overrideSpawnSave != null)
            {
                save = overrideSpawnSave;
                overrideSpawnSave = null;
            }
            else
            {
                save = new DynamicUnitMemento(this, CurrentStateId, isSpawn, Tf.position, skin.rotation, startHeight, endHeight
                , unitTypeY, unitTypeXZ, belowUnits, neighborUnits, upperUnits, mainCell, cellInUnits, islandID, lastPushedDirection);
            }
            return save;
        }

        public class DynamicUnitMemento : UnitMemento<GridUnitDynamic>
        {
            protected StateEnum currentState;

            public DynamicUnitMemento(GridUnitDynamic main,StateEnum currentState, params object[] data) : base(main, data)
            {
                this.currentState = currentState;
            }

            public override void Restore()
            {
                base.Restore();
                main.CurrentStateId = currentState;
            }
        }
        #endregion
    }
}

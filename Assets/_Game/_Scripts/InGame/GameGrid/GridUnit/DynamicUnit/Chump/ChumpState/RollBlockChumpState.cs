using _Game.DesignPattern.StateMachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Chump.ChumpState
{
    public class RollBlockChumpState : IState<Chump>
    {
        public void OnEnter(Chump t)
        {
            switch (t.UnitTypeY)
            {
                case UnitTypeY.Up:
                    //NOTE: Blocking when chump is ahead
                    break;
                case UnitTypeY.Down:
                    //NOTE: Blocking when chump is down
                    break;
            }
        }

        public void OnExecute(Chump t)
        {
            
        }

        public void OnExit(Chump t)
        {
            
        }

    }
}
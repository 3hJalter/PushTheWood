using Game.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class Chump
    {
        public class ChumpMemento : Memento
        {
            Chump main;
            TREE_TYPE type;
            TREE_STATE state;
            STATE currentState;
            Vector3 rotation;
            GameCell currentCell;

            public ChumpMemento(Chump main)
            {
                this.main = main;
            }

            public override void Save()
            {
                type = main.type;
                state = main.state;
                currentState = main.stateMachine.CurrentStateName;
                rotation = main.transform.rotation.eulerAngles;
                gridPosition = main.gridPosition;
                currentCell = new GameCell(main.currentCell);
            }
            public override void Revert()
            {
                main.type = type;
                main.state = state;
                main.stateMachine.Start(currentState);
                main.transform.rotation = Quaternion.Euler(rotation);
                GameCell cell = main.map.GetGridCell(gridPosition.x, gridPosition.y);
                main.transform.position = cell.WorldPos;
                main.map.SetGridCell(currentCell.X, currentCell.Y, currentCell);
            }

        }
    }
}


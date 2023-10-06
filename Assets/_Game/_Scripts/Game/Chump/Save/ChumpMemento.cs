using _Game.AI;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump
    {
        public class ChumpMemento : Memento
        {
            private readonly Chump main;
            private GameCell currentCell;
            private STATE currentState;
            private Vector3 rotation;
            private TreeState state;
            private TreeType type;

            public ChumpMemento(Chump main)
            {
                this.main = main;
            }

            public override void Save()
            {
                type = main.Type;
                state = main.SState;
                currentState = main.stateMachine.CurrentStateName;
                rotation = main.transform.rotation.eulerAngles;
                gridPosition = main.gridPosition;
                currentCell = new GameCell(main.currentCell);
            }

            public override void Revert()
            {
                main.Type = type;
                main.SState = state;
                main.stateMachine.Start(currentState);
                main.transform.rotation = Quaternion.Euler(rotation);
                GameCell cell = main.map.GetGridCell(gridPosition.x, gridPosition.y);
                main.transform.position = cell.WorldPos;
                main.map.SetGridCell(currentCell.X, currentCell.Y, currentCell);
            }
        }
    }
}

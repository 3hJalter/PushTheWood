using _Game._Scripts.DesignPattern;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump
    {
        public class ChumpUpState : State
        {
            public ChumpUpState(Chump tree, StateMachine<Chump, State> stateMachine) : base(tree, stateMachine)
            {
            }

            public override void Enter()
            {
                base.Enter();
                stateHolder.SState = TreeState.Up;
                currentCell = stateHolder.map.GetGridCell(stateHolder.gridPosition.x, stateHolder.gridPosition.y);
                switch (currentCell.Data.type)
                {
                    case CellType.Water:
                        stateMachine.ChangeState(STATE.TREE_WATER);
                        break;
                }
            }

            public override int LogicUpdate()
            {
                if (nextState != STATE.NONE && !stateHolder.isMoving) stateMachine.ChangeState(nextState);
                return TRUE;

            }

            public override void Move(Vector3 dir)
            {

                #region INIT

                Vector3 anchor = stateHolder.Tf.position;

                currentCell = stateHolder.map.GetGridCell(stateHolder.gridPosition.x, stateHolder.gridPosition.y);
                nextCell = GetNextCell(dir);

                #endregion

                #region CHECKING NEXT CELL

                if (nextCell.IsBlockingRollingTree) return;

                if (nextCell.Tree1 != null && nextCell.Data.type == CellType.Ground)
                {
                    Vector3 direction = new(nextCell.X - currentCell.X, 0, nextCell.Y - currentCell.Y);
                    nextCell.Tree1.MovingTree(direction);
                    return;
                }

                #endregion

                #region MOVING

                if (dir.x != 0) //Horizontal Move
                {
                    anchor += (Vector3.down * stateHolder.treeSize + dir) * 0.5f;
                    stateHolder.Type = TreeType.Horizontal;
                }
                else if (dir.z != 0) //Vertical Move
                {
                    anchor += (Vector3.down * stateHolder.treeSize + dir) * 0.5f;
                    stateHolder.Type = TreeType.Vertical;
                }

                #endregion

                #region UPDATE CELL AND TREE

                desCell = nextCell;
                //Data.Save(currentCell);

                if (currentCell.Tree1 == stateHolder)
                    currentCell.Tree1 = null;
                else if (currentCell.Tree2 == stateHolder) currentCell.Tree2 = null;

                if (desCell.Tree1 == null)
                    desCell.Tree1 = stateHolder;
                else
                    desCell.Tree2 = stateHolder;


                stateHolder.gridPosition.Set(desCell.X, desCell.Y);
                nextState = STATE.TREE_DOWN;
                Vector3 axis = Vector3.Cross(Vector3.up, dir);
                stateHolder.Rolling(anchor, axis, desCell.WorldPos, false);

                #endregion

            }
        }
    }
}

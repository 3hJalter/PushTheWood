using Game.AI;
using Game;
using UnityEngine;

namespace Game
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
                Data.state = TREE_STATE.UP;
                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
                switch (currentCell.Value.type)
                {
                    case CELL_TYPE.WATER:
                        stateMachine.ChangeState(STATE.TREE_WATER);
                        break;
                }
            }

            public override int LogicUpdate()
            {
                if (nextState != STATE.NONE && !Data.isMoving)
                {
                    stateMachine.ChangeState(nextState);
                }
                return TRUE;

            }
            public override void Move(Vector3 dir)
            {
                #region INIT
                Vector3 anchor = Data.transform.position;

                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
                nextCell = GetNextCell(dir);
                #endregion

                #region CHECKING NEXT CELL
                if (nextCell.IsBlockingRollingTree)
                {
                    return;
                }

                if (nextCell.Tree1 != null && nextCell.Value.type == CELL_TYPE.GROUND)
                {
                    Vector3 direction = new Vector3(nextCell.X - currentCell.X, 0, nextCell.Y - currentCell.Y);
                    nextCell.Tree1.MovingTree(direction);
                    return;
                }

                #endregion

                #region MOVING
                if (dir.x != 0) //Horizontal Move
                {
                    anchor += (Vector3.down * Data.treeSize + dir) * 0.5f;
                    Data.type = TREE_TYPE.HORIZONTAL;
                }
                else if (dir.z != 0) //Vertical Move
                {
                    anchor += (Vector3.down * Data.treeSize + dir) * 0.5f;
                    Data.type = TREE_TYPE.VERTICAL;
                }
                #endregion

                #region UPDATE CELL AND TREE
                desCell = nextCell;
                //Data.Save(currentCell);

                if (currentCell.Tree1 == Data)
                {
                    currentCell.Tree1 = null;
                }
                else if (currentCell.Tree2 == Data)
                {
                    currentCell.Tree2 = null;
                }

                if (desCell.Tree1 == null)
                {
                    desCell.Tree1 = Data;
                }
                else
                {
                    desCell.Tree2 = Data;
                }


                Data.gridPosition.Set(desCell.X, desCell.Y);
                nextState = STATE.TREE_DOWN;
                Vector3 axis = Vector3.Cross(Vector3.up, dir);
                Data.Rolling(anchor, axis, desCell.WorldPos, false);
                #endregion
            }
        }
    }
}
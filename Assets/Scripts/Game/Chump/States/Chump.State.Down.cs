using Game.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public partial class Chump
    {
        public class ChumpDownState : State
        {
            private bool isRolling = false;
            private bool isRollHorizontal = false;
            private Vector3 moveDir;
            public ChumpDownState(Chump tree, StateMachine<Chump, State> stateMachine) : base(tree, stateMachine)
            {
            }

            public override void Enter()
            {
                base.Enter();
                Data.state = TREE_STATE.DOWN;
                moveDir = Vector3.zero;
                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
                switch (currentCell.Value.type)
                {
                    case CELL_TYPE.WATER:
                        stateMachine.ChangeState(STATE.TREE_WATER);
                        break;
                }
            }
            public override void Move(Vector3 dir)
            {
                #region INIT
                moveDir = dir;
                Vector3 anchor = Data.transform.position;

                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
                nextCell = GetNextCell(dir);
                #endregion

                #region CHECKING NEXT CELL
                if (nextCell.IsBlockingRollingTree)
                {
                    isRolling = false;
                    return;
                }
                else
                {
                    isRolling = true;
                }

                if (nextCell.Value.type == CELL_TYPE.WATER)
                {
                    isRolling = false;
                    nextState = STATE.TREE_WATER;
                }
                else if (nextCell.Tree1 != null && nextCell.Value.type == CELL_TYPE.GROUND)
                {
                    isRolling = false;
                    Vector3 direction = new Vector3(nextCell.X - currentCell.X, 0, nextCell.Y - currentCell.Y);
                    nextCell.Tree1.MovingTree(direction);
                    return;
                }
                else if (nextCell.Value.state == CELL_STATE.TREE_OBSTANCE)
                {
                    Vector2 direction = new Vector2(nextCell.X - currentCell.X, nextCell.Y - currentCell.Y);
                    if ((Mathf.Abs(direction.x) > 0 && Data.type == TREE_TYPE.VERTICAL) || (Mathf.Abs(direction.y) > 0 && Data.type == TREE_TYPE.HORIZONTAL))
                    {
                        isRolling = false;
                        return;
                    }
                }
                #endregion

                #region MOVING
                if (dir.x != 0) //Horizontal Move
                {
                    switch (Data.type)
                    {
                        case TREE_TYPE.HORIZONTAL: //When tree horizontal down
                            anchor += (Vector3.down + dir * Data.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
                        case TREE_TYPE.VERTICAL:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                    }
                }
                else if (dir.z != 0) //Vertical Move
                {
                    switch (Data.type)
                    {
                        case TREE_TYPE.HORIZONTAL:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                        case TREE_TYPE.VERTICAL: //When tree vertical down
                            anchor += (Vector3.down + dir * Data.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
                    }
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
                Vector3 axis = Vector3.Cross(Vector3.up, dir);
                Data.Rolling(anchor, axis, desCell.WorldPos, isRollHorizontal);
                #endregion

            }
            public override int LogicUpdate()
            {
                if (isEndState) return FALSE;
                if (nextState != STATE.NONE && !Data.isMoving)
                {
                    stateMachine.ChangeState(nextState);
                }

                if (!isRolling) return FALSE;
                if (!Data.isMoving)
                {
                    Move(moveDir);
                }
                return TRUE;
            }

            public override void Exit()
            {
                base.Exit();
                isRolling = false;
            }

        }
    }
}
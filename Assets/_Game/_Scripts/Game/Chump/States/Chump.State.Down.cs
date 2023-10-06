using _Game.AI;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump
    {
        public class ChumpDownState : State
        {
            private bool isRollHorizontal;
            private bool isRolling;
            private Vector3 moveDir;

            public ChumpDownState(Chump tree, StateMachine<Chump, State> stateMachine) : base(tree, stateMachine)
            {
            }

            public override void Enter()
            {
                base.Enter();
                stateHolder.SState = TreeState.Down;
                moveDir = Vector3.zero;
                currentCell = stateHolder.map.GetGridCell(stateHolder.gridPosition.x, stateHolder.gridPosition.y);
                switch (currentCell.Value.type)
                {
                    case CellType.Water:
                        stateMachine.ChangeState(STATE.TREE_WATER);
                        break;
                }
            }

            public override void Move(Vector3 dir)
            {

                #region INIT

                moveDir = dir;
                Vector3 anchor = stateHolder.transform.position;

                currentCell = stateHolder.map.GetGridCell(stateHolder.gridPosition.x, stateHolder.gridPosition.y);
                nextCell = GetNextCell(dir);

                #endregion

                #region CHECKING NEXT CELL

                if (nextCell.IsBlockingRollingTree)
                {
                    isRolling = false;
                    return;
                }

                isRolling = true;

                if (nextCell.Value.type == CellType.Water)
                {
                    isRolling = false;
                    nextState = STATE.TREE_WATER;
                }
                else if (nextCell.Tree1 != null && nextCell.Value.type == CellType.Ground)
                {
                    isRolling = false;
                    Vector3 direction = new(nextCell.X - currentCell.X, 0, nextCell.Y - currentCell.Y);
                    nextCell.Tree1.MovingTree(direction);
                    return;
                }
                else if (nextCell.Value.state == CellState.TreeObstacle)
                {
                    Vector2 direction = new(nextCell.X - currentCell.X, nextCell.Y - currentCell.Y);
                    if ((Mathf.Abs(direction.x) > 0 && stateHolder.Type == TreeType.Vertical) ||
                        (Mathf.Abs(direction.y) > 0 && stateHolder.Type == TreeType.Horizontal))
                    {
                        isRolling = false;
                        return;
                    }
                }

                #endregion

                #region MOVING

                if (dir.x != 0) //Horizontal Move
                    switch (stateHolder.Type)
                    {
                        case TreeType.Horizontal: //When tree horizontal down
                            anchor += (Vector3.down + dir * stateHolder.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
                        case TreeType.Vertical:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                    }
                else if (dir.z != 0) //Vertical Move
                    switch (stateHolder.Type)
                    {
                        case TreeType.Horizontal:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                        case TreeType.Vertical: //When tree vertical down
                            anchor += (Vector3.down + dir * stateHolder.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
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
                Vector3 axis = Vector3.Cross(Vector3.up, dir);
                stateHolder.Rolling(anchor, axis, desCell.WorldPos, isRollHorizontal);

                #endregion

            }

            public override int LogicUpdate()
            {
                if (isEndState) return FALSE;
                if (nextState != STATE.NONE && !stateHolder.isMoving) stateMachine.ChangeState(nextState);

                if (!isRolling) return FALSE;
                if (!stateHolder.isMoving) Move(moveDir);
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

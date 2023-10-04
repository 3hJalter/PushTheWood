using Game.AI;
using UnityEngine;

namespace Game
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
                Data.SState = TreeState.Down;
                moveDir = Vector3.zero;
                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
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
                    if ((Mathf.Abs(direction.x) > 0 && Data.Type == TreeType.Vertical) ||
                        (Mathf.Abs(direction.y) > 0 && Data.Type == TreeType.Horizontal))
                    {
                        isRolling = false;
                        return;
                    }
                }

                #endregion

                #region MOVING

                if (dir.x != 0) //Horizontal Move
                    switch (Data.Type)
                    {
                        case TreeType.Horizontal: //When tree horizontal down
                            anchor += (Vector3.down + dir * Data.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
                        case TreeType.Vertical:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                    }
                else if (dir.z != 0) //Vertical Move
                    switch (Data.Type)
                    {
                        case TreeType.Horizontal:
                            anchor += (Vector3.down + dir) * 0.5f;
                            isRollHorizontal = true;
                            break;
                        case TreeType.Vertical: //When tree vertical down
                            anchor += (Vector3.down + dir * Data.treeSize) * 0.5f;
                            nextState = STATE.TREE_UP;
                            isRollHorizontal = false;
                            break;
                    }

                #endregion

                #region UPDATE CELL AND TREE

                desCell = nextCell;
                //Data.Save(currentCell);

                if (currentCell.Tree1 == Data)
                    currentCell.Tree1 = null;
                else if (currentCell.Tree2 == Data) currentCell.Tree2 = null;

                if (desCell.Tree1 == null)
                    desCell.Tree1 = Data;
                else
                    desCell.Tree2 = Data;

                Data.gridPosition.Set(desCell.X, desCell.Y);
                Vector3 axis = Vector3.Cross(Vector3.up, dir);
                Data.Rolling(anchor, axis, desCell.WorldPos, isRollHorizontal);

                #endregion

            }

            public override int LogicUpdate()
            {
                if (isEndState) return FALSE;
                if (nextState != STATE.NONE && !Data.isMoving) stateMachine.ChangeState(nextState);

                if (!isRolling) return FALSE;
                if (!Data.isMoving) Move(moveDir);
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

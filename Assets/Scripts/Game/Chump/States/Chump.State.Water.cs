using DG.Tweening;
using Game.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public partial class Chump
    {
        public class ChumpWaterState : State
        {
            private Vector3 direction;
            private const float FLOAT_TIME = 0.5f;
            private const float RAFT_HEIGHT = -0.75f;

            bool isRaftMoving = false;

            public ChumpWaterState(Chump tree, StateMachine<Chump, State> stateMachine) : base(tree, stateMachine)
            {
            }
            public override void Enter()
            {
                base.Enter();
                direction = Vector3.zero;

                #region CHEKING RAFT
                GameCell cell = Data.map.GetGridCell(Data.transform.position);
                float originHeight = Data.transform.position.y;
                Vector3 rotateVector = Vector3.zero;
                switch (Data.type)
                {
                    case TREE_TYPE.HORIZONTAL:
                        rotateVector = new Vector3(0, 0, 90);
                        break;
                    case TREE_TYPE.VERTICAL:
                        rotateVector = new Vector3(0, 90, 90);
                        break;
                }
                if (!cell.IsRaft)
                {
                    Data.transform.DOMoveY(RAFT_HEIGHT - 2f, 0.25f).OnComplete(() =>
                    {
                        Data.transform.DORotate(rotateVector, 0.4f);
                        Data.transform.DOMoveY(RAFT_HEIGHT, 0.5f);
                    });
                }
                else
                {
                    float originHeight1 = cell.Tree1.transform.position.y;
                    cell.Tree2.transform.DOMoveY(originHeight1 - 2.5f, 0.25f).OnComplete(() =>
                    {
                        cell.Tree2.transform.DORotate(new Vector3(0, 0, 90), 0.5f);
                        cell.Tree2.transform.DOMove(new Vector3(cell.Tree2.transform.position.x, originHeight1, cell.Tree2.transform.position.z + 0.5f), FLOAT_TIME);
                    });
                    cell.Tree2.state = TREE_STATE.DOWN;
                    cell.Tree2.type = TREE_TYPE.HORIZONTAL;


                    cell.Tree1.transform.DOMoveY(originHeight1 - 2.5f, 0.25f).OnComplete(() =>
                    {
                        cell.Tree1.transform.DORotate(new Vector3(0, 0, 90), 0.5f);
                        cell.Tree1.transform.DOMove(new Vector3(cell.Tree1.transform.position.x, originHeight1, cell.Tree1.transform.position.z - 0.5f), FLOAT_TIME);

                    });
                    cell.Tree1.state = TREE_STATE.DOWN;
                    cell.Tree2.type = TREE_TYPE.HORIZONTAL;
                }
                //Data.Save(currentCell);
                #endregion
            }

            public override void Move(Vector3 dir)
            {
                #region INIT
                direction = dir;
                currentCell = Data.map.GetGridCell(Data.gridPosition.x, Data.gridPosition.y);
                nextCell = GetNextCell(dir);
                #endregion

                #region CHEKING NEXT CELL
                if (nextCell.Value.type == CELL_TYPE.WATER && !nextCell.IsRaft && nextCell.Tree1 == null)
                {
                    isRaftMoving = true;
                    desCell = nextCell;
                }
                else
                {
                    isRaftMoving = false;
                    nextCell = null;
                    return;
                }
                #endregion

                #region UPDATE CELL AND TREE
                if (desCell != null)
                {

                    desCell = nextCell;
                    //Data.Save(currentCell);

                    desCell.Tree1 = currentCell.Tree1;
                    desCell.Tree2 = currentCell.Tree2;
                    desCell.Tree1.gridPosition.Set(desCell.X, desCell.Y);
                    desCell.Tree2.gridPosition.Set(desCell.X, desCell.Y);
                    desCell.Player = currentCell.Player;

                    Vector2Int destination = new Vector2Int(desCell.X, desCell.Y);
                    currentCell.Player.GridPosition = destination;
                    currentCell.Player.MovingOnRaft(destination, Ease.Linear);
                    //currentCell.Player.MovingOnRaft(destination, Ease.Linear);

                    currentCell.Tree1 = null;
                    currentCell.Tree2 = null;
                    currentCell.Player = null;

                    desCell.Tree1.MovingRaft(desCell.WorldPos - Vector3.forward * 0.5f, Ease.Linear);
                    desCell.Tree2.MovingRaft(desCell.WorldPos + Vector3.forward * 0.5f, Ease.Linear);
                }
                #endregion

            }

            public override int LogicUpdate()
            {
                if (Data.isMoving) return FALSE;
                if (isRaftMoving)
                {
                    Move(direction);
                }

                return TRUE;
            }
        }
    }
}
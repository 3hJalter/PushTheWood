using _Game._Scripts.DesignPattern;
using DG.Tweening;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump
    {
        public class ChumpWaterState : State
        {
            private const float FLOAT_TIME = 0.5f;
            private const float RAFT_HEIGHT = -0.75f;
            private Vector3 direction;

            private bool isRaftMoving;

            public ChumpWaterState(Chump tree, StateMachine<Chump, State> stateMachine) : base(tree, stateMachine)
            {
            }

            public override void Enter()
            {
                base.Enter();
                direction = Vector3.zero;

                #region CHEKING RAFT

                GameCell cell = stateHolder.map.GetGridCell(stateHolder.Tf.position);
                float originHeight = stateHolder.Tf.position.y;
                Vector3 rotateVector = Vector3.zero;
                switch (stateHolder.Type)
                {
                    case TreeType.Horizontal:
                        rotateVector = new Vector3(0, 0, 90);
                        break;
                    case TreeType.Vertical:
                        rotateVector = new Vector3(0, 90, 90);
                        break;
                }

                if (!cell.IsRaft)
                {
                    stateHolder.Tf.DOMoveY(RAFT_HEIGHT - 2f, 0.25f).OnComplete(() =>
                    {
                        stateHolder.Tf.DORotate(rotateVector, 0.4f);
                        stateHolder.Tf.DOMoveY(RAFT_HEIGHT, 0.5f);
                    });
                }
                else
                {
                    float originHeight1 = cell.Tree1.Tf.position.y;
                    cell.Tree2.Tf.DOMoveY(originHeight1 - 2.5f, 0.25f).OnComplete(() =>
                    {
                        cell.Tree2.Tf.DORotate(new Vector3(0, 0, 90), 0.5f);
                        cell.Tree2.Tf.DOMove(
                            new Vector3(cell.Tree2.Tf.position.x, originHeight1,
                                cell.Tree2.Tf.position.z + 0.5f), FLOAT_TIME);
                    });
                    cell.Tree2.SState = TreeState.Down;
                    cell.Tree2.Type = TreeType.Horizontal;


                    cell.Tree1.Tf.DOMoveY(originHeight1 - 2.5f, 0.25f).OnComplete(() =>
                    {
                        cell.Tree1.Tf.DORotate(new Vector3(0, 0, 90), 0.5f);
                        Vector3 position = cell.Tree1.Tf.position;
                        cell.Tree1.Tf.DOMove(
                            new Vector3(position.x, originHeight1,
                                position.z - 0.5f), FLOAT_TIME);

                    });
                    cell.Tree1.SState = TreeState.Down;
                    cell.Tree2.Type = TreeType.Horizontal;
                }

                //Data.Save(currentCell);

                #endregion

            }

            public override void Move(Vector3 dir)
            {

                #region INIT

                direction = dir;
                currentCell = stateHolder.map.GetGridCell(stateHolder.gridPosition.x, stateHolder.gridPosition.y);
                nextCell = GetNextCell(dir);

                #endregion

                #region CHEKING NEXT CELL

                if (nextCell.Data.type == CellType.Water && !nextCell.IsRaft && nextCell.Tree1 == null)
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

                    Vector2Int destination = new(desCell.X, desCell.Y);
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
                if (stateHolder.isMoving) return FALSE;
                if (isRaftMoving) Move(direction);

                return TRUE;
            }
        }
    }
}

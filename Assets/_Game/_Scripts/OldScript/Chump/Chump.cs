using System.Collections;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Utilities.Grid;
using DG.Tweening;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump : HMonoBehaviour, IInit
    {
        [SerializeField] private float treeSize;

        [SerializeField] private float rollSpeed;

        private readonly StateMachine<Chump, State> stateMachine = new();

        private GameCell currentCell;
        private Vector2Int gridPosition = Vector2Int.zero;
        private bool isMoving;

        private Grid<GameCell, GameCellData> map;
        private GameCell nextCell;

        public Vector2Int GridPosition => gridPosition;
        public TreeType Type { get; private set; } = TreeType.Vertical;

        public TreeState SState { get; private set; } = TreeState.Down;

        private void Update()
        {
            stateMachine.CurrentState?.LogicUpdate();

        }

        private void FixedUpdate()
        {
            stateMachine.CurrentState?.PhysicUpdate();
        }

        public void OnInit()
        {
            map = OldLevelManager.Ins.Map.GridMap;
            GameCell initCell = map.GetGridCell(Tf.position);
            initCell.Tree1 = this;
            gridPosition.Set(initCell.X, initCell.Y);
            Tf.position = new Vector3(initCell.WorldPos.x, Constants.TREE_HEIGHT, initCell.WorldPos.z);

            stateMachine.PushState(STATE.TREE_UP, new ChumpUpState(this, stateMachine));
            stateMachine.PushState(STATE.TREE_DOWN, new ChumpDownState(this, stateMachine));
            stateMachine.PushState(STATE.TREE_WATER, new ChumpWaterState(this, stateMachine));
            stateMachine.Start(STATE.TREE_UP);
        }

        public void MovingTree(Vector3 dir)
        {
            if (isMoving) return;
            stateMachine.CurrentState.Move(dir);
        }

        private void MovingRaft(Vector3 des, Ease ease = Ease.OutQuad)
        {
            if (isMoving) return;
            isMoving = true;
            Tf.DOMove(new Vector3(des.x, Tf.position.y, des.z), Constants.MOVING_TIME2).SetEase(ease)
                .OnComplete(() => isMoving = false);
        }

        private void Rolling(Vector3 anchor, Vector3 axis, Vector3 des, bool isHorizontal)
        {
            StartCoroutine(Roll(anchor, axis, des, isHorizontal));
        }

        private IEnumerator Roll(Vector3 anchor, Vector3 axis, Vector3 des, bool isHorizontal)
        {
            isMoving = true;
            int count = (int)(90 / rollSpeed);
            Vector3 distance;
            if (isHorizontal)
                distance = (des - Tf.position) / (count * 2);
            else
                distance = (des - Tf.position) / (count * 2 * 1.45f);
            distance.Set(distance.x, 0, distance.z);

            for (int i = 0; i < count; i++)
            {
                Tf.RotateAround(anchor, axis, rollSpeed);
                Tf.position += distance;
                anchor += distance;
                yield return new WaitForSeconds((Constants.MOVING_LOG_TIME - 0.001f) / count);
            }

            Tf.DOMove(new Vector3(des.x, Tf.position.y, des.z), 0.001f).SetEase(Ease.Linear)
                .OnComplete(() => { isMoving = false; });
        }

        #region CHUMP STATE

        public abstract class State : BaseState<Chump>
        {
            protected GameCell currentCell;

            protected GameCell desCell;
            protected GameCell nextCell;
            protected STATE nextState;
            protected readonly StateMachine<Chump, State> stateMachine;

            protected State(Chump tree, StateMachine<Chump, State> stateMachine)
            {
                this.stateMachine = stateMachine;
                stateHolder = tree;
            }

            public override void Enter()
            {
                base.Enter();
                nextState = STATE.NONE;
                desCell = null;
                nextCell = null;
                currentCell = null;
            }

            protected GameCell GetNextCell(Vector3 dir)
            {
                Vector2Int nextPos;
                nextCell = null;
                if (dir.x != 0) //Horizontal Move
                {
                    if (dir.x > 0) //Update Grid Position
                        nextPos = stateHolder.gridPosition + Vector2Int.right;
                    else
                        nextPos = stateHolder.gridPosition + Vector2Int.left;
                    nextCell = stateHolder.map.GetGridCell(nextPos.x, nextPos.y);
                }
                else if (dir.z != 0) //Vertical Move
                {
                    if (dir.z > 0) //Update Grid Position
                        nextPos = stateHolder.gridPosition + Vector2Int.up;
                    else
                        nextPos = stateHolder.gridPosition + Vector2Int.down;
                    nextCell = stateHolder.map.GetGridCell(nextPos.x, nextPos.y);
                }

                return nextCell;
            }

            public void UpdateCell()
            {

            }

            public abstract void Move(Vector3 dir);
        }

        #endregion
    }
}

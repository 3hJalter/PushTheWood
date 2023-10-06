using System.Collections;
using System.Collections.Generic;
using _Game.AI;
using DesignPattern.Grid;
using DG.Tweening;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Chump : MonoBehaviour, IInit
    {
        [SerializeField] private float treeSize;

        [SerializeField] private float rollSpeed;
        private readonly Stack<ChumpMemento> saveSteps = new();

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

            //if (isMoving) return;
            //if (Input.GetKeyDown(KeyCode.A)) stateMachine.CurrentState.Move(Vector3.left);
            //else if (Input.GetKeyDown(KeyCode.D)) stateMachine.CurrentState.Move(Vector3.right);
            //else if (Input.GetKeyDown(KeyCode.W)) stateMachine.CurrentState.Move(Vector3.forward);
            //else if (Input.GetKeyDown(KeyCode.S)) stateMachine.CurrentState.Move(Vector3.back);

        }

        private void FixedUpdate()
        {
            stateMachine.CurrentState?.PhysicUpdate();
        }

        public void OnInit()
        {
            map = LevelManager.Ins.Map.GridMap;
            GameCell initCell = map.GetGridCell(transform.position);
            initCell.Tree1 = this;
            gridPosition.Set(initCell.X, initCell.Y);
            transform.position = new Vector3(initCell.WorldPos.x, Constants.TREE_HEIGHT, initCell.WorldPos.z);

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
            transform.DOMove(new Vector3(des.x, transform.position.y, des.z), Constants.MOVING_TIME).SetEase(ease)
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
                distance = (des - transform.position) / (count * 2);
            else
                distance = (des - transform.position) / (count * 2 * 1.45f);
            distance.Set(distance.x, 0, distance.z);

            for (int i = 0; i < count; i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                transform.position += distance;
                anchor += distance;
                yield return new WaitForSeconds((Constants.MOVING_LOG_TIME - 0.001f) / count);
            }

            transform.DOMove(new Vector3(des.x, transform.position.y, des.z), 0.001f).SetEase(Ease.Linear)
                .OnComplete(() => { isMoving = false; });
        }

        private void Save(GameCell currentCell)
        {
            this.currentCell = currentCell;
            saveSteps.Push(new ChumpMemento(this));
        }


        #region CHUMP STATE

        public abstract class State : BaseState<Chump>
        {
            protected GameCell currentCell;

            protected GameCell desCell;
            protected GameCell nextCell;
            protected STATE nextState;
            protected StateMachine<Chump, State> stateMachine;

            public State(Chump tree, StateMachine<Chump, State> stateMachine)
            {
                this.stateMachine = stateMachine;
                Data = tree;
            }

            public override void Enter()
            {
                base.Enter();
                nextState = STATE.NONE;
                desCell = null;
                nextCell = null;
                currentCell = null;
            }

            public GameCell GetNextCell(Vector3 dir)
            {
                Vector2Int nextPos;
                nextCell = null;
                if (dir.x != 0) //Horizontal Move
                {
                    if (dir.x > 0) //Update Grid Position
                        nextPos = Data.gridPosition + Vector2Int.right;
                    else
                        nextPos = Data.gridPosition + Vector2Int.left;
                    nextCell = Data.map.GetGridCell(nextPos.x, nextPos.y);
                }
                else if (dir.z != 0) //Vertical Move
                {
                    if (dir.z > 0) //Update Grid Position
                        nextPos = Data.gridPosition + Vector2Int.up;
                    else
                        nextPos = Data.gridPosition + Vector2Int.down;
                    nextCell = Data.map.GetGridCell(nextPos.x, nextPos.y);
                }

                return nextCell;
            }

            public void UpdateCell()
            {

            }

            public abstract void Move(Vector3 dir);

            public override void Exit()
            {
                base.Exit();
            }
        }

        #endregion
    }
}

using DG.Tweening;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    using Game.AI;
    using System;
    using Utilities.AI;

    public partial class Player : MonoBehaviour, IInit
    {
        public static event Action<int> _OnChangeIsland;
        public Vector3 InitPos1 = new Vector3(5.09999847f, 0, 244.850006f);
        public Vector3 InitPos2 = new Vector3(83.0999985f, 0, 240.850006f);
        public Vector3 InitPos3 = new Vector3(107.099998f, 0, 258.850006f);
        public Vector3 InitPos4 = new Vector3(47.0999985f, 0, 192.850006f);
        public Vector3 InitPos5 = new Vector3(111.099998f, 0, 220.850006f);
        public Vector3 InitPos6 = new Vector3(25.0999985f, 0, 170.850006f);
        public Vector3 InitPos7 = new Vector3(99.0999985f, 0, 158.850006f);

        [Header("Anim")]
        [SerializeField] private Transform _modelTransform = null;
        [SerializeField] private Daivq.PlayerAnimationControl _playerAnimationControl = null;
        [SerializeField] private float _delayPush = 0.5f;
        [SerializeField] private float _durationRotate = 0.25f;
        [SerializeField] private Ease _easeRotate = Ease.OutQuad;
        [SerializeField] Transform model;

        [Header("Control")]
        [SerializeField] private SwipeDetector _swipeDetector = null;
        public Daivq.UIManager _uiManager = null;

        Vector2Int gridPosition = Vector2Int.zero;

        Grid<GameCell, GameCellData> map;
        Vector2Int nextPos = Vector2Int.zero;

        GameCell currentCell;
        GameCell nextCell;
        bool isMoving = false;
        private int currentIslandID = -1;
        private Stack<PlayerMemento> saveSteps = new Stack<PlayerMemento>();
        private PlayerMemento currentSave;
        public int CurrentIslandID
        {
            get => currentIslandID;
            private set
            {
                if (currentIslandID != value)
                {
                    currentIslandID = value;
                    _OnChangeIsland?.Invoke(currentIslandID);
                }
            }
        }


        public void OnInit()
        {
            map = LevelManager.Inst.Map.GridMap;
            GameCell initCell = map.GetGridCell(transform.position);
            LevelManager.Inst.FindIsland(initCell);
            CurrentIslandID = initCell.IslandID;
            gridPosition.Set(initCell.X, initCell.Y);
        }
        public Vector2Int GridPosition
        {
            get => gridPosition;
            set
            {
                if (gridPosition != value)
                {
                    gridPosition = value;
                }
            }
        }
        private void Update()
        {
            isMoving = false;
            Vector2Int moveDir = Vector2Int.zero;

            if (!_swipeDetector.IsBlockPlayerInput.Value)
            {
                moveDir = _swipeDetector.SwipeDirection;
                if (moveDir.x == 0 && moveDir.y == 0)
                {
                    moveDir = _uiManager.MoveDirectionFromButton;
                }
            }

            if (moveDir.x == 0 && moveDir.y == 0)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    moveDir = Vector2Int.left;
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    moveDir = Vector2Int.up;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    moveDir = Vector2Int.right;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    moveDir = Vector2Int.down;
                }
            }

            //if (Input.GetKeyDown(KeyCode.J))
            //{
            //    saveSteps.Pop().Revert();
            //    return;
            //}

            nextPos = gridPosition + moveDir;

            if (moveDir.sqrMagnitude > 0)
            {
                currentCell = map.GetGridCell(gridPosition.x, gridPosition.y);
                nextCell = map.GetGridCell(nextPos.x, nextPos.y);

                //currentSave = new PlayerMemento(this);
                //saveSteps.Push(currentSave);


                if (!currentCell.IsRaft) // Player is not in Raft
                {
                    if (nextCell.IsRaft) //Move to Raft
                    {
                        currentCell.Player = null;
                        GridPosition = nextPos;
                        MovingTo(nextPos, moveDir);
                        transform.DOMoveY(0f, 0.05f).SetDelay(CONSTANTS.MOVING_TIME);
                        nextCell.Player = this;
                    }
                    else if (nextCell.Tree1 != null)
                    {
                        Vector2 direction = new Vector2(nextCell.X - currentCell.X, nextCell.Y - currentCell.Y);
                        switch (nextCell.Value.type)
                        {
                            case CELL_TYPE.GROUND: // Push Log
                                if (currentCell.Value.state == CELL_STATE.TREE_OBSTANCE && nextCell.Tree1.SState != TREE_STATE.UP) //When player in root and tree is down
                                {
                                    if (nextCell.Value.state == CELL_STATE.TREE_OBSTANCE) // When tree in tree root 
                                    {
                                        MoveTree(nextCell.Tree1, direction);
                                        return;
                                    }

                                    switch (nextCell.Tree1.Type) //Tree not in root
                                    {
                                        case TREE_TYPE.HORIZONTAL:
                                            if (Mathf.Abs(direction.x) > 0)
                                            {
                                                currentCell.Player = null;
                                                GridPosition = nextPos;
                                                MovingTo(nextPos, moveDir);
                                                transform.DOMoveY(1f, 0.05f);
                                                nextCell.Player = this;
                                            }
                                            else
                                            {
                                                MoveTree(nextCell.Tree1, direction);
                                            }
                                            break;
                                        case TREE_TYPE.VERTICAL:
                                            if (Mathf.Abs(direction.y) > 0)
                                            {
                                                currentCell.Player = null;
                                                GridPosition = nextPos;
                                                MovingTo(nextPos, moveDir);
                                                transform.DOMoveY(1f, 0.05f);
                                                nextCell.Player = this;
                                            }
                                            else
                                            {
                                                MoveTree(nextCell.Tree1, direction);
                                            }
                                            break;
                                    }
                                }
                                else //When player not in root
                                {
                                    Vector3 dir = new Vector3(nextPos.x - gridPosition.x, 0, nextPos.y - gridPosition.y);
                                    MoveTree(nextCell.Tree1, dir);
                                }
                                break;
                            case CELL_TYPE.WATER: //Move to log in water
                                switch (nextCell.Tree1.Type)
                                {
                                    case TREE_TYPE.HORIZONTAL:
                                        if (Mathf.Abs(direction.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }
                                        break;
                                    case TREE_TYPE.VERTICAL:
                                        if (Mathf.Abs(direction.y) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }
                                        break;
                                }
                                break;
                        }


                    }
                    else if (nextCell.IsBlockingPlayer) //Block by Water and high rock
                    {

                    }
                    else //None
                    {
                        currentCell.Player = null;
                        GridPosition = nextPos;
                        MovingTo(nextPos, moveDir);
                        transform.DOMoveY(0f, 0.05f).SetDelay(CONSTANTS.MOVING_TIME);
                        nextCell.Player = this;
                    }
                }
                else //Player in Raft
                {
                    if (nextCell.Value.type == CELL_TYPE.GROUND && !nextCell.IsBlockingPlayer) //Player move to ground
                    {
                        currentCell.Player = null;
                        GridPosition = nextPos;
                        MovingTo(nextPos, moveDir);
                        nextCell.Player = this;
                    }
                    else if (nextCell.IsCanPushRaft) //Player move raft when push
                    {
                        MoveTree(currentCell.Tree1, new Vector3(currentCell.X - nextCell.X, 0, currentCell.Y - nextCell.Y));
                    }
                    else if (nextCell.Tree1 != null)
                    {
                        Vector3 direction = new Vector3(currentCell.X - nextCell.X, 0, currentCell.Y - nextCell.Y);
                        switch (nextCell.Value.type)
                        {
                            case CELL_TYPE.GROUND:
                                MoveTree(currentCell.Tree1, direction);
                                MoveTree(nextCell.Tree1, -direction);
                                break;
                            case CELL_TYPE.WATER:
                                switch (nextCell.Tree1.Type)
                                {
                                    case TREE_TYPE.HORIZONTAL:
                                        if (Mathf.Abs(direction.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }
                                        break;
                                    case TREE_TYPE.VERTICAL:
                                        if (Mathf.Abs(direction.y) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void MovingTo(Vector2Int pos, Vector2Int direction, Ease ease = Ease.InOutSine)
        {
            GameCell desCell = map.GetGridCell(pos.x, pos.y);
            desCell.Player = null;

            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMoveX(desCell.WorldPos.x, CONSTANTS.MOVING_TIME).SetEase(ease));
            s.Join(transform.DOMoveZ(desCell.WorldPos.z, CONSTANTS.MOVING_TIME).SetEase(ease));
            _playerAnimationControl.Run();
            s.Play().OnComplete(OnMoveDone);
            _modelTransform.DOLocalRotateQuaternion(Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y), Vector3.up), _durationRotate).SetEase(_easeRotate).Play();
            void OnMoveDone()
            {
                _playerAnimationControl.Idle();
                CurrentIslandID = desCell.IslandID;
                LevelManager.Inst.Steps++;
                _uiManager._stepText.text = LevelManager.Inst.Steps.ToString() + " / 150";
            }
        }
        public void SetPosition(Vector3 pos)
        {
            GameCell cell = map.GetGridCell(pos);
            gridPosition.Set(cell.X, cell.Y);
            transform.position = cell.WorldPos;
            cell.Player = this;
        }

        public void MovingOnRaft(Vector2Int pos, Ease ease = Ease.InOutSine)
        {
            GameCell desCell = map.GetGridCell(pos.x, pos.y);
            transform.DOMove(desCell.WorldPos, CONSTANTS.MOVING_TIME).SetEase(ease).OnComplete(() => CurrentIslandID = desCell.IslandID);

            Vector3 deltaPosition = desCell.WorldPos - transform.position;
            deltaPosition.y = 0f;
            _modelTransform.DOLocalRotateQuaternion(Quaternion.LookRotation(-deltaPosition, Vector3.up), _durationRotate).SetEase(_easeRotate).Play();
        }

        private void MoveTree(Chump tree, Vector3 direction)
        {
            StartCoroutine(IEPushAndMoveTree(tree, direction));

            IEnumerator IEPushAndMoveTree(Chump tree, Vector3 direction)
            {
                _playerAnimationControl.Push();
                _modelTransform.DOLocalRotateQuaternion(Quaternion.LookRotation(direction, Vector3.up), _durationRotate).SetEase(_easeRotate).Play();
                yield return new WaitForSeconds(_delayPush);
                tree.MovingTree(direction);
            }
        }
        #region PLAYER STATE
        public class State : BaseState<Player>
        {
            public State(Player Data)
            {
                this.Data = Data;
            }
        }
        #endregion

        #region SAVE
        public class PlayerMemento : Memento
        {
            Player main;
            Vector3 rotation;
            GameCell currentCell;
            //GameCell nextCell;

            public PlayerMemento(Player main)
            {
                this.main = main;
                Save();
            }

            public override void Save()
            {
                rotation = main.model.transform.rotation.eulerAngles;
                gridPosition = main.gridPosition;
                currentCell = new GameCell(main.currentCell);
                //nextCell = new GameCell(main.nextCell);
            }

            public override void Revert()
            {
                main.model.transform.rotation = Quaternion.Euler(rotation);
                main.transform.position = currentCell.WorldPos;
                main.gridPosition = gridPosition;
                main.map.SetGridCell(currentCell.X, currentCell.Y, currentCell);
                //main.map.SetGridCell(nextCell.X, nextCell.X, nextCell);
            }
        }
        #endregion
    }
}
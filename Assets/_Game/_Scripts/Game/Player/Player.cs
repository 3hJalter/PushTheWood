using System;
using System.Collections;
using System.Collections.Generic;
using _Game.AI;
using CnControls;
using Daivq;
using DesignPattern.Grid;
using DG.Tweening;
using MapEnum;
using UnityEngine;

namespace _Game
{
    public partial class Player : MonoBehaviour, IInit
    {
        [Header("Anim")] [SerializeField] private Transform _modelTransform;

        [SerializeField] private PlayerAnimationControl _playerAnimationControl;
        [SerializeField] private float _delayPush = 0.5f;
        [SerializeField] private float _durationRotate = 0.25f;
        [SerializeField] private Ease _easeRotate = Ease.OutQuad;
        [SerializeField] private Transform model;

        [Header("Control")] [SerializeField] private SwipeDetector _swipeDetector;

        // public UIManager _uiManager;

        private Vector2 _input;

        private GameCell currentCell;
        private int currentIslandID = -1;
        private PlayerMemento currentSave;

        private Direction direction;

        private Vector2Int gridPosition = Vector2Int.zero;
        private bool isMoving;

        private Grid<GameCell, GameCellData> map;
        private GameCell nextCell;
        private Vector2Int nextPos = Vector2Int.zero;
        private Stack<PlayerMemento> saveSteps = new();

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

        public Vector2Int GridPosition
        {
            get => gridPosition;
            set
            {
                if (gridPosition != value) gridPosition = value;
            }
        }


        private void Update()
        {
            if (isMoving) return;
            _input = new Vector2(CnInputManager.GetAxisRaw(Constants.HORIZONTAL),
                CnInputManager.GetAxisRaw(Constants.VERTICAL));
            direction = GetDirection();
            if (direction == Direction.None) return;
            isMoving = true;
            Vector2Int moveDir = direction switch
            {
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                _ => Vector2Int.zero
            };
            // Get left, up , right, down from _input Vector2


            // if (!_swipeDetector.IsBlockPlayerInput.Value)
            // {
            //     moveDir = _swipeDetector.SwipeDirection;
            //     if (moveDir.x == 0 && moveDir.y == 0) moveDir = _uiManager.MoveDirectionFromButton;
            // }
            //
            // if (moveDir.x == 0 && moveDir.y == 0)
            // {
            //     if (Input.GetKeyDown(KeyCode.A))
            //         moveDir = Vector2Int.left;
            //     else if (Input.GetKeyDown(KeyCode.W))
            //         moveDir = Vector2Int.up;
            //     else if (Input.GetKeyDown(KeyCode.D))
            //         moveDir = Vector2Int.right;
            //     else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector2Int.down;
            // }

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
                        transform.DOMoveY(0f, 0.05f).SetDelay(Constants.MOVING_TIME);
                        nextCell.Player = this;
                    }
                    else if (nextCell.Tree1 != null)
                    {
                        Vector2 direction = new(nextCell.X - currentCell.X, nextCell.Y - currentCell.Y);
                        switch (nextCell.Value.type)
                        {
                            case CellType.Ground: // Push Log
                                if (currentCell.Value.state == CellState.TreeObstacle &&
                                    nextCell.Tree1.SState != TreeState.Up) //When player in root and tree is down
                                {
                                    if (nextCell.Value.state == CellState.TreeObstacle) // When tree in tree root 
                                    {
                                        MoveTree(nextCell.Tree1, direction);
                                        return;
                                    }

                                    switch (nextCell.Tree1.Type) //Tree not in root
                                    {
                                        case TreeType.Horizontal:
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
                                        case TreeType.Vertical:
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
                                    Vector3 dir = new(nextPos.x - gridPosition.x, 0, nextPos.y - gridPosition.y);
                                    MoveTree(nextCell.Tree1, dir);
                                }

                                break;
                            case CellType.Water: //Move to log in water
                                switch (nextCell.Tree1.Type)
                                {
                                    case TreeType.Horizontal:
                                        if (Mathf.Abs(direction.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }

                                        break;
                                    case TreeType.Vertical:
                                        if (Mathf.Abs(direction.y) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }
                                        else
                                        {
                                            isMoving = false;

                                        }

                                        break;
                                }

                                break;
                        }


                    }
                    else if (nextCell.IsBlockingPlayer) //Block by Water and high rock
                    {
                        isMoving = false;
                    }
                    else //None
                    {
                        currentCell.Player = null;
                        GridPosition = nextPos;
                        MovingTo(nextPos, moveDir);
                        transform.DOMoveY(0f, 0.05f).SetDelay(Constants.MOVING_TIME);
                        nextCell.Player = this;
                    }
                }
                else //Player in Raft
                {
                    if (nextCell.Value.type == CellType.Ground && !nextCell.IsBlockingPlayer) //Player move to ground
                    {
                        currentCell.Player = null;
                        GridPosition = nextPos;
                        MovingTo(nextPos, moveDir);
                        nextCell.Player = this;
                    }
                    else if (nextCell.IsCanPushRaft) //Player move raft when push
                    {
                        MoveTree(currentCell.Tree1,
                            new Vector3(currentCell.X - nextCell.X, 0, currentCell.Y - nextCell.Y));
                    }
                    else if (nextCell.Tree1 != null)
                    {
                        Vector3 direction = new(currentCell.X - nextCell.X, 0, currentCell.Y - nextCell.Y);
                        switch (nextCell.Value.type)
                        {
                            case CellType.Ground:
                                MoveTree(currentCell.Tree1, direction);
                                MoveTree(nextCell.Tree1, -direction);
                                break;
                            case CellType.Water:
                                switch (nextCell.Tree1.Type)
                                {
                                    case TreeType.Horizontal:
                                        if (Mathf.Abs(direction.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }

                                        break;
                                    case TreeType.Vertical:
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


        public void OnInit()
        {
            map = LevelManager.Ins.Map.GridMap;
            GameCell initCell = map.GetGridCell(transform.position);
            LevelManager.Ins.FindIsland(initCell);
            CurrentIslandID = initCell.IslandID;
            gridPosition.Set(initCell.X, initCell.Y);
        }

        private Direction GetDirection()
        {
            // check if _input is zero
            if (_input.sqrMagnitude < 0.01f) return Direction.None;
            float angle = Mathf.Atan2(_input.y, -_input.x);
            _input = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            if (Mathf.Abs(_input.x) > Mathf.Abs(_input.y))
                return _input.x > 0 ? Direction.Left : Direction.Right;
            return _input.y > 0 ? Direction.Up : Direction.Down;
        }

        public static event Action<int> _OnChangeIsland;

        private void MovingTo(Vector2Int pos, Vector2Int direction, Ease ease = Ease.InOutSine)
        {
            GameCell desCell = map.GetGridCell(pos.x, pos.y);
            desCell.Player = null;

            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMoveX(desCell.WorldPos.x, Constants.MOVING_TIME).SetEase(ease));
            s.Join(transform.DOMoveZ(desCell.WorldPos.z, Constants.MOVING_TIME).SetEase(ease));
            _playerAnimationControl.Run();
            s.Play().OnComplete(OnMoveDone);
            _modelTransform
                .DOLocalRotateQuaternion(Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y), Vector3.up),
                    _durationRotate).SetEase(_easeRotate).Play();

            void OnMoveDone()
            {
                _playerAnimationControl.Idle();
                CurrentIslandID = desCell.IslandID;
                LevelManager.Ins.Steps++;
                // _uiManager._stepText.text = LevelManager.Ins.Steps + " / 150";
                isMoving = false;
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
            transform.DOMove(desCell.WorldPos, Constants.MOVING_TIME).SetEase(ease)
                .OnComplete(() => CurrentIslandID = desCell.IslandID);

            Vector3 deltaPosition = desCell.WorldPos - transform.position;
            deltaPosition.y = 0f;
            _modelTransform
                .DOLocalRotateQuaternion(Quaternion.LookRotation(-deltaPosition, Vector3.up), _durationRotate)
                .SetEase(_easeRotate).Play();
        }

        private void MoveTree(Chump tree, Vector3 direction)
        {
            StartCoroutine(IEPushAndMoveTree(tree, direction));

            IEnumerator IEPushAndMoveTree(Chump tree, Vector3 direction)
            {
                _playerAnimationControl.Push();
                _modelTransform.DOLocalRotateQuaternion(Quaternion.LookRotation(direction, Vector3.up), _durationRotate)
                    .SetEase(_easeRotate).Play();
                yield return new WaitForSeconds(_delayPush);
                tree.MovingTree(direction);
                isMoving = false;
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
            private readonly Player main;
            private GameCell currentCell;

            private Vector3 rotation;
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

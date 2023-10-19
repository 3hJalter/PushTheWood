using System;
using System.Collections;
using _Game.DesignPattern;
using _Game.Managers;
using _Game.Utilities.Grid;
using CnControls;
using DG.Tweening;
using MapEnum;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.InGame.Player
{
    public partial class Player : HMonoBehaviour, IInit
    {
        [Header("Anim")] [SerializeField] private Transform modelTransform;

        [SerializeField] private PlayerAnimationControl playerAnimationControl;

        [SerializeField] private float delayPush = 0.2f;

        [SerializeField] private float durationRotate = 0.25f;

        [FormerlySerializedAs("_easeRotate")] [SerializeField]
        private Ease easeRotate = Ease.OutQuad;

        private Vector2 _input;

        private GameCell currentCell;
        private int currentIslandID = -1;

        private Direction direction;

        private Vector2Int gridPosition = Vector2Int.zero;
        private bool isMoving;

        private Grid<GameCell, GameCellData> map;
        private GameCell nextCell;
        private Vector2Int nextPos = Vector2Int.zero;

        public int CurrentIslandID
        {
            get => currentIslandID;
            private set
            {
                if (currentIslandID != value)
                {
                    currentIslandID = value;
                    OnChangeIsland?.Invoke(currentIslandID);
                }
            }
        }

        public Vector2Int GridPosition
        {
            set => gridPosition = value;
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
                Direction.Forward => Vector2Int.up,
                Direction.Back => Vector2Int.down,
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
                        Tf.DOMoveY(0f, 0.05f).SetDelay(Constants.MOVING_TIME);
                        nextCell.Player = this;
                    }
                    else if (nextCell.Tree1 != null)
                    {
                        Vector2 tDirection = new(nextCell.X - currentCell.X, nextCell.Y - currentCell.Y);
                        switch (nextCell.Data.type)
                        {
                            case CellType.Ground: // Push Log
                                if (currentCell.Data.state == CellState.TreeObstacle &&
                                    nextCell.Tree1.SState != TreeState.Up) //When player in root and tree is down
                                {
                                    if (nextCell.Data.state == CellState.TreeObstacle) // When tree in tree root 
                                    {
                                        MoveTree(nextCell.Tree1, tDirection);
                                        return;
                                    }

                                    switch (nextCell.Tree1.Type) //Tree not in root
                                    {
                                        case TreeType.Horizontal:
                                            if (Mathf.Abs(tDirection.x) > 0)
                                            {
                                                currentCell.Player = null;
                                                GridPosition = nextPos;
                                                MovingTo(nextPos, moveDir);
                                                Tf.DOMoveY(1f, 0.05f);
                                                nextCell.Player = this;
                                            }
                                            else
                                            {
                                                MoveTree(nextCell.Tree1, tDirection);
                                            }

                                            break;
                                        case TreeType.Vertical:
                                            if (Mathf.Abs(tDirection.y) > 0)
                                            {
                                                currentCell.Player = null;
                                                GridPosition = nextPos;
                                                MovingTo(nextPos, moveDir);
                                                Tf.DOMoveY(1f, 0.05f);
                                                nextCell.Player = this;
                                            }
                                            else
                                            {
                                                MoveTree(nextCell.Tree1, tDirection);
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
                                        if (Mathf.Abs(tDirection.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }

                                        break;
                                    case TreeType.Vertical:
                                        if (Mathf.Abs(tDirection.y) > 0)
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
                        Tf.DOMoveY(0f, 0.05f).SetDelay(Constants.MOVING_TIME);
                        nextCell.Player = this;
                    }
                }
                else //Player in Raft
                {
                    if (nextCell.Data.type == CellType.Ground && !nextCell.IsBlockingPlayer) //Player move to ground
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
                        Vector3 tDirection = new(currentCell.X - nextCell.X, 0, currentCell.Y - nextCell.Y);
                        switch (nextCell.Data.type)
                        {
                            case CellType.Ground:
                                MoveTree(currentCell.Tree1, tDirection);
                                MoveTree(nextCell.Tree1, -tDirection);
                                break;
                            case CellType.Water:
                                switch (nextCell.Tree1.Type)
                                {
                                    case TreeType.Horizontal:
                                        if (Mathf.Abs(tDirection.x) > 0)
                                        {
                                            currentCell.Player = null;
                                            GridPosition = nextPos;
                                            MovingTo(nextPos, moveDir);
                                            nextCell.Player = this;
                                        }

                                        break;
                                    case TreeType.Vertical:
                                        if (Mathf.Abs(tDirection.y) > 0)
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
            GameCell initCell = map.GetGridCell(Tf.position);
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
            return _input.y > 0 ? Direction.Forward : Direction.Back;
        }

        public static event Action<int> OnChangeIsland;

        private void MovingTo(Vector2Int pos, Vector2Int directionIn, Ease ease = Ease.InOutSine)
        {
            GameCell desCell = map.GetGridCell(pos.x, pos.y);
            desCell.Player = null;

            Sequence s = DOTween.Sequence();
            s.Append(Tf.DOMoveX(desCell.WorldPos.x, Constants.MOVING_TIME).SetEase(ease));
            s.Join(Tf.DOMoveZ(desCell.WorldPos.z, Constants.MOVING_TIME).SetEase(ease));
            playerAnimationControl.Run();
            s.Play().OnComplete(OnMoveDone);
            modelTransform
                .DOLocalRotateQuaternion(
                    Quaternion.LookRotation(new Vector3(directionIn.x, 0f, directionIn.y), Vector3.up),
                    durationRotate).SetEase(easeRotate).Play();

            void OnMoveDone()
            {
                playerAnimationControl.Idle();
                CurrentIslandID = desCell.IslandID;
                LevelManager.Ins.steps++;
                // _uiManager._stepText.text = LevelManager.Ins.Steps + " / 150";
                isMoving = false;
            }
        }

        public void SetPosition(Vector3 pos)
        {
            GameCell cell = map.GetGridCell(pos);
            gridPosition.Set(cell.X, cell.Y);
            Tf.position = cell.WorldPos;
            cell.Player = this;
        }

        public void MovingOnRaft(Vector2Int pos, Ease ease = Ease.InOutSine)
        {
            GameCell desCell = map.GetGridCell(pos.x, pos.y);
            Tf.DOMove(desCell.WorldPos, Constants.MOVING_TIME).SetEase(ease)
                .OnComplete(() => CurrentIslandID = desCell.IslandID);

            Vector3 deltaPosition = desCell.WorldPos - Tf.position;
            deltaPosition.y = 0f;
            modelTransform
                .DOLocalRotateQuaternion(Quaternion.LookRotation(-deltaPosition, Vector3.up), durationRotate)
                .SetEase(easeRotate).Play();
        }

        private void MoveTree(Chump tree, Vector3 directionIn)
        {
            StartCoroutine(IEPushAndMoveTree(tree, directionIn));
            return;

            IEnumerator IEPushAndMoveTree(Chump t, Vector3 d)
            {
                playerAnimationControl.Push();
                modelTransform.DOLocalRotateQuaternion(Quaternion.LookRotation(d, Vector3.up), durationRotate)
                    .SetEase(easeRotate).Play();
                yield return new WaitForSeconds(delayPush);
                t.MovingTree(d);
                isMoving = false;
            }
        }

        #region PLAYER STATE

        public class State : BaseState<Player>
        {
            protected State(Player stateHolder)
            {
                this.stateHolder = stateHolder;
            }
        }

        #endregion
    }
}

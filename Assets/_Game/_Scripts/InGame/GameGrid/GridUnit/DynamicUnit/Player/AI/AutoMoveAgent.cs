using _Game.GameGrid;
using _Game.GameGrid.Unit;
using GameGridEnum;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace _Game.AI
{
    public class AutoMoveAgent : MonoBehaviour
    {
        // Start is called before the first frame update
        protected const int SCAN_DISTANCE = 3;
        protected GridUnitCharacter character;

        private readonly Queue<Direction> moveDirections = new Queue<Direction>();
        private Direction nextDirection;
        public Direction NextDirection {
            get
            {
                if (moveDirections.Count == 0)
                {
                    nextDirection = Direction.None;
                    enabled = false;
                }
                else
                {
                    nextDirection = moveDirections.Dequeue();
                }
                return nextDirection;
            }
            private set => nextDirection = value;
        }
        public void Init(GridUnitCharacter character)
        {
            if (this.character != null)
            {
                this.character._OnCharacterChangePosition -= OnCharacterChangePosition;
            }
            this.character = character;
            this.character._OnCharacterChangePosition += OnCharacterChangePosition;
        }

        public void LoadPath(List<Vector3> path)
        {
            moveDirections.Clear();
            for (int i = 0; i < path.Count - 1; i++)
            {
                float forwardBack = path[i + 1].z - path[i].z;
                float leftRight = path[i + 1].x - path[i].x;

                //NOTE: If 2 point is similar then ignore it
                if (Mathf.Abs(forwardBack) < 0.001f && Mathf.Abs(leftRight) < 0.001f) continue;

                if (Mathf.Abs(forwardBack) > Mathf.Abs(leftRight))
                {
                    if (forwardBack > 0)
                    {
                        moveDirections.Enqueue(Direction.Forward);
                    }
                    else
                    {
                        moveDirections.Enqueue(Direction.Back);
                    }
                }
                else
                {
                    if (leftRight > 0)
                    {
                        moveDirections.Enqueue(Direction.Right);
                    }
                    else
                    {
                        moveDirections.Enqueue(Direction.Left);
                    }
                }
            }
        }

        public (GameGridCell, GameGridCell) GetPathToSitDown()
        {
            moveDirections.Clear();
            GameGridCell waterCell = null;
            GameGridCell playerCell = null;
            bool isFindWater = false;
            for(int i = 0; i <= 3; i++)
            {
                if (isFindWater) break;
                for (int j = 1; j <= SCAN_DISTANCE; j++)
                {
                    playerCell = character.MainCell.GetNeighborCell((Direction)i, j - 1);
                    waterCell = character.MainCell.GetNeighborCell((Direction)i, j);
                    moveDirections.Enqueue((Direction)i);
                    if (waterCell.Data.gridSurfaceType == GridSurfaceType.Water)
                    {
                        isFindWater = true;
                        //NOTE: Checking if the direction between camera and player has objects
                        for (int k = 1; k <= SCAN_DISTANCE; k++)
                        {
                            GameGridCell checkingCell = waterCell.GetNeighborCell((Direction)i, j + k);
                            if (checkingCell != null && checkingCell.GetGridUnitAtHeight(Constants.DirFirstHeightOfSurface[GridSurfaceType.Water]))
                            {
                                isFindWater = false;
                                break;
                            }
                        }
                        if(isFindWater)
                            break;
                    }
                }
                if (!isFindWater)
                {
                    moveDirections.Clear();
                    waterCell = null;
                    playerCell = null;
                }
            }
            return (waterCell, playerCell);
        }

        private void OnCharacterChangePosition()
        {
            if (moveDirections.Count == 0)
            {
                NextDirection = Direction.None;
                enabled = false;
                return;
            }
            NextDirection = moveDirections.Dequeue();
        }
    }
}
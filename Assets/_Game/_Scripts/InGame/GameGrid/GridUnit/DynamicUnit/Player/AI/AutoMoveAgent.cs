using _Game.GameGrid.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace _Game.AI
{
    public class AutoMoveAgent : MonoBehaviour
    {
        // Start is called before the first frame update
        protected GridUnitCharacter character;

        private readonly Queue<Direction> moveDirections = new Queue<Direction>();
        public Direction NextDirection { get; private set; }
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

        public void Run()
        {
            if(moveDirections.Count == 0)
            {
                NextDirection = Direction.None;
                enabled = false;
                return;
            }
            NextDirection = moveDirections.Dequeue();
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
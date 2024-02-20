using _Game.GameGrid.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AutoMoveAgent : MonoBehaviour
{
    // Start is called before the first frame update
    protected GridUnitCharacter character;
    public event Action<Direction> OnMoveToDir;

    private readonly Queue<Direction> moveDirections = new Queue<Direction>();
    public void Init(GridUnitCharacter character)
    {
        if(character != null)
        {
            this.character.OnCharacterIdle -= OnAgentChoosingDirection;           
        }
        this.character = character;
        this.character.OnCharacterIdle += OnAgentChoosingDirection;
    }

    public void LoadPath(List<Vector3> path)
    {
        moveDirections.Clear();
        for (int i = 0; i < path.Count - 1; i++)
        {
            float forwardBack = path[i + 1].z - path[i].z;
            float leftRight = path[i + 1].x - path[i].x;

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

    private void OnAgentChoosingDirection(List<GridUnit> nearUnits)
    {
        Direction moveDirection = moveDirections.Peek();
        int index = (int)moveDirection - 1;
        if (nearUnits[index] == null)
        {
            moveDirections.Dequeue();
        }
        OnMoveToDir?.Invoke(moveDirection);
    }

}

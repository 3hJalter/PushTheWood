using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    [CreateAssetMenu(fileName = "PlacedObjectDatabase", menuName = "ScriptableObjects/PlacedObjectDatabase")]
    public class PlacedObjectDatabaseSO : ScriptableObject
    {
        public List<PlacedObjectData> BuildingObjectDataList;
    }

    [System.Serializable]
    public class PlacedObjectData
    {
        public static Direction GetNextDirection(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Left:
                    return Direction.Forward;
                case Direction.Forward:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Back;
                case Direction.Back:
                    return Direction.Left;
            }
        }

        public static float GetRotationAngle(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Left:
                    return 270f;
                case Direction.Right:
                    return 90f;
                case Direction.Forward:
                    return 0;
                case Direction.Back:
                    return 180f;
            }
        }

        public int ID;
        public string Name;
        public int Width;
        public int Height;
        public Transform Prefab;
        public Transform Visual;

        public Vector2Int GetRotationOffset(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.Left:
                    return new Vector2Int(0, Width);
                case Direction.Right:
                    return new Vector2Int(Height, 0);
                case Direction.Forward:
                    return new Vector2Int(Width, Height);
                case Direction.Back:
                    return Vector2Int.zero;
            }
        }

        public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (direction)
            {
                default:
                case Direction.Forward:
                case Direction.Back:
                    for (int i = 0; i < Width; i++)
                    {
                        for (int j = 0; j < Height; j++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(i, j));
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int i = 0; i < Height; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(i, j));
                        }
                    }
                    break;
            }

            return gridPositionList;
        }
    }
}

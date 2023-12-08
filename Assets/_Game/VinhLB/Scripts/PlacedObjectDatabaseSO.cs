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
                case Direction.Left:
                    return Direction.Forward;
                case Direction.Forward:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Back;
                case Direction.Back:
                default:
                    return Direction.Left;
            }
        }

        public static float GetRotationAngle(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return 270f;
                case Direction.Right:
                    return 90f;
                case Direction.Forward:
                    return 0;
                case Direction.Back:
                default:
                    return 180f;
            }
        }
        
        public static Vector2Int GetRotationOffset(int width, int height, Direction direction)
        {
            width = Mathf.Clamp(width - 1, 0, width - 1);
            height = Mathf.Clamp(height - 1, 0, height - 1);
            
            switch (direction)
            {
                case Direction.Left:
                    return new Vector2Int(0, width);
                case Direction.Right:
                    return new Vector2Int(height, 0);
                case Direction.Forward:
                    return new Vector2Int(width, height);
                case Direction.Back:
                default:
                    return Vector2Int.zero;
            }
        }
        
        public static List<Vector2Int> GetGridPositionList(int width, int height, Vector2Int offset, Direction direction)
        {
            List<Vector2Int> gridPositionList = new List<Vector2Int>();
            switch (direction)
            {
                default:
                case Direction.Forward:
                case Direction.Back:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(i, j));
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(i, j));
                        }
                    }
                    break;
            }

            return gridPositionList;
        }

        public int ID;
        public string Name;
        public int Width;
        public int Height;
        public Transform Prefab;
        public Transform Visual;
    }
}

using _Game.GameGrid;
using _Game.GameGrid.Unit;
using GameGridEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class PlacedObject : GridUnitStatic
    {
        public static PlacedObject Create(PlacedObjectData placedObjectData, Vector3 worldPosition, Vector2Int origin, Direction direction)
        {
            Transform placedObjectTransform = Instantiate(placedObjectData.Prefab, 
                worldPosition, Quaternion.Euler(0, PlacedObjectData.GetRotationAngle(direction), 0));

            PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
            placedObject._placedObjectData = placedObjectData;
            placedObject._origin = origin;
            placedObject._direction = direction;

            return placedObject;
        }

        private PlacedObjectData _placedObjectData;
        private Vector2Int _origin; 
        private Direction _direction;

        public List<Vector2Int> GetGridPositionList()
        {
            return _placedObjectData.GetGridPositionList(_origin, _direction);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}

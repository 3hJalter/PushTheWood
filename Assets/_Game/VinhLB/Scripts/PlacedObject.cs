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
        public static PlacedObject Create(PlacedObjectData placedObjectData)
        {
            Transform placedObjectTransform = Instantiate(placedObjectData.Prefab);
            PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

            return placedObject;
        }
    }
}

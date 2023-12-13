using _Game.GameGrid.Unit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VinhLB
{
    public class BuildingUnit : GridUnit
    {
        public static BuildingUnit Create(BuildingUnitData buildingUnitData)
        {
            BuildingUnit unit = Instantiate(buildingUnitData.Prefab);

            return unit;
        }
    }
}

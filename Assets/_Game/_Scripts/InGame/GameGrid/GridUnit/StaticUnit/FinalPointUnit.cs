using _Game.GameGrid.GridUnit.DynamicUnit;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class FinalPointUnit : GridUnitStatic
    {
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            base.OnInteract(direction, interactUnit);
            if (interactUnit is PlayerUnit)
            {
                Debug.Log("Win");
            }
        }
    }
}

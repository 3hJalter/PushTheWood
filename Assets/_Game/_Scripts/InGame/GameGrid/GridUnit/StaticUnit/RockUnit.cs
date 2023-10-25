using _Game.GameGrid.GridUnit.DynamicUnit;
using _Game.GameGrid.GridUnit.StaticUnit.Interface;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class RockUnit : GridUnitStatic, IPlayerInteracted
    {
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            base.OnInteract(direction, interactUnit);
            if (interactUnit is not PlayerUnit playerUnit) return;
            OnPlayerInteracted(direction, playerUnit);
        }

        public void OnPlayerInteracted(Direction direction, PlayerUnit player)
        {
            // Check if below player unit is vehicle
            GridUnit belowPlayerUnit = player.GetBelowUnit();
            if (belowPlayerUnit is not IVehicle vehicleUnit) return;
            // invert direction
            direction = GridUnitFunc.InvertDirection(direction);
            vehicleUnit.OnMove(direction);
        }
    }
}

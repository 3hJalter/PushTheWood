using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class BridgeUnit : GridUnitStatic
    {
        [SerializeField] private BridgeType bridgeType;

        public BridgeType BridgeType => bridgeType;
    }

    public enum BridgeType
    {
        BridgeHorizontal = 0,
        BridgeVertical = 1
    }
}

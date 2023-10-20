using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.DynamicUnit
{
    public class RaftUnit : GridUnitDynamic
    {
        public void OnInit(GameGridCell mainCellIn, ChumpType type, HeightLevel startHeightIn = HeightLevel.ZeroPointFive)
        {
            base.OnInit(mainCellIn, startHeightIn);
            RotateSkin(type);
        }

        private void RotateSkin(ChumpType type)
        {
            skin.localRotation =
                Quaternion.Euler(type is ChumpType.Horizontal ? Constants.horizontalSkinRotation : Constants.verticalSkinRotation);
            if (type is ChumpType.Vertical) size = new Vector3Int(size.z, size.y, size.x);
        }
    }
}

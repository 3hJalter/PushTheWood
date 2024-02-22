using _Game.DesignPattern;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class TreeSeed : GridUnitStatic
    {
        private bool _isGrown;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            _isGrown = false;
        }

        [ContextMenu("Grow Tree")] // TEST
        public void OnGrow()
        {
            _isGrown = true;
           // TODO: Some animation
           // On Complete Animation, Spawn the tree, Despawn the seed
           Tree tree = SimplePool.Spawn<Tree>(PoolType.TreeShort);
           tree.OnInit(mainCell, startHeight);
           // Despawn
           OnDespawn();
        }
    }
}

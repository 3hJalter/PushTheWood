using _Game.DesignPattern;
using _Game.InGame.GameGrid.GridUnit.DynamicUnit;
using _Game.Managers;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class TreeUnit : GridUnitStatic
    {
        [SerializeField] private ChumpUnit chumpSpawn;
        // SPAGHETTI CODE, change later
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            // Spawn TreeRootUnit at height of tree and Chump at height of tree + 1
            // Spawn TreeRoot
            TreeRootUnit treeRoot = SimplePool.Spawn<TreeRootUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.TreeRoot));
            treeRoot.OnInit(mainCell, startHeight);
            // Spawn Chump
            ChumpUnit chump = SimplePool.Spawn<ChumpUnit>(chumpSpawn);
            chump.OnInit(mainCell, startHeight + 1);
            // chump.Tf.position -= treeRoot.offsetY;
            chump.OnInteract(direction);
            // OnPushChump(direction);
            OnDespawn();
        }
    }
}

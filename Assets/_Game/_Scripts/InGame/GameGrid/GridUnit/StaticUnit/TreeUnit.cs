using _Game.DesignPattern;
using _Game.GameGrid.GridUnit.DynamicUnit;
using _Game.Managers;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.GridUnit.StaticUnit
{
    public class TreeUnit : GridUnitStatic
    {
        [SerializeField] private ChumpUnit chumpSpawn;
        // Spawn TreeRootUnit at height of tree and Chump at height of tree + 1
        public override void OnInteract(Direction direction, GridUnit interactUnit = null)
        {
            if (!gameObject.activeSelf) return;
            if (interactUnit is not PlayerUnit) return;
            // Spawn TreeRoot
            SpawnTreeRoot();
            // Spawn Chump and Push it
            SpawnChump().OnInteract(direction, interactUnit);
            OnDespawn();
        }

        private void SpawnTreeRoot()
        {
            TreeRootUnit treeRoot = SimplePool.Spawn<TreeRootUnit>(DataManager.Ins.GetGridUnitStatic(GridUnitStaticType.TreeRoot));
            treeRoot.OnInit(mainCell, startHeight);
            treeRoot.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(treeRoot);
        }

        private ChumpUnit SpawnChump()
        {
            ChumpUnit chump = SimplePool.Spawn<ChumpUnit>(chumpSpawn);
            chump.OnInit(mainCell, startHeight + 1);
            chump.islandID = islandID;
            LevelManager.Ins.AddNewUnitToIsland(chump);
            return chump;
        }
    }
}

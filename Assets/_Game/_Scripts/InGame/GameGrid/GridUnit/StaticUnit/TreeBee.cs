using System;
using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using _Game.Managers;
using _Game.Utilities;
using DG.Tweening;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class TreeBee : Tree
    {
        [SerializeField] private Tree treePrefab;
        [SerializeField] private Transform hive;
        private bool _isSetHiveLocalPos;
        private Vector3 _hiveInitLocalPos;
        private Vector3 _hiveFallPos;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, bool isUseInitData = true,
            Direction skinDirection = Direction.None, bool hasSetPosAndRot = false)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection, hasSetPosAndRot);
            hive.parent ??= skin;
            if (_isSetHiveLocalPos) hive.localPosition = _hiveInitLocalPos;
            _hiveFallPos = hive.position + new Vector3(0, -0.5f, 0);
        }

        public override void OnCutTree(GridUnit cutter)
        {
            base.OnCutTree(cutter);
            if (cutter is not Player player) return;
            
            // TEST: Make the hive fall down, then the player will be stunned -> End Game
            // TEMPORARY: Make the hive out of the tree
            hive.parent = null;
            DOVirtual.DelayedCall(Constants.CUT_TREE_TIME, () =>
            {
                player.IsStun = true;
                hive.DOMove(_hiveFallPos, 0.2f).OnComplete(() =>
                {
                    player.IsDead = true;
                });
            });
            DevLog.Log(DevId.Hoang, "Bee attack player");

            return;

            
            Direction GetDirectionFromPlayer()
            {
                Vector3 playerPos = player.MainCell.WorldPos;
                Vector3 treePos = mainCell.WorldPos;
                if (Math.Abs(playerPos.x - treePos.x) < 0.01f)
                    return playerPos.z > treePos.z ? Direction.Back : Direction.Forward;
                return playerPos.x > treePos.x ? Direction.Left : Direction.Right;
            }
        }

        // BUG: Tree also spawn with TreeBee when back the game
        protected override void OnPushedComplete()
        {
            if (!gameObject.activeSelf) return;
            // Spawn the Base tree in this cell + Save the state
            //LevelManager.Ins.SaveGameState(true);
            //pushedUnit.MainCell.ValueChange();
            //mainCell.ValueChange();
            //LevelManager.Ins.SaveGameState(false);
            // Spawn tree
            Tree tree = SimplePool.Spawn<Tree>(treePrefab);
            tree.OnInit(mainCell, startHeight);
            LevelManager.Ins.CurrentLevel.AddNewUnitToIsland(tree);
            // Then despawn it
            OnDespawn();
            LevelManager.Ins.SaveGameState(true);

        }

        protected override void OnAwakeCall()
        {
            _hiveInitLocalPos = hive.localPosition;
            _isSetHiveLocalPos = true;
        }

        protected override void OnDestroyCall()
        {
            _isSetHiveLocalPos = false;
        }
    }
}

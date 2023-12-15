using System;
using _Game.Camera;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Chump;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using GameGridEnum;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class Tree : GridUnitStatic
    {
        [SerializeField] public Chump chumpPrefab;
        [SerializeField] private Canvas cutTreeCanvas;

        public override void OnInit(GameGridCell mainCellIn, HeightLevel startHeightIn = HeightLevel.One, 
            bool isUseInitData = true, Direction skinDirection = Direction.None)
        {
            base.OnInit(mainCellIn, startHeightIn, isUseInitData, skinDirection);
            cutTreeCanvas.worldCamera = CameraFollow.Ins.MainCamera;
        }

        protected override void OnEnterTriggerNeighbor(GridUnit triggerUnit)
        {
            if (triggerUnit is Player)
            {
                cutTreeCanvas.gameObject.SetActive(true);
            }
        }

        protected override void OnOutTriggerNeighbor(GridUnit triggerUnit)
        {
            if (triggerUnit is Player)
            {
                cutTreeCanvas.gameObject.SetActive(false);
            }
        }

        public override void OnDespawn()
        {
            cutTreeCanvas.gameObject.SetActive(false);
            base.OnDespawn();
        }

        public void CutTree()
        {
            // Get the Player from Level Manager
            Player player = LevelManager.Ins.Player;
            // Change it state to cut tree
            player.CutTreeData.SetData(GetDirectionFromPlayer(), this);
            player.ChangeState(StateEnum.CutTree);
            return;
            
            Direction GetDirectionFromPlayer()
            {
                Vector3 playerPos = player.MainCell.WorldPos;
                Vector3 treePos = mainCell.WorldPos;
                if (Math.Abs(playerPos.x - treePos.x) < 0.01f)
                {
                    return playerPos.y > treePos.y ? Direction.Back : Direction.Forward;
                }
                return playerPos.x > treePos.x ? Direction.Left : Direction.Right;
            }
        }
    }
}

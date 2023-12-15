using System;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.DynamicUnit.Chump;
using _Game.GameGrid.Unit.DynamicUnit.Player;
using UnityEngine;

namespace _Game.GameGrid.Unit.StaticUnit
{
    public class Tree : GridUnitStatic
    {
        [SerializeField] public Chump chumpPrefab;

        protected override void OnInteractBtnClick()
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
                    return playerPos.z > treePos.z ? Direction.Back : Direction.Forward;
                }
                return playerPos.x > treePos.x ? Direction.Left : Direction.Right;
            }
        }
    }
}

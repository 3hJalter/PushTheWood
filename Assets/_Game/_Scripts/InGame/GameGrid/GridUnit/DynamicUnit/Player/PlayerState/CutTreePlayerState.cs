using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using _Game.Utilities.Timer;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class CutTreePlayerState : IState<Player>
    {
        private bool _isExecuted;
        float originAnimSpeed;

        public StateEnum Id => StateEnum.CutTree;

        public void OnEnter(Player t)
        {
            originAnimSpeed = t.AnimSpeed;
            t.SetAnimSpeed(originAnimSpeed * Constants.CUT_TREE_ANIM_TIME / Constants.CUT_TREE_TIME);
            t.ChangeAnim(Constants.CUT_TREE_ANIM);
            t.LookDirection(t.CutTreeData.inputDirection);
            TimerManager.Inst.WaitForTime(Constants.CUT_TREE_TIME, () => CutTreeAction(t));
        }

        public void OnExecute(Player t)
        {
           
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
            t.SetAnimSpeed(originAnimSpeed);
        }

        private void CutTreeAction(Player t)
        {
            if (!_isExecuted)
            {
                _isExecuted = true;
                // Make the tree OnBePushed with direction calculated by position of the Player and the Tree
                // Spawn a TreeRoot
                TreeRoot treeRoot = SimplePool.Spawn<TreeRoot>(DataManager.Ins.GetGridUnit(PoolType.TreeRoot));
                treeRoot.OnInit(t.CutTreeData.tree.MainCell, t.CutTreeData.tree.StartHeight);
                LevelManager.Ins.AddNewUnitToIsland(treeRoot);
                // Spawn a Chump
                Chump.Chump chump = SimplePool.Spawn<Chump.Chump>(t.CutTreeData.tree.chumpPrefab);
                chump.OnInit(t.CutTreeData.tree.MainCell, t.CutTreeData.tree.StartHeight + 1);
                LevelManager.Ins.AddNewUnitToIsland(chump);
                // Push the Chump with the direction
                chump.OnBePushed(t.CutTreeData.inputDirection, t);
                // Despawn the Tree
                t.CutTreeData.tree.OnDespawn();
                t.StateMachine.ChangeState(StateEnum.Idle);
            }
        }
    }
}

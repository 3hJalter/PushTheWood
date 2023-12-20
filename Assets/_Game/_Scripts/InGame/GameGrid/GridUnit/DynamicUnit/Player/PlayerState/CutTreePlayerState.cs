using _Game.DesignPattern;
using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using _Game.Managers;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class CutTreePlayerState : IState<Player>
    {
        private bool _isExecuted;
        private float _timeCounter;

        public void OnEnter(Player t)
        {
            _timeCounter = 0.5f;
            t.ChangeAnim(Constants.CUT_TREE_ANIM);
            t.LookDirection(t.CutTreeData.inputDirection);
        }

        public void OnExecute(Player t)
        {
            if (_timeCounter > 0)
            {
                _timeCounter -= Time.fixedDeltaTime;
                return;
            }

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
            }

            if (t.IsCurrentAnimDone()) t.ChangeState(StateEnum.Idle);
        }

        public void OnExit(Player t)
        {
            _isExecuted = false;
        }
    }
}

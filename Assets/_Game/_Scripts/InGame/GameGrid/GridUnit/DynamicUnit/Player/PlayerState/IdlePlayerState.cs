using _Game.DesignPattern.StateMachine;
using _Game.GameGrid.Unit.StaticUnit;
using UnityEngine;

namespace _Game.GameGrid.Unit.DynamicUnit.Player.PlayerState
{
    public class IdlePlayerState : IState<Player>
    {
        private bool _isChangeAnim;
        private bool isFirstStop;
        bool hasTreeRoot = false;

        public StateEnum Id => StateEnum.Idle;

        public void OnEnter(Player t)
        {
            isFirstStop = true;
        }

        public void OnExecute(Player t)
        {
            //NOTE:Checking for IdleState
            if (t.Direction == Direction.None)
            {
                if (!_isChangeAnim && !t.isRideVehicle)
                {
                    _isChangeAnim = true;
                    t.ChangeAnim(Constants.IDLE_ANIM);
                }
                return;
            }
            t.LookDirection(t.Direction);
            //NOTE: Checking for riding raft or something like that
            if (t.HasVehicle() && !t.isRideVehicle)
                if (t.CanRideVehicle(t.Direction))
                {
                    t.OnRideVehicle(t.Direction);
                    return;
                }
            t.MovingData.SetData(t.Direction);
            if (!t.conditionMergeOnMoving.IsApplicable(t.MovingData))
            {
                //NOTE: Checking to push an dynamic object
                if (t.MovingData.blockDynamicUnits.Count > 0)
                    t.StateMachine.ChangeState(StateEnum.Push);               
                else if (!_isChangeAnim) t.ChangeAnim(Constants.IDLE_ANIM);

                //NOTE: Checking to push an static object
                if (t.MovingData.blockStaticUnits.Count > 0)
                {
                    switch (t.MovingData.blockStaticUnits[0])
                    {
                        case StaticUnit.Tree tree:
                            if (isFirstStop)
                            {
                                //NOTE: Checking button down of player input
                                if (t.InputDetection.InputAction == InputAction.ButtonDown) isFirstStop = false;
                                if (!_isChangeAnim) t.ChangeAnim(Constants.IDLE_ANIM);
                            }
                            else
                            {
                                tree.OnInteract();
                            }
                            break;
                        case Rock rock:
                            if (t.InputDetection.InputAction == InputAction.ButtonDown)
                                t.StateMachine.ChangeState(StateEnum.Push);
                            break;
                    }
                }
                return;
            }

            hasTreeRoot = false;
            if (t.MovingData.blockStaticUnits.Count == 1) 
            {
                switch (t.MovingData.blockStaticUnits[0])
                {
                    case TreeRoot treeRoot:
                        hasTreeRoot = true;
                        treeRoot.UpHeight(t, t.MovingData.inputDirection);
                        break;
                }               
            }

            t.SetEnterCellData(t.MovingData.inputDirection, t.MovingData.enterMainCell, t.UnitTypeY);
            t.SetIslandId(t.MovingData.enterMainCell);
            t.OnOutCells();
            t.OnEnterCells(t.MovingData.enterMainCell, t.MovingData.enterCells);
            if (hasTreeRoot) 
                t.StateMachine.ChangeState(StateEnum.JumpUp);
            else
                t.StateMachine.ChangeState(t.EnterPosData.isFalling ? StateEnum.JumpDown : StateEnum.Move);
        }

        public void OnExit(Player t)
        {
            _isChangeAnim = false;
        }
    }
}
